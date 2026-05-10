using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Libraries.ElectricsLib
{
    public class ConnectorExtractor
    {
        /// <summary>
        /// Возвращает коннекторы любого объекта электрики
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<Connector> GetConnectors(Element element)
        {
            if (element == null)
                yield break;

            switch (element)
            {
                case FamilyInstance familyInstance:
                    foreach (var c in GetFamilyInstanceConnectors(familyInstance))
                        yield return c;
                    break;

                case MEPCurve mepCurve:
                    foreach (var c in GetMEPCurveConnectors(mepCurve))
                        yield return c;
                    break;

                default:
                    yield break;
            }
        }


        /// <summary>
        /// Коннекторы FamilyInstance (Внимание! это и светильники, и розетки и т.д.)
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        private IEnumerable<Connector> GetFamilyInstanceConnectors(FamilyInstance familyInstance)
        {
            var mepModel = familyInstance.MEPModel;
            if (mepModel?.ConnectorManager == null)
                yield break;

            foreach (Connector connector in mepModel.ConnectorManager.Connectors)
                yield return connector;
        }

        /// <summary>
        /// Коннекторы для элементов с родительским классом MEPCurve, типов: Conduit, CableTray, Duct, Pipe и т.д.
        /// </summary>
        /// <param name="mepCurve"></param>
        /// <returns></returns>
        private IEnumerable<Connector> GetMEPCurveConnectors(MEPCurve mepCurve)
        {
            var manager = mepCurve.ConnectorManager;
            if (manager == null)
                yield break;

            foreach (Connector connector in manager.Connectors)
                yield return connector;
        }
    }
}
