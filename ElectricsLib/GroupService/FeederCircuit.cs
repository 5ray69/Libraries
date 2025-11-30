using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using CalculationGroups.MyDll.UserWarningCalculationGroups;
using Libraries.ErrorModelLib;
using System.Collections.Generic;
using System.Linq;

namespace CalculationGroups.MyDll.Work
{
    /// <summary>
    /// цепь, которая питает панель
    /// </summary>
    /// <param name="document"></param>
    /// <param name="errorModel"></param>
    public class FeederCircuit(Document document, ErrorModel errorModel)
    {
        private readonly Document _doc = document;
        private readonly ErrorModel _errorModel = errorModel;



        /// <summary>
        /// возвращает цепь, которая питает панель
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public ElectricalSystem Get(FamilyInstance panel)
        {

            MEPModel mepModel = panel.MEPModel;

            //если семейство не является MEP оборудованием
            if (mepModel == null)
            {
                //уведомляем пользователя и завершаем код
                _errorModel.UserWarning(new FamilyIsNotMEP().MessageForUser(_doc, panel));
            }

            // все цепи, включая цепь питания панели
            ISet<ElectricalSystem> circuitsAll = mepModel.GetElectricalSystems();

            // Только цепи нагрузок, без цепи питания панели
            List<ElementId> circuitsLoads = mepModel.GetAssignedElectricalSystems().Select(es => es.Id).ToList();

            // питающая цепь
            List<ElectricalSystem> circuitFeeder = [];

            // из всех цепей вычли цепи нагрузок и получили питающую цепь
            foreach (ElectricalSystem es in circuitsAll)
            {
                ElementId esId = es.Id;
                //если в нагрузках нет цепи, то добавляем ее в circuitFeeder
                if (!circuitsLoads.Contains(esId))
                {
                    circuitFeeder.Add(es);
                }
            }

            //если питацающей цепи нет, список пуст
            if (circuitFeeder.Count == 0)
            {
                return null;
            }

            return circuitFeeder[0];

        }
    }
}



//ElectricalSystem circuit = null;
//MEPModel mepModel = panel.MEPModel;

////если семейство не является MEP оборудованием
//if (mepModel == null)
//{
//    //уведомляем пользователя и завершаем код
//    _errorModel.UserWarning(new FamilyIsNotMEP().MessageForUser(_doc, panel));
//}


//ConnectorSet connectors = mepModel.ConnectorManager.Connectors;

//foreach (Connector connector in connectors)
//{
//    if (connector.Domain == Domain.DomainElectrical)  //коннектор питающей цепи, у отходящей цепи Domain = DomainUndefined
//    {
//        //если есть питающая цепь, то в AllRefs виртуальный коннектор цепи
//        ConnectorSet allRefs = connector.AllRefs;

//        if (allRefs.Size > 0)
//        {
//            foreach (Connector conRef in allRefs)
//            {
//                ElectricalSystem electricalSystem = conRef.Owner as ElectricalSystem;
//                circuit = electricalSystem;
//                break; // выход из цикла после первой итерации
//            }
//        }

//        //если питацающей цепи нет, список пуст, значит это и есть головная панель
//        if (allRefs.Size == 0)
//        {
//            return null;
//        }
//    }
//}
//return circuit;