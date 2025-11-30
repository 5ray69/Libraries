using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using System.Collections.Generic;

namespace Libraries.ElectricsLib.GroupService
{
    public class GroupMaxdU
    {
        private readonly CircuitMetrics _circuitMetrics;

        public GroupMaxdU(Document doc, ErrorModel errorModel)
        {
            _circuitMetrics = new CircuitMetrics(doc, errorModel);
        }


        private Dictionary<string, (double maxDU, string pathName, List<ElectricalSystem> circuits)>
    CalculateGroupMaxDU(Dictionary<string, Dictionary<string, List<ElectricalSystem>>> groupAllPaths)
        {
            Dictionary<string, (double, string, List<ElectricalSystem>)> result = [];

            //перебираем каждую группу
            foreach (KeyValuePair<string, Dictionary<string, List<ElectricalSystem>>> group in groupAllPaths)
            {
                string groupName = group.Key;
                //все пути внутри группы
                Dictionary<string, List<ElectricalSystem>> paths = group.Value;

                double maxDU = double.MinValue;
                string maxPathName = null;
                List<ElectricalSystem> maxCircuits = null;

                //перебираем каждый путь внутри группы
                foreach (KeyValuePair<string, List<ElectricalSystem>> path in paths)
                {
                    //сумма dU всех цепей внутри пути
                    double sumDU = 0.0;
                    foreach (ElectricalSystem circuit in path.Value)
                        sumDU += _circuitMetrics.GetDUfor220V(circuit);

                    if (sumDU > maxDU)
                    {
                        maxDU = sumDU;  //масимальное dU
                        maxPathName = path.Key;  //имя пути с масимальным dU
                        maxCircuits = path.Value;  //список цепей всего пути с масимальным dU
                    }
                }

                result[groupName] = (maxDU, maxPathName, maxCircuits);
            }

            return result;
        }



        /// <summary>
        /// Возвращает максимальный dU по каждой группе.
        /// </summary>
        /// <param name="groupAllPaths">все пути содержащиеся в группе</param>
        /// <returns>Dictionary<string, double></returns>
        public Dictionary<string, double> GetMaxDU(Dictionary<string, Dictionary<string, List<ElectricalSystem>>> groupAllPaths)
        {
            Dictionary<string, (double maxDU, string pathName, List<ElectricalSystem> circuits)> calc = CalculateGroupMaxDU(groupAllPaths);
            Dictionary<string, double> result = [];

            foreach (KeyValuePair<string, (double maxDU, string pathName, List<ElectricalSystem> circuits)> group in calc)
                result[group.Key] = group.Value.maxDU;

            return result;
        }



        /// <summary>
        /// Возвращает цепи пути с максимальным dU для каждой группы.
        /// </summary>
        /// <param name="groupAllPaths">все пути содержащиеся в группе</param>
        /// <returns>Dictionary<string, List<ElectricalSystem>></returns>
        public Dictionary<string, List<ElectricalSystem>> GetCircuitsMaxDU(Dictionary<string, Dictionary<string, List<ElectricalSystem>>> groupAllPaths)
        {
            Dictionary<string, (double maxDU, string pathName, List<ElectricalSystem> circuits)> calc = CalculateGroupMaxDU(groupAllPaths);

            Dictionary<string, List<ElectricalSystem>> result = [];

            foreach (KeyValuePair<string, (double maxDU, string pathName, List<ElectricalSystem> circuits)> group in calc)
                result[group.Key] = group.Value.circuits;

            return result;
        }
    }
}





