using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;
using Libraries.ParametersLib;

namespace IntersectionsOfHolesAndWalls.LogicRevit.Collectors
{
    /// <summary>
    /// Коллектор семейств-заглушек
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="errorModel"></param>
    public class HolesCollector(Document doc, ErrorModel errorModel)
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
        public HolesSortingResult GetCollections()
        {
            var holesSortingResult = new HolesSortingResult();

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
                                .OfClass(typeof(FamilyInstance));

            // Получаем количество элементов «нативно» без загрузки в память C#
            int collectorSize = collector.GetElementCount();

            // Если в проекте не размещено ни одной заглушки завершаем код с уведомлением пользователя
            if (collectorSize == 0)
            {
                _errorModel.UserWarning(new NoHolesInProject().MessageForUser());
                //return result;
            }

            // Локальный кэш для отметки уровня, чтобы не вызывать level.Elevation для каждого FamilyInstance повторно
            Dictionary<ElementId, long> levelKeyCache = [];

            foreach (Element elem in collector)
            {
                // Безопасно получаем имя семейства через цепочку свойств
                if (elem is not FamilyInstance inst) continue;

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


                Level level = _levelAnyObject.GetLevel(inst);
                if (level == null)
                {
                    _errorModel.UserWarning(new FamilyHasNoLevel().MessageForUser(inst)); // завершаем код
                    //return result;
                }

                ElementId levelId = level.Id;

                // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждого FamilyInstance не вызывать level.Elevation повторно
                if (!levelKeyCache.TryGetValue(levelId, out long levelKey))  // если ключа/ElementId уровня еще нет в словаре, то создаем его
                {
                    // округление во избежание погрешностей float/double
                    levelKey = (long)Math.Round(level.Elevation * 100);
                    levelKeyCache[levelId] = levelKey;
                }


                holesSortingResult.HolesAll.Add(inst);

                if (!holesSortingResult.AllHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> allSet))
                {
                    allSet = [];
                    holesSortingResult.AllHolesByLevels[levelKey] = allSet;
                }
                allSet.Add(inst);


                switch (familyName)
                {
                    case "BDV_Заглушка круглая в стене":
                        holesSortingResult.RoundWallHoles.Add(inst);

                        if (!holesSortingResult.RoundHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> roundSet))
                        {
                            roundSet = [];
                            holesSortingResult.RoundHolesByLevels[levelKey] = roundSet;
                        }
                        roundSet.Add(inst);
                        break;

                    case "BDV_Заглушка_прямоугольная_в_стене":
                        holesSortingResult.RectangularWallHoles.Add(inst);

                        if (!holesSortingResult.RectangularHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> rectangularSet))
                        {
                            rectangularSet = [];
                            holesSortingResult.RectangularHolesByLevels[levelKey] = rectangularSet;
                        }
                        rectangularSet.Add(inst);
                        break;

                    default:
                        holesSortingResult.HolesInTheFloor.Add(inst);

                        if (!holesSortingResult.FloorHolesByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> floorSet))
                        {
                            floorSet = [];
                            holesSortingResult.FloorHolesByLevels[levelKey] = floorSet;
                        }
                        floorSet.Add(inst);
                        break;
                }
            }
        return holesSortingResult;
        }
    }


    /// <summary>
    /// Класс-контейнер для хранения результатов классификации отверстий в стенах и перекрытиях
    /// </summary>
    public class HolesSortingResult
    {
        /// <summary>
        /// Список круглых заглушек в стенах
        /// </summary>
        public List<FamilyInstance> RoundWallHoles { get; } = [];

        /// <summary>
        /// Список прямоугольных заглушек в стенах
        /// </summary>
        public List<FamilyInstance> RectangularWallHoles { get; } = [];

        /// <summary>
        /// Список круглых и прямоугольных заглушек в перекрытиях
        /// </summary>
        public List<FamilyInstance> HolesInTheFloor { get; } = [];

        /// <summary>
        /// Список всех заглушек в проекте
        /// </summary>
        public List<FamilyInstance> HolesAll { get; } = [];


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
    }
}



