using Autodesk.Revit.DB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CreateRisersShield.MyDll
{
    /// <summary>
    /// <para>Класс, представляющий коллекцию групп коннекторов</para>
    /// <para>Ключ — имя группы (string), значение — список коннекторов (List<Connector>)</para>
    /// </summary>
    public class GroupsOfConnectors : IEnumerable<KeyValuePair<string, List<Connector>>>
    {
        private readonly Dictionary<string, List<Connector>> _groups = [];
        /// <summary>
        /// Проверяет, содержит ли коллекция группу с указанным именем.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool ContainsGroup(string groupName)
        {
            return _groups.ContainsKey(groupName);
        }


        /// <summary>
        /// Возвращает список всех групп, содержащихся в коллекции.
        /// </summary>
        /// <returns></returns>
        public List<string> GetGroupNames()
        {
            return _groups.Keys.ToList();
        }


        /// <summary>
        /// <para>Добавляет один коннектор в существующую группу (или создаёт новую).</para>
        /// <para>Если группа не существует — создаёт новую.</para>
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="connector"></param>
        public void AddConnector(string groupName, Connector connector)
        {
            if (!_groups.ContainsKey(groupName))
                _groups[groupName] = new List<Connector>();

            _groups[groupName].Add(connector);
        }


        /// <summary>
        /// <para>Добавляет несколько коннекторов в существующую группу (или создаёт новую).</para>
        /// <para>Если группа не существует — создаёт новую.</para>
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="connectors"></param>
        public void AddConnectors(string groupName, IEnumerable<Connector> connectors)
        {
            if (connectors == null)
                return;

            if (!_groups.ContainsKey(groupName))
                _groups[groupName] = new List<Connector>();

            _groups[groupName].AddRange(connectors);
        }


        /// <summary>
        /// Добавляет новую группу и список коннекторов, если такой группы ещё нет.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="connectors"></param>
        public void AddGroup(string groupName, List<Connector> connectors)
        {
            if (!_groups.ContainsKey(groupName))
                _groups[groupName] = connectors;
        }


        /// <summary>
        /// Добавляет группу или объединяет с существующей.
        /// </summary>
        public void AddOrMergeGroup(string groupName, IEnumerable<Connector> connectors)
        {
            if (connectors == null)
                return;

            if (_groups.TryGetValue(groupName, out var existingConnectors))
                existingConnectors.AddRange(connectors);  // добавляем в существующую группу
            else
                _groups[groupName] = new List<Connector>(connectors);  // создаём новую группу
        }


        /// <summary>
        /// <para> Извлекает список коннекторов по имени группы. </para>
        /// <para> Если группы нет, возвращает пустой список. </para>
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public List<Connector> GetConnectors(string groupName)
        {
            if (_groups.TryGetValue(groupName, out var connectors))
                return connectors;

            return new List<Connector>();
        }


        /// <summary>
        /// Возвращает первый коннектор из группы (или null, если нет группы или элементов).
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Connector GetFirstConnector(string groupName)
        {
            if (_groups.TryGetValue(groupName, out var connectors) && connectors.Count > 0)
                return connectors.First();

            return null;
        }


        /// <summary>
        /// Возвращает последний коннектор из группы (или null, если нет группы или элементов).
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Connector GetLastConnector(string groupName)
        {
            if (_groups.TryGetValue(groupName, out var connectors) && connectors.Count > 0)
                return connectors.Last();

            return null;
        }


        /// <summary>
        /// Реализация IEnumerable для перебора в foreach.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, List<Connector>>> GetEnumerator()
        {
            return _groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}









//****************************************************************************************************
//foreach (var group in groups)
//{
//    string name = group.Key;
//    List<Connector> connectors = group.Value;
//}
//Для этого реализовали интерфейс IEnumerable<KeyValuePair<string, List<Connector>>>.
//****************************************************************************************************


//****************************************************************************************************
//var groups = new GroupsOfConnectors();

//// добавляем или объединяем
//groups.AddOrMergeGroup("ЩО-1", list1);
//groups.AddOrMergeGroup("ЩО-1", list2);
//groups.AddOrMergeGroup("ЩО-2", list3);

//// перебор, как по обычному словарю
//foreach (var group in groups)
//{
//    string name = group.Key;
//    List<Connector> connectors = group.Value;

//    TaskDialog.Show("Инфо", $"Группа {name} содержит {connectors.Count} коннекторов");
//}
//****************************************************************************************************