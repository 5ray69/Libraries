namespace Libraries.Collectors
{
    /// <summary>
    /// для выборки значений из словаря в соответствии с ключами, находящимися в списке
    /// </summary>
    public class ElevationLevels
    {
        /// <summary>
        /// <para> Из словаря высотных отметок уровней выбирает </para>
        /// <para> высотные отметки в соответсвии с именами уровней, имеющимися в списке </para>
        /// </summary>
        /// <param name="levelElevation"></param>
        /// <param name="userSelectedNames"></param>
        /// <returns></returns>
        public List<long> ExtractByName(Dictionary<string, long> levelElevation, List<string> userSelectedNames)
        {
            List<long> userSelectedElevations = [];

            foreach (string name in userSelectedNames)
            {
                if (levelElevation.TryGetValue(name, out long roundElev))
                {
                    userSelectedElevations.Add(roundElev);
                }
            }

            return userSelectedElevations;
        }


        /// <summary>
        /// <para> Из словаря имен уровней и их округленных высотных отметок </para>
        /// <para> выбирает по округленной высотной отметке уровня имя уровня </para>
        /// </summary>
        /// <param name="levelElevation"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public string ExtractByElevation(Dictionary<string, long> levelElevation, long elevation)
        {
            foreach (KeyValuePair<string, long> kvp in levelElevation)
            {
                if (kvp.Value != elevation) continue;
                return kvp.Key;
            }

            return null;
        }
    }
}