using System.Collections;
using System.Collections.Generic;

namespace Libraries.ElectricsLib
{
    public class DozaGroups : IEnumerable<DozaGroup>
    {
        private readonly List<DozaGroup> _groups;

        public DozaGroups()
        {
            _groups = new List<DozaGroup>();
        }

        public void Add(DozaGroup group) => _groups.Add(group);

        public IEnumerator<DozaGroup> GetEnumerator() => _groups.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
