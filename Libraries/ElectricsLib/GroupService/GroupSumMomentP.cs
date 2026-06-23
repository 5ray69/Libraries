using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;

namespace Libraries.ElectricsLib.GroupService
{
    /// <summary>
    /// Суммарный момент активной мощности (кВт·м) группы
    /// </summary>
    public class GroupSumMomentP(Document doc, ErrorModel errorModel)
    {
        private readonly CircuitMetrics _circuitMetrics = new CircuitMetrics(doc, errorModel);

        /// <summary>
        /// <para> Возвращает сумму моментов активной мощности (кВт·м) </para>
        /// <para> для списка цепей каждой группы.</para>
        /// <para> Создан для нахождения момента пути из цепей с максимальным dU</para>
        /// </summary>
        /// <param name="circuitsMaxdU"></param>
        /// <returns></returns>
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
