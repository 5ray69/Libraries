using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;
using Libraries.ParametersLib;

namespace Libraries.Collectors
{
    /// <summary>
    /// Заглушки/отверстия из основного файла электрика
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="errorModel"></param>
    public class HolesElectricCollector(Document doc, ErrorModel errorModel)
    {
        private readonly LevelAnyObject _levelAnyObject = new(doc);
        private readonly Document _doc = doc;
        private readonly ErrorModel _errorModel = errorModel;
        private readonly ParameterValidatorIsMissing parameterValidatorIsMissing = new(errorModel);


        /// <summary>
        /// <para> Возвращает списки заглушек </para>
        /// <para> круглые в стенах, </para>
        /// <para> прямогульные в стенах </para>
        /// <para> и круглые с прямоугольными в перекрытиях </para>
        /// </summary>
        /// <returns></returns>
        public ElHolesSortingResult GetCollections()
        {

            // Создаем HashSet имен семейств
            HashSet<string> namesOfHoleFamilies = [
                                                                "BDV_Заглушка круглая в стене",
                                                                "BDV_Заглушка_прямоугольная_в_стене",
                                                                "BDV_Заглушка круглая в перекрытии",
                                                                "BDV_Заглушка_прямоугольная_в_перекрытии"
                                                                ];


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
                //return result;
            }
            
            var elHolesSortingResult = new ElHolesSortingResult(collectorSize);

            // Локальный кэш для отметки уровня, чтобы не вызывать level.Elevation для каждого FamilyInstance повторно
            Dictionary<ElementId, long> levelKeyCache = [];

            foreach (Element elem in collector)
            {
                FamilyInstance inst = (FamilyInstance)elem;

                FamilySymbol symbol = inst.Symbol;
                if (symbol == null) continue;

                Family family = symbol.Family;
                if (family == null) continue;

                string familyName = family.Name;

                // Проверяем, входит ли семейство в список имен семейств
                if (!namesOfHoleFamilies.Contains(familyName)) continue;

                // если галка стоит
                Parameter paramEl = parameterValidatorIsMissing.ValidateExistsParameter(inst, "ЭЛ");
                int el = paramEl.AsInteger();
                if (el != 1) continue;


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
                    elHolesSortingResult.NoHostAllHoles.Add(inst);

                    // Получаем или создаем LevelHolesGroup
                    if (!elHolesSortingResult.NoHostHolesByLevels.TryGetValue(levKey, out LevelHolesGroup levelGroup))
                    {
                        levelGroup = new LevelHolesGroup(level.Name);
                        elHolesSortingResult.NoHostHolesByLevels[levKey] = levelGroup;
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


                elHolesSortingResult.HolesAll.Add(inst);

                if (!elHolesSortingResult.AllHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> allSet))
                {
                    allSet = [];
                    elHolesSortingResult.AllHolesByLevels[levelKey] = allSet;
                }
                allSet.Add(inst);


                switch (familyName)
                {
                    case "BDV_Заглушка круглая в стене":
                        elHolesSortingResult.RoundWallHoles.Add(inst);

                        if (!elHolesSortingResult.RoundHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> roundSet))
                        {
                            roundSet = [];
                            elHolesSortingResult.RoundHolesByLevels[levelKey] = roundSet;
                        }
                        roundSet.Add(inst);
                        break;

                    case "BDV_Заглушка_прямоугольная_в_стене":
                        elHolesSortingResult.RectangularWallHoles.Add(inst);

                        if (!elHolesSortingResult.RectangularHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> rectangularSet))
                        {
                            rectangularSet = [];
                            elHolesSortingResult.RectangularHolesByLevels[levelKey] = rectangularSet;
                        }
                        rectangularSet.Add(inst);
                        break;

                    default:
                        elHolesSortingResult.HolesInTheFloor.Add(inst);

                        if (!elHolesSortingResult.FloorHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> floorSet))
                        {
                            floorSet = [];
                            elHolesSortingResult.FloorHolesByLevels[levelKey] = floorSet;
                        }
                        floorSet.Add(inst);
                        break;
                }
            }
            return elHolesSortingResult;
        }
    }


    /// <summary>
    /// Класс-контейнер для хранения результатов классификации отверстий в стенах и перекрытиях
    /// </summary>
    public class ElHolesSortingResult(int capacity = 0)
    {
        /// <summary>
        /// список круглых заглушек/отверстий в стенах
        /// </summary>
        public List<FamilyInstance> RoundWallHoles { get; } = new List<FamilyInstance>(capacity);  // capacity заранее задаваемая емкость списка
        
        /// <summary>
        /// список прямоугольных заглушек/отверстий в стенах
        /// </summary>
        public List<FamilyInstance> RectangularWallHoles { get; } = new List<FamilyInstance>(capacity);  // capacity заранее задаваемая емкость списка
        
        /// <summary>
        /// список заглушек/отверстий в перекрытиях
        /// </summary>
        public List<FamilyInstance> HolesInTheFloor { get; } = new List<FamilyInstance>(capacity);  // capacity заранее задаваемая емкость списка
        
        /// <summary>
        /// список всех заглушек/отверстий
        /// </summary>
        public List<FamilyInstance> HolesAll { get; } = new List<FamilyInstance>(capacity);  // capacity заранее задаваемая емкость списка


        /// <summary>
        /// Ключ - высотная отметка Elevation уровня, значение - HashSet заглушек
        /// </summary>
        public Dictionary<long, HashSet<FamilyInstance>> RoundHolesByLevels { get; } = [];

        /// <summary>
        /// Ключ - высотная отметка Elevation уровня, значение - HashSet заглушек
        /// </summary>
        public Dictionary<long, HashSet<FamilyInstance>> RectangularHolesByLevels { get; } = [];

        /// <summary>
        /// Ключ - высотная отметка Elevation уровня, значение - HashSet заглушек
        /// </summary>
        public Dictionary<long, HashSet<FamilyInstance>> FloorHolesByLevels { get; } = [];

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
    }
}



