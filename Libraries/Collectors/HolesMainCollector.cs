using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;

namespace Libraries.Collectors
{
    /// <summary>
    /// заглушки/отверстия из основного файла
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="errorModel"></param>
    public class HolesMainCollector(Document doc, ErrorModel errorModel)
    {
        private readonly LevelAnyObject _levelAnyObject = new(doc);
        private readonly Document _doc = doc;
        private readonly ErrorModel _errorModel = errorModel;


        /// <summary>
        /// <para> Возвращает коллекции заглушек из основного файла. </para>
        /// <para> Все заглушки как с рабочей плоскостью, так и без нее </para>
        /// <para> и заглушки без рабочей плоскости </para>
        /// </summary>
        /// <returns></returns>
        public MainHolesResult GetCollections()
        {
            

            // Собираем элементы из категории Оборудование
            FilteredElementCollector collector = new FilteredElementCollector(_doc)
                                .OfCategory(BuiltInCategory.OST_MechanicalEquipment)
                                .OfClass(typeof(FamilyInstance))
                                .WhereElementIsNotElementType();

            // Получаем количество элементов «нативно» без загрузки в память C#
            int collectorSize = collector.GetElementCount();

            // Если в проекте не размещено ни одной заглушки завершаем код с уведомлением пользователя
            if (collectorSize == 0)
            {
                _errorModel.UserWarning(new NoHolesInProject().MessageForUser());
                //return new AllHolesSortingResult(0);
            }

            var mainHolesResult = new MainHolesResult(collectorSize);

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
                    mainHolesResult.UniqueHoleTypes.Add(inst);
                    mainHolesResult.UniqTypeNames[symbolId.Value] = inst.Symbol.Name;
                }

                Level level;  // объявляем переменную level, не назначив значение

                if (inst.Host != null)  // если заглушка имеет значение параметра Рабочая плоскость
                {
                    level = _levelAnyObject.GetLevel(inst);
                    if (level == null)
                    {
                        _errorModel.UserWarning(new FamilyHasNoLevel().MessageForUser(inst)); // завершаем код
                        //return mainHolesSortingResult;
                    }
                }
                else  // (inst.Host == null) если у заглушки значение параметра Рабочая плоскость 'не связано'
                {
                    level = _levelAnyObject.GetСlosestLevel(inst);
                    if (level == null)
                    {
                        _errorModel.UserWarning(new FamilyHasNoLevel().MessageForUser(inst)); // завершаем код
                        //return mainHolesSortingResult;
                    }
                    ElementId levId = level.Id;

                    // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждого FamilyInstance не вызывать level.Elevation повторно
                    if (!levelKeyCache.TryGetValue(levId, out long levKey))  // если ключа/ElementId уровня еще нет в словаре, то создаем его
                    {
                        // округление во избежание погрешностей float/double
                        levKey = (long)Math.Round(level.Elevation * 100);
                        levelKeyCache[levId] = levKey;
                    }
                    mainHolesResult.NoHostAllHoles.Add(inst);

                    // Получаем или создаем LevelHolesGroup
                    if (!mainHolesResult.NoHostHolesByLevels.TryGetValue(levKey, out LevelHolesGroup levelGroup))
                    {
                        levelGroup = new LevelHolesGroup(level.Name);
                        mainHolesResult.NoHostHolesByLevels[levKey] = levelGroup;
                    }
                    // Добавляем заглушку в HashSet внутри контейнера
                    levelGroup.Holes.Add(inst);
                }


                ElementId levelId = level.Id;

                // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждого FamilyInstance не вызывать level.Elevation повторно
                if (!levelKeyCache.TryGetValue(levelId, out long levelKey))  // если ключа/ElementId уровня еще нет в словаре, то создаем его
                {
                    // округление во избежание погрешностей float/double
                    levelKey = (long)Math.Round(level.Elevation * 100);
                    levelKeyCache[levelId] = levelKey;
                }

                mainHolesResult.AllHoles.Add(inst);

                if (!mainHolesResult.AllHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> allSet))
                {
                    allSet = [];
                    mainHolesResult.AllHolesByLevels[levelKey] = allSet;
                }
                allSet.Add(inst);
            }
            return mainHolesResult;
        }
    }


    /// <summary>
    /// Класс-контейнер для хранения результатов классификации отверстий в стенах и перекрытиях
    /// </summary>
    public class MainHolesResult(int capacity = 0)
    {
        /// <summary>
        /// Все отверстия, как с рабочей плоскостью, так и без нее
        /// </summary>
        public List<FamilyInstance> AllHoles { get; } = new List<FamilyInstance>(capacity);  // capacity заранее задаваемая емкость списка

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
