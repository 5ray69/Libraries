using System.Collections;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// все группы доз одного семейства
    /// </summary>
    public class DozaGroups : IEnumerable<DozaGroup>
    {
        private readonly List<DozaGroup> _groups;

        /// <summary>
        /// дозы/соединительные коробки одной группы
        /// </summary>
        public DozaGroups()
        {
            _groups = new List<DozaGroup>();
        }


        /// <summary>
        /// добавить группу доз/соединительных коробок
        /// </summary>
        /// <param name="group"></param>
        public void Add(DozaGroup group) => _groups.Add(group);


        /// <summary>
        /// получить перечислитель для групп доз/соединительных коробок
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DozaGroup> GetEnumerator() => _groups.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
