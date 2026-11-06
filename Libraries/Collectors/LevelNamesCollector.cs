using Autodesk.Revit.DB;

namespace Libraries.Collectors
{
    /// <summary>
    /// Коллектор уровней
    /// </summary>
    public class LevelNamesCollector
    {
        /// <summary>
        /// имена уровней
        /// </summary>
        public List<string> Names { get; }

        /// <summary>
        /// словарь ключ - имя уровня, значение - высотная отметка long
        /// </summary>
        public Dictionary<string, long> NamesElevations { get; }

        /// <summary>
        /// словарь ключ -  высотная отметка long, значение - имя уровня
        /// </summary>
        public Dictionary<long, string> ElevationsNames { get; }


        /// <summary>
        /// наполняет свойства класса
        /// </summary>
        /// <param name="doc"></param>
        public LevelNamesCollector(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));

            NamesElevations = new Dictionary<string, long>();
            ElevationsNames = new Dictionary<long, string>();

            // Быстро получаем точное количество элементов коллектора на стороне C++ ядра Revit
            int elCount = collector.GetElementCount();

            // Сразу выделяем точный объем памяти,
            // избегая пересоздания массивов и для списка и для словарей
            List<string> tempNames = new List<string>(elCount);
            NamesElevations = new Dictionary<string, long>(elCount);  // инициализируем словарь
            ElevationsNames = new Dictionary<long, string>(elCount);  // инициализируем словарь

            foreach (Element elem in collector)
            {
                if (elem is Level level)
                {
                    string name = level.Name;
                    tempNames.Add(name);

                    // округление во избежание погрешностей float/double
                    long roundElev = (long)Math.Round(level.Elevation * 100);

                    // Записываем вычисленное значение (если в проекте возможны дубликаты имен уровней,
                    // индексер перезапишет значение или можно использовать TryAdd / проверку)
                    NamesElevations[name] = roundElev;
                    ElevationsNames[roundElev] = name;
                }
            }
            // результат сортировки в свойство Names
            MySort mySort = new();
            Names = mySort.LevelNames(tempNames);
        }
    }
}


//В C# для свойств только с геттером ({ get; }) есть два законных места для инициализации:
//Прямо при объявлении (например, public List<string> Names { get; } = [];).
//Внутри конструктора этого класса.