using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using LevelsLib;
using System.Collections.Generic;

namespace ElectricsLib
{
    public class AllCircuits
    {
        private readonly Document _doc;
        private readonly ICollection<ElectricalSystem> _electricalSystems;

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
        /// <para> возвращает уникальные имена групп на указанном
        /// <para> уровне, получаемые из параметра "БУДОВА_Группа"
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
