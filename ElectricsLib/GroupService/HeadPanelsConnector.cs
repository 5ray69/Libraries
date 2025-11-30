using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using CalculationGroups.MyDll.UserWarningCalculationGroups;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;
using Libraries.ParametersLib.UserWarningParametersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationGroups.MyDll.Work
{
    public class HeadPanelsConnector(Document document, ErrorModel errorModel)
    {
        private readonly Document _doc = document;
        private readonly ErrorModel _errorModel = errorModel;
        private readonly ParameterValidatorMissingOrEmpty _parameterValidatorMissingOrEmpty = new(document, errorModel);


        public Dictionary<string, ElementId> GetPanelOfCircuitGroups(Dictionary<string, List<ElectricalSystem>> endCircuitGroups)
        {
            Dictionary<string, HashSet<ElementId>> headPanelOfGroups = new();


            ElectricalSystem anyCircuit = null;
            foreach (KeyValuePair<string, List<ElectricalSystem>> kvp in endCircuitGroups)
            {
                List<ElectricalSystem> listEs = kvp.Value;

                //если список не пуст
                if (listEs != null && listEs.Count > 0)
                {
                    anyCircuit = listEs[0];  // получили первый элемент
                    break;                // сразу завершаем цикл
                }
            }
            Definition paramDefinition = GetParameterDefinitionCirc(anyCircuit, "БУДОВА_Группа");


            foreach (KeyValuePair<string, List<ElectricalSystem>> kvpEs in endCircuitGroups)
            {
                string groupStr = kvpEs.Key;
                List<ElectricalSystem> listCircuit = kvpEs.Value;


                //Если гурппы/ключа нет в словаре
                if (!headPanelOfGroups.TryGetValue(groupStr, out var headPanels))
                {
                    headPanels = new HashSet<ElementId>();
                    headPanelOfGroups[groupStr] = headPanels;
                }

                foreach (ElectricalSystem endCircuit in listCircuit)
                {
                    // visitedPanels содержит Id панелей, которые уже были проверены в текущем проходе цикла,
                    // если мы снова наткнемся на ту же панель, значит у нас циклическая зависимость в построениях цепей
                    // и нужно прервать цикл, чтобы избежать бесконечного обхода по этому кольцу
                    HashSet<ElementId> visitedPanels = [];

                    //список питающей цепи, одна цепь в списке
                    List<ElectricalSystem> feederCircuit = [endCircuit];

                    List<ElectricalSystem> circuitFeeder = new(1);  // выделяем память под одну цепь

                    //повторяется пока feederCircuit не станет пустым
                    while (feederCircuit.Count > 0)
                    {
                        ElectricalSystem currentCircuit = feederCircuit[0];
                        feederCircuit.Clear(); // чтобы не хранить лишнее
                        circuitFeeder.Clear(); // чтобы не хранить лишнее

                        FamilyInstance baseEquipment = currentCircuit.BaseEquipment;
                        //если цепь не подключена к панели
                        if (baseEquipment == null)
                        {
                            //уведомляем пользователя и завершаем код
                            _errorModel.UserWarning(new NoConnectCircuit().MessageForUser(endCircuit));
                        }

                        ElementId baseEquipmentId = baseEquipment.Id;

                        // если панель уже встречалась в обходе, значит есть циклическая зависимость
                        if (visitedPanels.Contains(baseEquipmentId))
                        {
                            //уведомляем пользователя и завершаем код
                            _errorModel.UserWarning(new СyclicDependenceOfPanels().MessageForUser(_doc, baseEquipment, groupStr));
                        }

                        visitedPanels.Add(baseEquipmentId);


                        MEPModel mepModel = baseEquipment.MEPModel;

                        //если семейство не является MEP оборудованием
                        if (mepModel == null)
                        {
                            //уведомляем пользователя и завершаем код
                            _errorModel.UserWarning(new FamilyIsNotMEP().MessageForUser(_doc, baseEquipment));
                        }


                        ConnectorSet connectors = mepModel.ConnectorManager.Connectors;

                        foreach (Connector connector in connectors)
                        {
                            if (connector.Domain == Domain.DomainElectrical)  //коннектор питающей цепи Domain = DomainElectrical, у отходящей цепи Domain = DomainUndefined
                            {
                                //если есть питающая цепь, то в AllRefs виртуальный коннектор цепи
                                ConnectorSet allRefs = connector.AllRefs;

                                if (allRefs.Size > 0)
                                {
                                    foreach (Connector conRef in allRefs)
                                    {
                                        ElectricalSystem electricalSystem = conRef.Owner as ElectricalSystem;

                                        string valueGroupCircuit = GetValuePramGroup(electricalSystem, paramDefinition);

                                        //если БУДОВА_Группа цепи равна groupStr, то нужно продолжать обход, это еще не головная панель
                                        //StringComparer.Ordinal.Equals(valueGroupCircuit, groupStr) - скоростное сравнение двух строк valueGroupCircuit == groupStr
                                        if (StringComparer.Ordinal.Equals(valueGroupCircuit, groupStr))
                                        {
                                            // добавляем цепь в circuitFeeder
                                            circuitFeeder.Add(electricalSystem);
                                            break; // выход из цикла после первой итерации
                                        }
                                    }
                                }

                                //если питацающей цепи нет, список пуст, значит это и есть головная панель
                                if (allRefs.Size == 0)
                                {
                                    headPanelOfGroups[groupStr].Add(baseEquipmentId);
                                    break;
                                }
                            }
                        }

                        feederCircuit = circuitFeeder;
                    }
                }
            }

            //headPanelOfGroups.Count - задаем начальную ёмкость словаря resultDict,
            var resultDict = new Dictionary<string, ElementId>(headPanelOfGroups.Count);
            foreach (var kvpR in headPanelOfGroups)
            {
                // берем первую найденную головную панель, если их несколько
                ElementId firstId = kvpR.Value.FirstOrDefault();
                resultDict[kvpR.Key] = firstId;
            }

            string stringWarning = "";
            foreach (KeyValuePair<string, HashSet<ElementId>> kvpW in headPanelOfGroups)
            {
                if (kvpW.Value.Count > 1)
                {
                    stringWarning += $"Имя группы: {kvpW.Key} - Id головных панелей: {string.Join(", ", kvpW.Value.Select(id => id.IntegerValue))}\n";
                }
            }

            //если строка не пустая, значит есть дублирующиеся группы
            if (!string.IsNullOrEmpty(stringWarning))
            {
                //уведомляем пользователя и завершаем код
                _errorModel.UserWarning(new DuplicateGroups().MessageForUser(stringWarning));
            }

            return resultDict;
            //return headPanelOfGroups.ToDictionary(k => k.Key, v => v.Value.ToList());

        }





        /// <summary>
        /// получаем Definition один раз, чтоб не получать значение параметра по LookupParameter
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private Definition GetParameterDefinitionCirc(ElectricalSystem electricalSystem, string paramName)
        {
            //if (_doc.GetElement(panelId) is not FamilyInstance panel)
            //    return null;

            //ElectricalSystem firstCircuit = panel.MEPModel.GetAssignedElectricalSystems().FirstOrDefault();
            //внутри ValidateAndWarning выполняется LookupParameter(paramName) = выполняем один раз
            //Parameter param = _parameterValidatorMissingOrEmpty.ValidateAndWarning(firstCircuit, paramName);
            Parameter param = _parameterValidatorMissingOrEmpty.ValidateAndWarning(electricalSystem, paramName);


            return param.Definition;
        }


        private string GetValuePramGroup(ElectricalSystem elSystem, Definition paramDefinition)
        {
            Parameter param = elSystem.get_Parameter(paramDefinition);

            //если у цепи нет параметра, то выведет предупреждение пользователю и завершит код
            if (param == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(elSystem, "БУДОВА_Группа"));
            }

            ////если у цепи параметр только для чтения, то выведет предупреждение пользователю и завершит код
            //if (param.IsReadOnly)
            //{
            //    _errorModel.UserWarning(new ParameterIsReadOnly().MessageForUser(elSystem, parameterName));
            //}
            string groupName = param.AsString();

            if (string.IsNullOrWhiteSpace(groupName))  //если параметр цепи не заполнен, то выведет предупреждение пользователю и завершит код
            {
                _errorModel.UserWarning(new ParameterElementAtLevelEmpty().MessageForUser(_doc, elSystem, paramDefinition.Name));
            }

            return groupName;
        }
    }
}
