using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.LevelsLib
{
    public class LevelCache
    {
        private static List<Level> _cachedLevels;
        private static Dictionary<string, ElementId> _levelDictionary;
        private static Dictionary<double, ElementId> _elevationDict;
        private static readonly object _lock = new();

        /// <summary>
        /// Получает кэшированную коллекцию уровней или создаёт её при первом запросе
        /// </summary>
        public static List<Level> GetSortedLevels(Document doc)
        {
            if (_cachedLevels == null)
            {
                lock (_lock) //lock (замок) гарантирует, что только один поток может выполнить критический участок кода, заключённый в блок lock
                {
                    if (_cachedLevels == null)
                    {
                        FilteredElementCollector levelCollector = new(doc);
                        _cachedLevels = levelCollector.OfClass(typeof(Level))
                                                       .OfType<Level>()
                                                       .OrderBy(level => level.Elevation)
                                                       .ToList();
                    }
                }
            }
            return _cachedLevels;
        }

        /// <summary>
        /// Возвращает список имен уровней из кэша
        /// </summary>
        public static List<string> GetLevelNames(Document doc)
        {
            return GetSortedLevels(doc).Select(level => level.Name).ToList();
        }



        /// <summary>
        /// Получает словарь, где ключ — имя уровня, а значение — его ElementId.
        /// </summary>
        public static Dictionary<string, ElementId> GetLevelDictionary(Document doc)
        {
            if (_levelDictionary == null)
            {
                lock (_lock)
                {
                    //  _levelDictionary ?? это все равно что if (_levelDictionary == null)
                    _levelDictionary ??= GetSortedLevels(doc)
                            .ToDictionary(level => level.Name, level => level.Id);
                }
            }
            return _levelDictionary;
        }


        /// <summary>
        /// Проверяет, все ли имена уровней из связанного файла есть в уровнях основного файла.
        /// </summary>
        public static ICollection<string> GetMissingLevels(Document doc, Document linkDoc)
        {
            var levelDict = GetLevelDictionary(doc); // словарь уровней из основного файла

            FilteredElementCollector levelCollector = new(linkDoc);
            IEnumerable<Level> linkedLevels = levelCollector.OfClass(typeof(Level)).OfType<Level>(); // уровни из связанного файла

            List<string> noLevelNames = new();
            foreach (Level linkedLevel in linkedLevels)
            {
                string levelName = linkedLevel.Name;
                if (!levelDict.ContainsKey(levelName)) // проверяем, есть ли имя уровня
                {
                    noLevelNames.Add(levelName);
                }
            }
            return noLevelNames;
        }




        /// <summary>
        /// Получает словарь, где ключ — округлённая до двух знаков высотная отметка уровня,
        /// а значение — ElementId уровня.
        /// </summary>
        public static Dictionary<double, ElementId> GetElevationLevelDictionary(Document doc)
        {
            // Получаем отсортированные уровни
            List<Level> levels = GetSortedLevels(doc);

            //Первая проверка(if (_elevationDict == null)):
            //Она нужна для того, чтобы не входить в lock каждый раз при вызове метода.
            //После первой инициализации словарь уже существует, и многопоточная блокировка будет не нужна — это ускоряет работу.
            if (_elevationDict == null)
            {
                lock (_lock)  //Если _elevationDict == null, мы заходим в lock, чтобы не допустить ситуацию, когда два потока одновременно начинают создавать словарь.
                {
                    //Вторая проверка(if (_elevationDict == null) внутри lock):
                    //Допустим, два потока одновременно проходят первую проверку.
                    //Первый поток заходит в lock и создаёт словарь.
                    //Второй поток после этого тоже заходит в lock, и теперь надо ещё раз проверить if (_elevationDict == null),
                    //потому что возможно, другой поток уже всё инициализировал.
                    //Если второй if пропустить — словарь будет пересоздан дважды, что приведёт к лишней работе или ошибке.
                    if (_elevationDict == null)
                    {
                        // Создаём словарь с округлением Elevation
                        // Если в модели окажутся несколько уровней с одинаковой округлённой отметкой
                        // (например, 3.501 и 3.499 оба могут дать 3.50), то метод GroupBy исключит дубликаты,
                        // и ToDictionary возьмёт первый попавшийся уровень для этой высоты.
                        _elevationDict = GetSortedLevels(doc)
                            .GroupBy(level => Math.Round(level.Elevation, 2))  // группируем по округлённой Elevation
                            .ToDictionary(group => group.Key, group => group.First().Id);  // если есть дубликаты, берём первый
                    }
                }
            }
            return _elevationDict;
        }


        /// <summary>
        /// Проверяет, все ли отметки уровней из связанного файла есть в отметках уровней основного файла.
        /// </summary>
        public static ICollection<string> GetMissingElevationLevels(Document doc, Document linkDoc)
        {
            var levelElevationDict = GetElevationLevelDictionary(doc); // словарь с высотными отметками уровней из основного файла

            FilteredElementCollector levelCollector = new(linkDoc);
            IEnumerable<Level> linkedLevels = levelCollector.OfClass(typeof(Level)).OfType<Level>(); // уровни из связанного файла

            List<string> noLevelNamesElevations = new();
            foreach (Level linkedLevel in linkedLevels)
            {
                double levelRoundElevation = Math.Round(linkedLevel.Elevation, 2);
                if (!levelElevationDict.ContainsKey(levelRoundElevation)) // проверяем, есть ли отметка уровня
                {
                    noLevelNamesElevations.Add(linkedLevel.Name);
                }
            }
            return noLevelNamesElevations;
        }


        /// <summary>
        /// Метод очистки/сброса кэша, если документ изменится после вставки/удаления уровней.
        /// </summary>
        public static void ResetCache()
        {
            lock (_lock)
            {
                _cachedLevels = null;
                _levelDictionary = null;
                _elevationDict = null;
            }
        }
    }
}