using Autodesk.Revit.DB;
using Libraries.LevelsLib;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib
{
    public class DozaGroup
    {
        private readonly Document _doc;
        public string Name
        {
            get; private set;
        }
        public List<FamilyInstance> Elements
        {
            get; private set;
        }

        public DozaGroup(Document doc)
        {
            _doc = doc;
            Name = string.Empty;
            Elements = new List<FamilyInstance>();
        }

        /// <summary>
        /// Инициализация группы данными.
        /// </summary>
        public void Initialize(string name, IEnumerable<FamilyInstance> elements)
        {
            LevelAnyObject levelAnyObject = new(_doc);
            Name = name;
            Elements = elements
                .OrderByDescending(fi => levelAnyObject.GetLevel(fi) != null ? levelAnyObject.GetLevel(fi).Elevation : double.MinValue)
                .ToList();
        }

        public FamilyInstance GetTopElement() => Elements.FirstOrDefault();
        public FamilyInstance GetBottomElement() => Elements.LastOrDefault();
    }
}
