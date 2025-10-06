using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// сборщик доз по группам одного семейства - дозы
    /// </summary>
    public class DozaCollector
    {
        private readonly Document _doc;

        public DozaCollector(Document doc)
        {
            _doc = doc;
        }


        /// <summary>
        /// <para>Группирует семейства по имени до точки.</para>
        /// <para>Группируются семейства указанного ElementId.</para>
        /// <para>Семейства выбираются на виде.</para>
        /// </summary>
        /// <param name="elementIdFamily"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public DozaGroups Collect(ElementId elementIdFamily, ElementId viewId)
        {
            // Собираем FamilyInstance на активном виде
            var familyInstances = new FilteredElementCollector(_doc, viewId)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(fi => fi.Symbol?.Family?.Id == elementIdFamily)
                .ToList();

            return GroupByName(familyInstances);
        }


        /// <summary>
        /// <para>Группирует семейства по имени до точки.</para>
        /// <para>Группируются семейства указанных в коллекции ElementId.</para>
        /// <para>Семейства выбираются на виде.</para>
        /// </summary>
        /// <param name="familyIds"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public DozaGroups Collect(ICollection<ElementId> familyIds, ElementId viewId)
        {
            if (familyIds == null || familyIds.Count == 0)
                return new DozaGroups();

            var familyInstances = new FilteredElementCollector(_doc, viewId)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(fi => fi.Symbol?.Family != null && familyIds.Contains(fi.Symbol.Family.Id))
                .ToList();

            return GroupByName(familyInstances);
        }


        /// <summary>
        /// Общая логика группировки по имени до точки.
        /// </summary>
        private DozaGroups GroupByName(IEnumerable<FamilyInstance> familyInstances)
        {
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
