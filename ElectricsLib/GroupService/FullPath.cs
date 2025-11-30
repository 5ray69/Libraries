using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using CalculationGroups.MyDll.UserWarningCalculationGroups;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using System.Collections.Generic;
using System.Linq;

namespace CalculationGroups.MyDll.Work
{
    public class FullPath(Document document, ErrorModel errorModel)
    {
        private readonly Document _doc = document;
        private readonly ErrorModel _errorModel = errorModel;
        //private readonly ParameterValidatorMissingOrEmpty _parameterValidatorMissingOrEmpty = new(document, errorModel);


        public List<ElectricalSystem> GetAllCircuits(ElementId baseEquipmentId, ElectricalSystem endCircuit)
        {
            List<ElectricalSystem> allCircuits = [endCircuit];


            // visitedPanels содержит Id панелей, которые уже были проверены в текущем проходе цикла,
            // если мы снова наткнемся на ту же панель, значит у нас циклическая зависимость в построениях цепей
            // и нужно прервать цикл, чтобы избежать бесконечного обхода по этому кольцу
            HashSet<ElementId> visitedPanels = [baseEquipmentId];

            ElectricalSystem feederCircuit = endCircuit;
            FamilyInstance currentPanel = feederCircuit.BaseEquipment;

            //если цепь не подключена к панели
            if (currentPanel == null)
            {
                //уведомляем пользователя и завершаем код
                _errorModel.UserWarning(new NoConnectCircuit().MessageForUser(endCircuit));
            }

            //повторяется пока выполняется условие,
            //currentPanel != null - защита, чтоб не было выброшено исключение, когда currentPanel == null
            while (currentPanel != null && currentPanel.Id != baseEquipmentId)
            {
                ElementId panelId = currentPanel.Id;
                // если панель уже встречалась в обходе, значит есть циклическая зависимость
                if (visitedPanels.Contains(panelId))
                {
                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new СyclicDependenceOfPanels().MessageForUser(_doc, currentPanel));
                }
                visitedPanels.Add(panelId);


                MEPModel mepModel = currentPanel.MEPModel;
                //если семейство не является MEP оборудованием
                if (mepModel == null)
                {
                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new FamilyIsNotMEP().MessageForUser(_doc, currentPanel));
                }

                // Все цепи, включая цепь питания панели
                ISet<ElectricalSystem> circuitsAll = mepModel.GetElectricalSystems();

                // Только цепи нагрузок, без цепи питания панели
                List<ElementId> circuitsLoads = mepModel.GetAssignedElectricalSystems().Select(es => es.Id).ToList();

                ElectricalSystem circuitFeeder = null;  // питающая цепь
                                                        // Из всех цепей вычли цепи нагрузок и получили питающую цепь
                foreach (ElectricalSystem es in circuitsAll)
                {
                    //если цепи нет в нагрузках, то это питающая цепь
                    if (!circuitsLoads.Contains(es.Id))
                    {
                        circuitFeeder = es;
                        break;
                    }
                }

                if (circuitFeeder == null)
                    return allCircuits;

                //если сюда дошли, то это питающая цепь
                allCircuits.Add(circuitFeeder);
                feederCircuit = circuitFeeder;  // обновляем цепь
                currentPanel = circuitFeeder.BaseEquipment;  // обновляем BaseEquipment

                //если цепь не подключена к панели
                if (currentPanel == null)
                {
                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new NoConnectCircuit().MessageForUser(endCircuit));
                }
            }

            return allCircuits;
        }
    }
}
