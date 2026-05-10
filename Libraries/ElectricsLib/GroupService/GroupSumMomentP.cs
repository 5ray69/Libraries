using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using System.Collections.Generic;

namespace Libraries.ElectricsLib.GroupService
{
    public class GroupSumMomentP
    {
        private readonly CircuitMetrics _circuitMetrics;

        public GroupSumMomentP(Document doc, ErrorModel errorModel)
        {
            _circuitMetrics = new CircuitMetrics(doc, errorModel);
        }


        /// <summary>
        /// <para> Возвращает сумму моментов активной мощности (кВт·м) </para>
        /// <para> для списка цепей каждой группы.</para>
        /// <para> Создан для нахождения момента пути из цепей с максимальным dU</para>
        /// </summary>
        /// <param name="circuitsMaxdU">словарь со списком цепей для каждой группы</param>
        /// <returns>Dictionary<string, double></returns>
        public Dictionary<string, double> GetSumM(Dictionary<string, List<ElectricalSystem>> circuitsMaxdU)
        {
            Dictionary<string, double> result = [];

            //перебираем каждую группу
            foreach (KeyValuePair<string, List<ElectricalSystem>> kvp in circuitsMaxdU)
            {

                double sumM = 0.0;

                //суммируем момент каждой цепи в группе
                foreach (ElectricalSystem circuit in kvp.Value)
                    sumM += _circuitMetrics.GetKilowattOnMeter(circuit);

                result[kvp.Key] = sumM;
            }

            return result;
        }
    }
}
