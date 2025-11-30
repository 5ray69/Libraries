using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using CalculationGroups.MyDll.UserWarningCalculationGroups;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;
using System.Collections.Generic;
using System.Linq;

namespace CalculationGroups.MyDll.Work
{

    public class FindHeadPanel(Document document, ErrorModel errorModel)
    {
        private readonly Document _doc = document;
        private readonly ErrorModel _errorModel = errorModel;



        public FamilyInstance Get(ElectricalSystem сircuit)
        {
            FamilyInstance headPanel = null;

            // visitedPanels содержит Id панелей, которые уже были проверены в текущем проходе цикла,
            // если мы снова наткнемся на ту же панель, значит у нас циклическая зависимость в построениях цепей
            // и нужно прервать цикл, чтобы избежать бесконечного обхода по этому кольцу
            HashSet<ElementId> visitedPanels = [];

            //список питающей цепи, одна цепь в списке
            List<ElectricalSystem> feederCircuit = [сircuit];


            //повторяется пока feederCircuit не станет пустым
            while (feederCircuit.Count > 0)
            {
                ElectricalSystem currentCircuit = feederCircuit[0];
                feederCircuit.Clear(); // чтобы не хранить лишнее

                FamilyInstance baseEquipment = currentCircuit.BaseEquipment;
                //если цепь не подключена к панели
                if (baseEquipment == null)
                {
                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new NoConnectCircuit().MessageForUser(сircuit));
                }

                ElementId baseEquipmentId = baseEquipment.Id;

                // если панель уже встречалась в обходе, значит есть циклическая зависимость
                if (visitedPanels.Contains(baseEquipmentId))
                {
                    Parameter paramGroup = new ParameterValidatorMissingOrEmpty(_doc, _errorModel).ValidateAndWarning(baseEquipment, "БУДОВА_Группа");
                    string groupName = paramGroup.AsString();

                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new СyclicDependenceOfPanels().MessageForUser(_doc, baseEquipment, groupName));
                }

                visitedPanels.Add(baseEquipmentId);


                MEPModel mepModel = baseEquipment.MEPModel;

                //если семейство не является MEP оборудованием
                if (mepModel == null)
                {
                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new FamilyIsNotMEP().MessageForUser(_doc, baseEquipment));
                }

                // Все цепи, включая цепь питания панели
                ISet<ElectricalSystem> circuitsAll = mepModel.GetElectricalSystems();

                // Только цепи нагрузок, без цепи питания панели
                List<ElementId> circuitsLoads = mepModel.GetAssignedElectricalSystems().Select(es => es.Id).ToList();

                List<ElectricalSystem> circuitFeeder = [];  // питающая цепь
                // Из всех цепей вычли цепи нагрузок и получили питающую цепь
                foreach (ElectricalSystem es in circuitsAll)
                {
                    ElementId esId = es.Id;
                    //если в нагрузках нет цепи, то добавляем ее в circuitFeeder
                    if (!circuitsLoads.Contains(esId))
                    {
                        circuitFeeder.Add(es);
                    }
                }


                // если питающей цепи нет, список пуст, значит это и есть головная панель
                if (!circuitFeeder.Any())
                {
                    headPanel = baseEquipment;
                    break;
                }

                feederCircuit = circuitFeeder;
            }


            return headPanel;
        }

    }
}
