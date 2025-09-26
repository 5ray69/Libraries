using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.LevelsLib
{
    public class Levels
    {
        private List<Level> _cachedLevels;
        private Dictionary<string, ElementId> _levelDictionary;
        private Dictionary<double, ElementId> _elevationDict;

        private readonly Document _doc;

        public Levels(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        /// <summary>
        /// Получает кэшированную коллекцию уровней отсортированную в порядке возрастания высотных отметок
        /// </summary>
        public List<Level> GetSortedLevels()
        {
            if (_cachedLevels == null)
            {
                _cachedLevels = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Level))
                    .OfType<Level>()
                    .OrderBy(level => level.Elevation)
                    .ToList();
            }
            return _cachedLevels;
        }

        /// <summary>
        /// Возвращает список имён уровней из кэша.
        /// </summary>
        public List<string> GetLevelNames()
        {
            return GetSortedLevels()
                .Select(level => level.Name)
                .ToList();
        }

        /// <summary>
        /// Получает словарь, где ключ — имя уровня, а значение — его ElementId.
        /// </summary>
        public Dictionary<string, ElementId> GetLevelDictionary()
        {
            if (_levelDictionary == null)
            {
                //  _levelDictionary ?? это все равно что if (_levelDictionary == null)
                _levelDictionary ??= GetSortedLevels()
                    .ToDictionary(level => level.Name, level => level.Id);
            }
            return _levelDictionary;
        }


        /// <summary>
        /// Проверяет, все ли имена уровней из связанного файла есть в уровнях основного файла.
        /// </summary>
        /// <param name="linkDoc"></param>
        /// <returns></returns>
        public ICollection<string> GetMissingNamesLevels(Document linkDoc)
        {
            var levelDict = GetLevelDictionary(); // словарь уровней из основного файла

            IEnumerable<Level> linkedLevels = new FilteredElementCollector(linkDoc)
                .OfClass(typeof(Level))
                .OfType<Level>();

            List<string> noLevelNames = new();
            foreach (Level linkedLevel in linkedLevels)
            {
                if (!levelDict.ContainsKey(linkedLevel.Name)) // проверяем, есть ли имя уровня
                {
                    noLevelNames.Add(linkedLevel.Name);
                }
            }
            return noLevelNames;
        }


        /// <summary>
        /// Проверяет, все ли уровни из связанного файла присутствуют по отметкам Elevation.
        /// </summary>
        /// <param name="linkDoc"></param>
        /// <returns></returns>
        public ICollection<string> GetMissingElevationLevels(Document linkDoc)
        {
            var elevationDict = GetElevationLevelDictionary(); // словарь с высотными отметками уровней из основного файла

            IEnumerable<Level> linkedLevels = new FilteredElementCollector(linkDoc) // уровни из связанного файла
                .OfClass(typeof(Level))
                .OfType<Level>();

            List<string> noLevelNamesElevations = new();
            foreach (Level linkedLevel in linkedLevels)
            {
                double roundedElev = Math.Round(linkedLevel.Elevation, 2); // проверяем, есть ли отметка уровня
                if (!elevationDict.ContainsKey(roundedElev))
                {
                    noLevelNamesElevations.Add(linkedLevel.Name);
                }
            }
            return noLevelNamesElevations;
        }


        /// <summary>
        /// Получает словарь, где ключ — округлённая до двух знаков высотная отметка уровня,
        /// а значение — ElementId уровня.
        /// </summary>
        public Dictionary<double, ElementId> GetElevationLevelDictionary()
        {
            if (_elevationDict == null)
            {
                // Создаём словарь с округлением Elevation
                // Если в модели окажутся несколько уровней с одинаковой округлённой отметкой
                // (например, 3.501 и 3.499 оба могут дать 3.50), то метод GroupBy исключит дубликаты,
                // и ToDictionary возьмёт первый попавшийся уровень для этой высоты.
                _elevationDict = GetSortedLevels()
                    .GroupBy(level => Math.Round(level.Elevation, 2))  // группируем по округлённой Elevation
                    .ToDictionary(group => group.Key, group => group.First().Id);  // если есть дубликаты, берём первый
            }
            return _elevationDict;
        }


        /// <summary>
        /// Находит уровень по высотной отметке (сравнение по округлённому значению до 2 знаков).
        /// Возвращает null, если уровень не найден.
        /// </summary>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public Level GetLevelByElevation(double elevation)
        {
            double roundedElevation = Math.Round(elevation, 2);

            // Берём кэшированные уровни
            List<Level> levels = GetSortedLevels();

            // Ищем первый уровень с совпадающей округлённой отметкой
            return levels.FirstOrDefault(
                lvl => Math.Round(lvl.Elevation, 2) == roundedElevation
            );
        }

        /// <summary>
        /// Метод очистки/сброса кэша (например, если уровни были изменены).
        /// </summary>
        public void ResetCache()
        {
            _cachedLevels = null;
            _levelDictionary = null;
            _elevationDict = null;
        }
    }
}
