using Autodesk.Revit.DB;

namespace Libraries.LevelsLib
{
    /// <summary>
    /// Вспомогательная структура для хранения уровня и его предрасчитанной отметки
    /// </summary>
    /// <param name="Level"></param>
    /// <param name="RoundedProjectElevation"></param>
    public readonly record struct CachedLevelInfo(Level Level, double RoundedProjectElevation);


    /// <summary>
    /// Кэш уровней
    /// </summary>
    public class LevelCache
    {
        /// <summary>
        /// <para> Константа точности округления отметок в футах (6 знаков ≈ 0.0003 мм), </para>
        /// <para> используется и в LevelCache, и в ElevationDouble для идентичного округления </para>
        /// </summary>
        public const int ElevationPrecisionDigits = 6;

        private static readonly Dictionary<int, List<CachedLevelInfo>> _cachedLevelInfosByDoc = [];
        private static readonly Dictionary<int, List<Level>> _cachedLevelsByDoc = [];
        private static readonly Dictionary<int, Dictionary<string, ElementId>> _levelDictionaryByDoc = [];
        private static readonly Dictionary<int, Dictionary<double, ElementId>> _elevationDictByDoc = [];

        
        /// <summary>
        /// Получает кэшированную коллекцию уровней с предрасчитанными отметками (для быстрого поиска в ElevationDouble)
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<CachedLevelInfo> GetSortedLevelInfos(Document doc)
        {
            // Храним кэш отдельно для каждого открытого документа (ключ — doc.GetHashCode())
            // Привязываем кэш к конкретному Document (по его уникальному HashCode в сессии Revit)
            int docKey = doc.GetHashCode();

            if (!_cachedLevelInfosByDoc.TryGetValue(docKey, out List<CachedLevelInfo> cachedInfos)
                || cachedInfos.Count == 0
                || !cachedInfos[0].Level.IsValidObject)
            {
                FilteredElementCollector levelCollector = new FilteredElementCollector(doc)
                                                                        .OfClass(typeof(Level));

                List<CachedLevelInfo> infos = [];
                foreach (Element element in levelCollector)
                {
                    if (element is Level level)
                    {
                        double roundedElevation = Math.Round(
                                                            level.ProjectElevation,
                                                            ElevationPrecisionDigits,
                                                            MidpointRounding.AwayFromZero);

                        infos.Add(new CachedLevelInfo(level, roundedElevation));
                    }
                }

                // Сортируем уровни по возрастанию высотной отметки
                infos.Sort((a, b) => a.RoundedProjectElevation.CompareTo(b.RoundedProjectElevation));

                cachedInfos = infos;
                _cachedLevelInfosByDoc[docKey] = cachedInfos;
                _cachedLevelsByDoc.Remove(docKey); // сбрасываем производный список, если уровни обновились
            }

            return cachedInfos;
        }

        
        /// <summary>
        /// Получает кэшированную коллекцию уровней или создаёт её при первом запросе
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<Level> GetSortedLevels(Document doc)
        {
            int docKey = doc.GetHashCode();

            if (!_cachedLevelsByDoc.TryGetValue(docKey, out List<Level> cachedLevels)
                || cachedLevels.Count == 0
                || !cachedLevels[0].IsValidObject)
            {
                List<CachedLevelInfo> infos = GetSortedLevelInfos(doc);
                cachedLevels = new List<Level>(infos.Count);

                for (int i = 0; i < infos.Count; i++)
                {
                    cachedLevels.Add(infos[i].Level);
                }

                _cachedLevelsByDoc[docKey] = cachedLevels;
            }

            return cachedLevels;
        }

        
        /// <summary>
        /// Возвращает список имен уровней из кэша
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> GetLevelNames(Document doc)
        {
            List<Level> levels = GetSortedLevels(doc);
            List<string> names = new(levels.Count);

            for (int i = 0; i < levels.Count; i++)
            {
                names.Add(levels[i].Name);
            }

            return names;
        }
        
        
        /// <summary>
        /// Возвращает словарь, где ключ — имя уровня, а значение — его ElementId.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<string, ElementId> GetLevelDictionary(Document doc)
        {
            int docKey = doc.GetHashCode();

            if (!_levelDictionaryByDoc.TryGetValue(docKey, out Dictionary<string, ElementId> levelDictionary))
            {
                List<Level> levels = GetSortedLevels(doc);
                levelDictionary = new Dictionary<string, ElementId>(levels.Count);

                for (int i = 0; i < levels.Count; i++)
                {
                    Level level = levels[i];
                    levelDictionary[level.Name] = level.Id;
                }

                _levelDictionaryByDoc[docKey] = levelDictionary;
            }

            return levelDictionary;
        }


        /// <summary>
        /// <para> Получает словарь, где ключ — округлённая до двух знаков высотная отметка уровня, </para>
        /// <para> а значение — ElementId уровня. </para>
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<double, ElementId> GetElevationLevelDictionary(Document doc)
        {
            // Получаем отсортированные уровни
            List<Level> levels = GetSortedLevels(doc);
            int docKey = doc.GetHashCode();

            // Проверяем, создан ли уже словарь для данного документа в кэше.
            // В Revit API весь код выполняется в одном главном потоке, поэтому lock не требуется.
            if (!_elevationDictByDoc.TryGetValue(docKey, out Dictionary<double, ElementId> elevationDict))
            {
                // Создаём словарь с округлением Elevation
                // Если в модели окажутся несколько уровней с одинаковой округлённой отметкой
                // (например, 3.501 и 3.499 оба могут дать 3.50), то проверка !ContainsKey исключит дубликаты,
                // и словарь возьмёт первый попавшийся уровень для этой высоты.
                elevationDict = new Dictionary<double, ElementId>(levels.Count);

                for (int i = 0; i < levels.Count; i++)
                {
                    Level level = levels[i];
                    double roundedElevation = Math.Round(level.Elevation, 2);

                    if (!elevationDict.ContainsKey(roundedElevation))
                    {
                        elevationDict.Add(roundedElevation, level.Id);
                    }
                }

                _elevationDictByDoc[docKey] = elevationDict;
            }

            return elevationDict;
        }


        /// <summary>
        /// <para> Проверяет, все ли имена уровней из связанного файла есть в именах уровней основного файла. </para>
        /// <para> Возвращает имена уровней из связанного файла, которых нет в уровнях основного файла. </para>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="linkDoc"></param>
        /// <returns></returns>
        public static List<string> GetMissingLevels(Document doc, Document linkDoc)
        {
            Dictionary<string, ElementId> levelDict = GetLevelDictionary(doc); // словарь уровней из основного файла

            FilteredElementCollector levelCollector = new FilteredElementCollector(linkDoc)
                                                                    .OfClass(typeof(Level)); // уровни из связанного файла

            List<string> noLevelNames = new List<string>(levelCollector.Count());
            foreach (Element element in levelCollector)
            {
                if (element is Level linkedLevel)
                {
                    string levelName = linkedLevel.Name;
                    if (!levelDict.ContainsKey(levelName)) // проверяем, есть ли имя уровня
                    {
                        noLevelNames.Add(levelName);
                    }
                }
            }

            return noLevelNames;
        }


        /// <summary>
        /// <para> Проверяет, все ли отметки уровней из связанного файла есть в отметках уровней основного файла. </para>
        /// <para> Возвращает имена уровней из связанного файла, отметки которых отсутствуют в отметках уровней основного файла. </para>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="linkDoc"></param>
        /// <returns></returns>
        public static List <string> GetMissingElevationLevels(Document doc, Document linkDoc)
        {
            Dictionary<double, ElementId> levelElevationDict = GetElevationLevelDictionary(doc); // словарь с высотными отметками уровней из основного файла

            FilteredElementCollector levelCollector = new FilteredElementCollector(linkDoc)
                                                                            .OfClass(typeof(Level)); // уровни из связанного файла

            List<string> noLevelNamesElevations = new List<string>(levelCollector.Count());
            foreach (Element element in levelCollector)
            {
                if (element is Level linkedLevel)
                {
                    double levelRoundElevation = Math.Round(linkedLevel.Elevation, 2);
                    if (!levelElevationDict.ContainsKey(levelRoundElevation)) // проверяем, есть ли отметка уровня
                    {
                        noLevelNamesElevations.Add(linkedLevel.Name);
                    }
                }
            }

            return noLevelNamesElevations;
        }

        
        /// <summary>
        /// Метод очистки/сброса кэша, если документ изменится после вставки/удаления уровней.
        /// </summary>
        /// <param name="doc"></param>
        public static void ResetCache(Document doc = null)
        {
            if (doc is null)
            {
                _cachedLevelInfosByDoc.Clear();
                _cachedLevelsByDoc.Clear();
                _levelDictionaryByDoc.Clear();
                _elevationDictByDoc.Clear();
            }
            else
            {
                int docKey = doc.GetHashCode();
                _cachedLevelInfosByDoc.Remove(docKey);
                _cachedLevelsByDoc.Remove(docKey);
                _levelDictionaryByDoc.Remove(docKey);
                _elevationDictByDoc.Remove(docKey);
            }
        }
    }
}


//// Получаем уровни из кэша вместе с уже округлёнными отметками
//List<CachedLevelInfo> levels = LevelCache.GetSortedLevelInfos(doc);
//new OutList().ShowLevels(levels, "Список уровней с высотными отметками");

//// Получаем уровни из кэша
//List<Level> levs = LevelCache.GetSortedLevels(doc);
//new OutList().ShowLevs(levs, "Список уровней");

//// Получаем уровни из кэша
//List<string> ls = LevelCache.GetLevelNames(doc);
//new OutList().ShowString(ls, "Список имен уровней");

//// Получаем словарь, где ключ — имя уровня, а значение — его ElementId
//Dictionary<string, ElementId> ldict = LevelCache.GetLevelDictionary(doc);
//new OutputDictionary().ShowStringElementId(ldict, "словарь, где ключ — имя уровня, а значение — его ElementId");

//// Получаем словарь, где ключ — округлённая до двух знаков высотная отметка уровня, а значение — его ElementId
//Dictionary<double, ElementId> ldictEl = LevelCache.GetElevationLevelDictionary(doc);
//new OutputDictionary().ShowDoubleElementId(ldictEl, "словарь, где ключ — округлённая высотная отметка, а значение — его ElementId");

//// Проверяет, все ли имена уровней из связанного файла есть в уровнях основного файла.
//List<string> missingLevels = LevelCache.GetMissingLevels(doc, linkDoc);
//new OutList().ShowString(missingLevels, "Список имен отсутствующих уровней в основном файле");

//// Проверяет, все ли отметки уровней из связанного файла есть в отметках уровней основного файла.
//List<string> missingElevationLevels = LevelCache.GetMissingElevationLevels(doc, linkDoc);
//new OutList().ShowString(missingElevationLevels, "Список имен уровней с отсутствующими отметками в основном файле");
