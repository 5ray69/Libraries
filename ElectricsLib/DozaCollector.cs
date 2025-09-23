using Autodesk.Revit.DB;
using System.Linq;

namespace Libraries.ElectricsLib
{
    public class DozaCollector
    {
        private readonly Document _doc;

        public DozaCollector(Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Собирает группы доз по выбранному семейству.
        /// </summary>
        public DozaGroups Collect(ElementId elementIdFamily)
        {
            // Собираем FamilyInstance на активном виде
            var familyInstances = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(fi => fi.Symbol?.Family?.Id == elementIdFamily)
                .ToList();

            // Создаём коллекцию DozaGroups
            var groups = new DozaGroups();

            // Группируем по имени до точки
            foreach (var g in familyInstances.GroupBy(fi =>
            {
                string name = fi.Name ?? string.Empty;
                int dotIndex = name.LastIndexOf('.');
                return dotIndex > 0 ? name.Substring(0, dotIndex) : name;
            }))
            {
                var dozaGroup = new DozaGroup(_doc);
                dozaGroup.Initialize(g.Key, g);
                groups.Add(dozaGroup);
            }

            return groups;
        }
    }
}
