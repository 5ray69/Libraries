using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// Все цепи проекта
    /// </summary>
    public class AllCircuits
    {
        private readonly Document _doc;
        private readonly ICollection<ElectricalSystem> _electricalSystems;


        /// <summary>
        /// Возвращает все цепи проекта
        /// </summary>
        /// <param name="doc"></param>
        public AllCircuits(Document doc)
        {
            _doc = doc;

            _electricalSystems = [];

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                                                    .OfCategory(BuiltInCategory.OST_ElectricalCircuit);

            foreach (Element elem in collector)
            {
                if (elem is ElectricalSystem es)
                {
                    _electricalSystems.Add(es);
                }
            }
        }


        /// <summary>
        /// возвращает все цепи проекта
        /// </summary>
        /// <returns></returns>
        public ICollection<ElectricalSystem> GetAllCircuits()
        {
            return _electricalSystems;
        }


        /// <summary>
        /// <para> возвращает уникальные имена групп на указанном</para>
        /// <para> уровне, получаемые из параметра "БУДОВА_Группа"</para>
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<string> GetAllGroupsOnLevel(Level level)
        {
            LevelAnyObject levelAnyObject = new(_doc);

            List<string> uniqGroups = [];

            foreach (ElectricalSystem electricalSystem in _electricalSystems)
            {
                ElementId idLevelCircuit = levelAnyObject.GetLevel(electricalSystem).Id;
                if (level.Id == idLevelCircuit)
                {
                    string grp = electricalSystem.LookupParameter("БУДОВА_Группа").AsString();
                    if (!uniqGroups.Contains(grp))
                    {
                        uniqGroups.Add(grp);
                    }
                }
            }
            return uniqGroups;
        }
    }
}
