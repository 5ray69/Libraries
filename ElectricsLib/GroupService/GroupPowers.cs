using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;
using Libraries.ParametersLib.UserWarningParametersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationGroups.MyDll.Work
{
    public class GroupPowers
    {
        private readonly Document _doc;
        private readonly ErrorModel _errorModel;

        private readonly ParameterValidatorMissingOrEmpty _parameterValidatorMissingOrEmpty;


        public GroupPowers(Document doc, ErrorModel errorModel)
        {
            _doc = doc;
            _errorModel = errorModel;
            _parameterValidatorMissingOrEmpty = new ParameterValidatorMissingOrEmpty(doc, errorModel);
        }

        public Dictionary<string, GroupData> Get(Dictionary<string, ElementId> headPanels)
        {
            Dictionary<string, GroupData> result = [];


            //Получаем Definition параметра "БУДОВА_Группа" один раз
            Definition paramDefinition = GetParameterDefinition(headPanels.Values.First(), "БУДОВА_Группа");

            //кэшируем для Id панели ее цепи нагрузок
            Dictionary<ElementId, List <ElectricalSystem>> cache = new();


            //Кэшируем для Id панели ее цепи нагрузок
            foreach (KeyValuePair<string, ElementId> kvp in headPanels)
            {
                ElementId panelIds = kvp.Value;

                if (!cache.ContainsKey(panelIds))
                {
                    FamilyInstance familyInstance = (FamilyInstance)_doc.GetElement(panelIds);
                    MEPModel mepModel = familyInstance.MEPModel;
                    // Только цепи нагрузок, без цепи питания панели
                    List<ElectricalSystem> circuitsLoads = mepModel.GetAssignedElectricalSystems().ToList();

                    cache[panelIds] = circuitsLoads;
                }
            }


            //цепей одной группы может быть подключено к головной панели несколько, потому суммируем их мощность
            foreach (KeyValuePair<string, ElementId> kvp in headPanels)
            {
                string groupName = kvp.Key;
                ElementId panelIds = kvp.Value;

                //Кэшируем цепи нагрузок в словаре, ключ = Id панели
                if (!cache.ContainsKey(panelIds))
                {
                    FamilyInstance familyInstance = (FamilyInstance)_doc.GetElement(panelIds);
                    MEPModel mepModel = familyInstance.MEPModel;
                    // Только цепи нагрузок, без цепи питания панели
                    cache[panelIds] = mepModel.GetAssignedElectricalSystems().ToList();
                }


                double totalActivePower = 0;
                double totalFullPower = 0;

                List<ElectricalSystem> loadCircuits = cache[panelIds];
                foreach (ElectricalSystem elSystem in loadCircuits)
                {
                    Parameter param = elSystem.get_Parameter(paramDefinition);

                    if (param == null)  //если у цепи нет параметра, то выведет предупреждение пользователю и завершит код
                        _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(elSystem, paramDefinition.Name));

                    if (string.IsNullOrWhiteSpace(param.AsString()))  //если параметр цепи не заполнен, то выведет предупреждение пользователю и завершит код
                        _errorModel.UserWarning(new ParameterElementAtLevelEmpty().MessageForUser(_doc, elSystem, paramDefinition.Name));

                    if (param.AsString() != groupName)
                        continue;


                    //Ативная мощность
                    Parameter activePowerParam = elSystem.get_Parameter(BuiltInParameter.RBS_ELEC_TRUE_LOAD);

                    if (activePowerParam == null)
                        _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(elSystem, "Активная нагрузка RBS_ELEC_TRUE_LOAD"));

                    totalActivePower += ConvertToKilowatts(activePowerParam);


                    //Полная мощность
                    Parameter fullPowerParam = elSystem.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD);

                    if (fullPowerParam == null)
                        _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(elSystem, "Полная установленная мощность RBS_ELEC_APPARENT_LOAD"));

                    totalFullPower += ConvertToKilowatts(fullPowerParam);
                }


                //КОСИНУС
                //если косинус сделать = 0, то ток в семействе будет деление на ноль, поэтому = 1
                //Math.Abs(totalFullPower) > 1e-9 это защита от деления на почти нуль
                double totalCosF = Math.Abs(totalFullPower) > 1e-9 ? totalActivePower / totalFullPower : 1;


                result[groupName] = new GroupData
                {
                    ActivePower = totalActivePower,
                    FullPower = totalFullPower,
                    CosF = totalCosF
                };
            }

            return result;
        }


        private Definition GetParameterDefinition(ElementId panelId, string paramName)
        {
            if (_doc.GetElement(panelId) is not FamilyInstance panel)
                return null;

            ElectricalSystem firstCircuit = panel.MEPModel.GetAssignedElectricalSystems().FirstOrDefault();
            //внутри ValidateAndWarning выполняется LookupParameter(paramName) = выполняем один раз
            Parameter param = _parameterValidatorMissingOrEmpty.ValidateAndWarning(firstCircuit, paramName);


            return param.Definition;
        }


        private double ConvertToKilowatts(Parameter param)
        {
            ForgeTypeId unitTypeId = param.GetUnitTypeId();
            double internalValue = param.AsDouble();
            double watts = UnitUtils.ConvertFromInternalUnits(internalValue, unitTypeId);
            return watts / 1000.0;
        }
    }
}
