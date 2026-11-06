using Autodesk.Revit.DB;

namespace Libraries.Collectors
{
    /// <summary>
    /// контейнер отверсий по именам уровней
    /// </summary>
    /// <param name="levelName"></param>
    public class LevelHolesGroup(string levelName)
    {
        /// <summary>
        /// имя уровня
        /// </summary>
        public string LevelName { get; } = levelName;
        
        /// <summary>
        /// HashSet заглушек/отверстий
        /// </summary>
        public HashSet<FamilyInstance> Holes { get; } = [];
    }
}
