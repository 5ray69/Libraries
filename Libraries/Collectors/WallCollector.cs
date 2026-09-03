using Autodesk.Revit.DB;

namespace HolesInConcreteByLevels.LogicRevit.Collectors
{
    /// <summary>
    /// <para> коллектор стен из связанного документа Revit, который фильтрует стены на бетонные и небетонные, </para> 
    /// <para> а также группирует их по уровням, по высотным отметкам Elevation уровней </para> 
    /// </summary>
    public class WallCollector
    {
        /// <summary>
        /// Список бетонных стен
        /// </summary>
        public List<Wall> ConcreteWalls { get; }

        /// <summary>
        /// Список небетонных стен
        /// </summary>
        public List<Wall> NoConcreteWalls { get; }

        /// <summary>
        /// Словарь бетонных стен по уровням
        /// </summary>
        public Dictionary<long, HashSet<Wall>> ConcreteWallsByLevels { get; }

        /// <summary>
        /// Словарь небетонных стен по уровням
        /// </summary>
        public Dictionary<long, HashSet<Wall>> NoConcreteWallsByLevels { get; }

        /// <summary>
        /// Конструктор класса, который собирает стены из связанного документа Revit
        /// </summary>
        /// <param name="linkDoc"></param>
        /// <param name="linkTransform"></param>
        public WallCollector(Document linkDoc, Transform linkTransform)
        {
            var collector = new FilteredElementCollector(linkDoc).OfClass(typeof(Wall));

            // Быстро получаем точное количество стен на стороне C++ ядра Revit
            int wallCount = collector.GetElementCount();

            // Сразу выделяем точный объем памяти, избегая пересоздания массивов
            ConcreteWalls = new List<Wall>(wallCount);
            NoConcreteWalls = new List<Wall>(wallCount);

            // Обязательная инициализация словарей для предотвращения NullReferenceException
            ConcreteWallsByLevels = new Dictionary<long, HashSet<Wall>>();
            NoConcreteWallsByLevels = new Dictionary<long, HashSet<Wall>>();

            // Локальный кэш для отметки уровня, чтобы не вызывать linkDoc.GetElement для каждой стены повторно
            Dictionary<ElementId, long> levelKeyCache = [];

            // Один-единственный проход по элементам
            foreach (Element elem in collector)
            {
                if (elem is Wall wall)
                {
                    ElementId levelId = wall.LevelId;
                    if (levelId == ElementId.InvalidElementId) continue;

                    // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждой стены не вызывать linkDoc.GetElement(levelId) повторно
                    if (!levelKeyCache.TryGetValue(levelId, out long mainLevelKey))  // если ключа/ElementId уровня еще нет в словаре
                    {
                        if (linkDoc.GetElement(levelId) is Level linkLevel)
                        {
                            XYZ linkPoint = new (0, 0, linkLevel.Elevation);
                            XYZ mainPoint = linkTransform.OfPoint(linkPoint);

                            // округление во избежание погрешностей float/double
                            mainLevelKey = (long)Math.Round(mainPoint.Z * 100);
                        }
                        else
                        {
                            mainLevelKey = 0;
                        }

                        levelKeyCache[levelId] = mainLevelKey;
                    }

                    // 2. Распределение по спискам и словарям
                    bool isConcrete = wall.Name != null && wall.Name.Contains("Бетон");

                    if (isConcrete)
                    {
                        ConcreteWalls.Add(wall);

                        if (!ConcreteWallsByLevels.TryGetValue(mainLevelKey, out HashSet<Wall> valueConcrete))
                        {
                            valueConcrete = new HashSet<Wall>();
                            ConcreteWallsByLevels[mainLevelKey] = valueConcrete;
                        }
                        valueConcrete.Add(wall);
                    }
                    else
                    {
                        NoConcreteWalls.Add(wall);

                        if (!NoConcreteWallsByLevels.TryGetValue(mainLevelKey, out HashSet<Wall> valueNoConcrete))
                        {
                            valueNoConcrete = new HashSet<Wall>();
                            NoConcreteWallsByLevels[mainLevelKey] = valueNoConcrete;
                        }
                        valueNoConcrete.Add(wall);
                    }
                }
            }
        }
    }
}
