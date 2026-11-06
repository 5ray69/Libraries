using Autodesk.Revit.DB;

namespace Libraries.Collectors
{
    /// <summary>
    /// коллектор осей
    /// </summary>
    public class GridsCollector
    {
        /// <summary>
        /// Все оси
        /// </summary>
        public List<Grid> AllGrids { get; }

        /// <summary>
        /// наполняет свойство/список всех осей
        /// </summary>
        /// <param name="doc"></param>
        public GridsCollector(Document doc)
        {
            var collector = new FilteredElementCollector(doc).OfClass(typeof(Grid));

            // Быстро получаем точное количество стен на стороне C++ ядра Revit
            int gridCount = collector.GetElementCount();

            // Сразу выделяем точный объем памяти, избегая пересоздания массивов
            AllGrids = new List<Grid>(gridCount);

            // Один-единственный проход по элементам
            foreach (Element elem in collector)
            {
                if (elem is Grid grid)
                {
                    AllGrids.Add(grid);
                }
            }
        }
    }
}
