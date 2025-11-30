using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using CalculationGroups.MyDll.UserWarningCalculationGroups;
using Libraries.ErrorModelLib;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CalculationGroups.MyDll.Work
{
    public class GroupLength
    {
        private readonly Document _doc;
        private readonly ErrorModel _errorModel;

        //Кэшируем Definition LookupParameter
        private Definition _defTypeCable;
        private Definition _defCabSection;
        private Definition _defCountOnSect;
        private Definition _defTypeInsulation;

        private readonly ValidatorParameter _validatorParameter;
        private readonly ParameterDefinition _parameterDefinition;

        private readonly CircuitMetrics _circuitMetrics;

        //CultureInfo.InvariantCulture инвариантная культура всегда ожидает точку
        //как разделитель десятичной части, и она никак не зависит от системных настроек языка или региона
        private readonly CultureInfo _inv = CultureInfo.InvariantCulture;


        public GroupLength(Document doc, ErrorModel errorModel)
        {
            _doc = doc;
            _errorModel = errorModel;

            _validatorParameter = new(doc, errorModel);
            _parameterDefinition = new(doc, errorModel);

            _circuitMetrics = new CircuitMetrics(doc, errorModel);
        }


        /// <summary>
        /// получить длины всех цепей каждой группы
        /// </summary>
        /// <param name="groupCircuits">все цепи в каждой группе</param>
        /// <returns>Dictionary<string, Dictionary<int, double>></returns>
        public Dictionary<string, Dictionary<int, CableInfo>> GetLength(Dictionary<string, List<ElectricalSystem>> groupCircuits)
        {
            Dictionary<string, Dictionary<int, CableInfo>> result = [];

            //перебираем каждую группу
            foreach (KeyValuePair<string, List<ElectricalSystem>> group in groupCircuits)
            {
                //имя группы
                string groupName = group.Key;

                //все имеющиеся цепи внутри группы
                List<ElectricalSystem> circuits = group.Value;

                //словарь для каждой группы
                Dictionary<int, CableInfo> typeCabInfo = [];


                foreach (ElectricalSystem circuit in circuits)
                {
                    const string nameParamTypeCab = "Тип кабеля";

                    //записываем в поле Definition параметра один раз
                    _defTypeCable ??= _parameterDefinition.Get(circuit, nameParamTypeCab);

                    //если не будет параметра или он будет пустым — покажет предупреждение пользователю и завершит код
                    Parameter paramTypeCable = _validatorParameter.MissingAndEmptyWarning(circuit, _defTypeCable);

                    ElementId elementId = paramTypeCable.AsElementId();

                    int id = elementId.IntegerValue;

                    double length = _circuitMetrics.GetLengthWithKoefUpRound(circuit);

                    // Получаем или создаём CableInfo
                    bool created = !typeCabInfo.TryGetValue(id, out CableInfo info);

                    //если ключа нет в словаре, то typeCabInfo.TryGetValue(id, out CableInfo info) == false? а info = null
                    if (created)
                    {
                        info = new CableInfo();
                        typeCabInfo[id] = info; // создаем новую пару ключ-значение
                    }

                    // суммируем длину с имеющейся в экземпляре класса, в свойстве TotalLength
                    info.TotalLength += length;



                    //параметры цепи одинаковые для одного и того же типа кабеля
                    //если ключ есть в словаре, тогда не задаем параметры, потому что они такие же как были при создании ключа
                    //если ключа нет в словаре, то typeCabInfo.TryGetValue(id, out CableInfo info) == false, тогда задаем параметры
                    if (created)
                    {
                        const string nameParamSection = "кабСечение";
                        _defCabSection ??= _parameterDefinition.Get(circuit, nameParamSection);
                        Parameter paramCabSection = _validatorParameter.MissingAndEmptyWarning(circuit, _defCabSection);

                        string sectionStr = paramCabSection.AsString();
                        // заменяем только если действительно есть запятая
                        if (sectionStr.Contains(','))
                            sectionStr = sectionStr.Replace(',', '.');

                        //TryParse возвращает true, если строку удалось преобразовать в число и false, если преобразование не удалось.
                        //А само значение числа записывает в переменную, переданную через out result(в нашем случае out double section).
                        //_inv = CultureInfo.InvariantCulture инвариантная культура всегда ожидает точку как разделитель десятичной части, и она никак не зависит от системных настроек языка или региона
                        if (double.TryParse(sectionStr, NumberStyles.Any, _inv, out double section))  // Пробуем получить сечение как число
                        {
                            info.CableSection = section;
                        }
                        else
                        {
                            // Если не удалось преобразовать в число, то выводим предупреждение пользователю и завершаем код
                            _errorModel.UserWarning(new NotConvertToNumber().MessageForUser(circuit, nameParamSection));
                        }



                        const string nameParamCountOnSect = "кабКолво на сеч";

                        //записываем в поле Definition параметра один раз
                        _defCountOnSect ??= _parameterDefinition.Get(circuit, nameParamCountOnSect);
                        Parameter paramCountOnSect = _validatorParameter.MissingAndEmptyWarning(circuit, _defCountOnSect);

                        string countOnSect = paramCountOnSect.AsString();
                        info.CountOnSection = countOnSect;



                        const string nameParamTypeIns = "кабТип изоляции";

                        //записываем в поле Definition параметра один раз
                        _defTypeInsulation ??= _parameterDefinition.Get(circuit, nameParamTypeIns);
                        Parameter paramTypeInsulation = _validatorParameter.MissingAndEmptyWarning(circuit, _defTypeInsulation);

                        string typeInsulation = paramTypeInsulation.AsString();
                        info.TypeInsulation = typeInsulation;

                    }


                }
                result[groupName] = typeCabInfo;
            }

            return result;
        }
    }



    public class CableInfo
    {
        public double TotalLength{get; set;}  // суммируется
        public double CableSection{get; set;}  // не суммируется
        public string CountOnSection{get; set;}  // не суммируется
        public string TypeInsulation{get; set;}  // не суммируется
    }

    //'кабКолво на сеч'
    //'кабТип изоляции'
}



