using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using System.Collections.Generic;

namespace CalculationGroups.MyDll.Work
{
    /// <summary>
    /// все полные пути каждой группы, состоящие из цепей
    /// </summary>
    /// <param name="document"></param>
    /// <param name="errorModel"></param>
    public class AllFullPaths(Document document, ErrorModel errorModel)
    {
        private readonly Document _doc = document;
        private readonly ErrorModel _errorModel = errorModel;
        private readonly FullPath _fullPath = new(document, errorModel);


        /// <summary>
        /// <para>Извлекает полный путь от конечной цепи до выбранной панели.</para>
        /// <para>На выходе словарь c словарями из всех имеющихся путей у группы, в каждом пути все цепи.</para>
        /// <para>Цепи дублирующихся групп здесь уже должны быть устранены после GetHeadPanel</para>
        /// </summary>
        /// <param name="headPanelOfGroups"> словарь id головной панели каждой группы</param>
        /// <param name="endCircuitGroups"> словарь конечной цепи каждой группы</param>
        /// <returns>словарь словарей - каждая группа содержит словарь всех её полных путей цепей</returns>
        public Dictionary<string, Dictionary<string, List<ElectricalSystem>>> Get(
                            Dictionary<string, ElementId> headPanelOfGroups,
                            Dictionary<string, List<ElectricalSystem>> endCircuitGroups)
        {

            Dictionary<string, Dictionary<string, List<ElectricalSystem>>> AllFullPaths = new();


            foreach (KeyValuePair<string, List<ElectricalSystem>> kvp in endCircuitGroups)
            {
                string groupName = kvp.Key;
                List<ElectricalSystem> endCircuits = kvp.Value;

                //Id головной панели группы
                headPanelOfGroups.TryGetValue(groupName, out ElementId headPanelId);

                Dictionary<string, List<ElectricalSystem>> dictOnePath = [];
                int i = 0;

                foreach (ElectricalSystem endCircuit in endCircuits)
                {
                    i++;
                    string pathKey = "path" + i;

                    //все цепи одного пути группы
                    List<ElectricalSystem> onePathCircuits = _fullPath.GetAllCircuits(headPanelId, endCircuit);

                    dictOnePath[pathKey] = onePathCircuits;
                }

                //все пути цепей одной группы
                AllFullPaths[groupName] = dictOnePath;
            }

            return AllFullPaths;
        }
    }
}
