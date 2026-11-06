using Autodesk.Revit.DB;

namespace Libraries.Collectors
{
    /// <summary>
    /// коллектор стен
    /// </summary>
    public class WallCollector
    {
        /// <summary>
        /// несущие/бетонные стены
        /// </summary>
        public List<Wall> ConcreteWalls { get; }

        /// <summary>
        /// ненесущие/небетонные стены "кирпич"
        /// </summary>
        public List<Wall> NoConcreteWalls { get; }

        /// <summary>
        /// возвращает несущие/бетонные и ненесущие/небетонные стены
        /// </summary>
        /// <param name="doc"></param>
        public WallCollector(Document doc)
        {
            var collector = new FilteredElementCollector(doc).OfClass(typeof(Wall));

            // Быстро получаем точное количество стен на стороне C++ ядра Revit
            int wallCount = collector.GetElementCount();

            // Сразу выделяем точный объем памяти, избегая пересоздания массивов
            ConcreteWalls = new List<Wall>(wallCount);
            NoConcreteWalls = new List<Wall>(wallCount);

            // Один-единственный проход по элементам
            foreach (Element elem in collector)
            {
                if (elem is Wall wall)
                {
                    // wall.Name обращается к Revit API, это неизбежные накладные расходы, 
                    // но цикл ниже — это самый быстрый способ фильтрации в памяти C#
                    if (wall.Name != null && wall.Name.Contains("Бетон"))
                    {
                        ConcreteWalls.Add(wall);
                    }
                    else
                    {
                        NoConcreteWalls.Add(wall);
                    }
                }
            }
        }
    }
}
