using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;

namespace Libraries.Collectors
{
    /// <summary>
    /// Заглушки/отверстия из связанного файла
    /// </summary>
    /// <param name="linkDoc"></param>
    /// <param name="errorModel"></param>
    public class HolesLinkCollector(Document linkDoc, ErrorModel errorModel)
    {
        private readonly LevelAnyObject _levelAnyObject = new(linkDoc);
        private readonly Document _linkDoc = linkDoc;
        private readonly ErrorModel _errorModel = errorModel;

        /// <summary>
        /// Возвращает заглушки/отверстия из связанного файла
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public LinkHolesResult GetHoles(Transform transform)
        {


            // Собираем элементы из категории Оборудование
            FilteredElementCollector collector = new FilteredElementCollector(_linkDoc)
                                .OfCategory(BuiltInCategory.OST_MechanicalEquipment)
                                .OfClass(typeof(FamilyInstance))
                                .WhereElementIsNotElementType();

            // Получаем количество элементов «нативно» без загрузки в память C#
            int collectorSize = collector.GetElementCount();

            // Если в проекте не размещено ни одной заглушки завершаем код с уведомлением пользователя
            if (collectorSize == 0)
            {
                _errorModel.UserWarning(new NoHolesInProject().MessageForUser());
                //return new LinkHolesResult(0);
            }

            LinkHolesResult linkHolesResult = new(collectorSize);

            // Локальный кэш для отметки уровня, чтобы не вызывать level.Elevation для каждого FamilyInstance повторно
            Dictionary<ElementId, long> levelKeyCache = [];

            // Вспомогательный HashSet для отслеживания уникальных типов (символов) семейств на лету
            HashSet<ElementId> symbolIds = [];

            foreach (Element elem in collector)
            {
                FamilyInstance inst = (FamilyInstance)elem;

                // Собираем уникальные экземпляры по их типу (FamilySymbol), максимум 4 штуки
                ElementId symbolId = inst.Symbol?.Id;
                if (symbolId != null && symbolIds.Add(symbolId))
                {
                    linkHolesResult.UniqueHoleTypes.Add(inst);
                    linkHolesResult.UniqTypeNames[symbolId.Value] = inst.Symbol.Name;
                }

                Level level = null;

                if (inst.Host != null)  // если заглушка имеет значение параметра Рабочая плоскость
                {
                    level = _levelAnyObject.GetLevel(inst);
                    if (level == null)
                    {
                        _errorModel.UserWarning(new FamilyHasNoLevel().MessageForUser(inst)); // завершаем код
                        //return linkHolesResult;
                    }
                }
                
                if (inst.Host == null)  // если у заглушки значение параметра Рабочая плоскость 'не связано'
                {
                    linkHolesResult.NoHostAllHoles.Add(inst);
                    level = _levelAnyObject.GetСlosestLevel(inst);
                    ElementId levId = level.Id;


                    // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждого FamilyInstance не вызывать level.Elevation повторно
                    if (!levelKeyCache.TryGetValue(level.Id, out long levKey))  // если ключа/ElementId уровня еще нет в словаре, то создаем его
                    {
                        // Берем отметку уровня в локальных координатах связи и формируем точку
                        XYZ localPoint = new(0, 0, level.Elevation);

                        // Трансформируем точку в глобальные координаты основного файла
                        XYZ hostPoint = transform.OfPoint(localPoint);

                        // Получаем реальную высоту в координатах основного файла (берем Z-координату)
                        double hostElevation = hostPoint.Z;

                        // Теперь округляем для сопоставления c высотной отметкой уровня в основном файле
                        // округление во избежание погрешностей float/double
                        levKey = (long)Math.Round(hostElevation * 100);
                        levelKeyCache[levId] = levKey;
                    }

                    // Получаем или создаем LevelHolesGroup
                    if (!linkHolesResult.NoHostHolesByLevels.TryGetValue(levKey, out LevelHolesGroup levelHoles))
                    {
                        levelHoles = new LevelHolesGroup(level.Name);
                        linkHolesResult.NoHostHolesByLevels[levKey] = levelHoles;
                    }
                    // Добавляем заглушку в HashSet внутри контейнера
                    levelHoles.Holes.Add(inst);
                }


                ElementId levelId = level.Id;

                // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждого FamilyInstance не вызывать level.Elevation повторно
                if (!levelKeyCache.TryGetValue(levelId, out long levelKey))  // если ключа/ElementId уровня еще нет в словаре, то создаем его
                {
                    // Берем отметку уровня в локальных координатах связи и формируем точку
                    XYZ localPoint = new(0, 0, level.Elevation);

                    // Трансформируем точку в глобальные координаты основного файла
                    XYZ hostPoint = transform.OfPoint(localPoint);

                    // Получаем реальную высоту в координатах основного файла (берем Z-координату)
                    double hostElevation = hostPoint.Z;

                    // Теперь округляем для сопоставления c высотной отметкой уровня в основном файле
                    // округление во избежание погрешностей float/double
                    levelKey = (long)Math.Round(hostElevation * 100);
                    levelKeyCache[levelId] = levelKey;
                }


                if (!linkHolesResult.AllHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> allSet))
                {
                    allSet = [];
                    linkHolesResult.AllHolesByLevels[levelKey] = allSet;
                }
                allSet.Add(inst);

            }
            return linkHolesResult;
        }
    }


    /// <summary>
    /// Класс-контейнер для хранения результатов классификации отверстий в стенах и перекрытиях
    /// </summary>
    public class LinkHolesResult(int capacity = 0)
    {
        /// <summary>
        /// Ключ - высотная отметка Elevation уровня, значение - HashSet заглушек
        /// </summary>
        public Dictionary<long, HashSet<FamilyInstance>> AllHolesByLevels { get; } = [];

        /// <summary>
        /// <para> Ключ - высотная отметка Elevation уровня, значение - HashSet заглушек </para>
        /// <para> Отверстия/заглушки, у которых значение параметра Рабочая плоскость 'не связано' </para>
        /// </summary>
        public Dictionary<long, LevelHolesGroup> NoHostHolesByLevels { get; } = [];

        /// <summary>
        /// Отверстия/заглушки, у которых значение параметра Рабочая плоскость 'не связано'
        /// </summary>
        public List<FamilyInstance> NoHostAllHoles { get; } = new List<FamilyInstance>(capacity);  // capacity заранее задаваемая емкость списка

        /// <summary>
        /// Уникальные представители (экземпляры) каждого типа семейства (максимум 4 штуки) для проверки параметров
        /// </summary>
        public List<FamilyInstance> UniqueHoleTypes { get; } = new(4);

        /// <summary>
        /// Ключ - ElementId.Value типоразмера отверстия, значение - имя типоразмера
        /// </summary>
        public Dictionary<long, string> UniqTypeNames { get; } = [];
    }
}
