using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using CalculationGroups.MyDll.UserWarningCalculationGroups;
using CalculationGroups.MyDll.Work;
using Libraries.ErrorModelLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CalculationGroups.MyDll
{
    /// <summary>
    /// извлекает метрики, электрические величины цепи
    /// </summary>
    public class CircuitMetrics
    {
        private readonly ErrorModel _errorModel;

        private const double KOEFLENGHTCIRCUIT = 1.05; // коэффициент запаса

        private readonly ParameterDefinition _parameterDefinition;
        private readonly ValidatorParameter _validatorParameter;

        //CultureInfo.InvariantCulture инвариантная культура всегда ожидает точку
        //как разделитель десятичной части, и она никак не зависит от системных настроек языка или региона
        private readonly CultureInfo _inv = CultureInfo.InvariantCulture;


        // Единицы измерения (кэшируются один раз)
        private ForgeTypeId _unitVoltage;
        private ForgeTypeId _unitActivePower;
        private ForgeTypeId _unitFullPower;
        private ForgeTypeId _unitLength;



        //Кэшируем Definition BuiltInParameter
        private Definition _defVoltage;
        private Definition _defCosF;
        private Definition _defActivePower;
        private Definition _defFullPower;
        private Definition _defLength;

        //Кэшируем Definition LookupParameter
        private Definition _defCabSection;

        public CircuitMetrics(Autodesk.Revit.DB.Document doc, ErrorModel errorModel)
        {
            _errorModel = errorModel;

            _validatorParameter = new(doc, errorModel);
           _parameterDefinition = new(doc, errorModel);
        }


        /// <summary>
        /// напряжение цепи в вольтах
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается напряжение</param>
        /// <returns>double в вольтах</returns>
        public double GetVoltage(ElectricalSystem circuit)
        {
            BuiltInParameter builtInParameter = BuiltInParameter.RBS_ELEC_VOLTAGE;

            //записываем в поле ForgeTypeId один раз
            _unitVoltage ??= circuit.get_Parameter(builtInParameter).GetUnitTypeId();

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defVoltage ??= _parameterDefinition.Get(circuit, builtInParameter);

            Parameter parameter = circuit.get_Parameter(_defVoltage);  //параметр по Definition из поля

            return UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), _unitVoltage);
        }


        /// <summary>
        /// коэффициент мощности (cos φ)
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается cos φ</param>56555
        /// <returns>double</returns>
        public double GetCosf(ElectricalSystem circuit)
        {
            BuiltInParameter builtInParameter = BuiltInParameter.RBS_ELEC_POWER_FACTOR;

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defCosF ??= _parameterDefinition.Get(circuit, builtInParameter);

            return circuit.get_Parameter(_defCosF).AsDouble();  //параметр по Definition из поля
        }


        /// <summary>
        /// активная мощность в киловаттах
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается активная мощность</param>
        /// <returns>double в киловаттах</returns>
        public double GetActivePower(ElectricalSystem circuit)
        {
            BuiltInParameter builtInParameter = BuiltInParameter.RBS_ELEC_TRUE_LOAD;

            //записываем в поле ForgeTypeId один раз
            _unitActivePower ??= circuit.get_Parameter(builtInParameter).GetUnitTypeId();

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defActivePower ??= _parameterDefinition.Get(circuit, builtInParameter);

            Parameter parameter = circuit.get_Parameter(_defActivePower);  //параметр по Definition из поля

            return UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), _unitActivePower) / 1000;
        }


        /// <summary>
        /// полная мощность в киловаттах
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается полная мощность</param>
        /// <returns>double в киловаттах</returns>
        public double GetFullPower(ElectricalSystem circuit)
        {
            BuiltInParameter builtInParameter = BuiltInParameter.RBS_ELEC_APPARENT_LOAD;

            //записываем в поле ForgeTypeId один раз
            _unitFullPower ??= circuit.get_Parameter(builtInParameter).GetUnitTypeId();

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defFullPower ??= _parameterDefinition.Get(circuit, builtInParameter);

            Parameter parameter = circuit.get_Parameter(_defFullPower);  //параметр по Definition из поля

            return UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), _unitFullPower) / 1000;
        }


        /// <summary>
        /// длина цепи с коэф = 1.05 округлённая до большего целого в метрах
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается длина</param>
        /// <returns>double в метрах</returns>
        public double GetLengthWithKoefUpRound(ElectricalSystem circuit)
        {
            BuiltInParameter builtInParameter = BuiltInParameter.RBS_ELEC_CIRCUIT_LENGTH_PARAM;

            //записываем в поле ForgeTypeId один раз
            _unitLength ??= circuit.get_Parameter(builtInParameter).GetUnitTypeId();

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defLength ??= _parameterDefinition.Get(circuit, builtInParameter);

            Parameter parameter = circuit.get_Parameter(_defLength);  //параметр по Definition из поля

            double length = UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), _unitLength);
            return Math.Ceiling((length * KOEFLENGHTCIRCUIT) / 1000);
        }


        /// <summary>
        /// момент мощности: киловатт на метр
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается момент мощности</param>
        /// <returns>double киловатт на метр</returns>
        public double GetKilowattOnMeter(ElectricalSystem circuit)
        {
            double activePower = GetActivePower(circuit);
            double lengthUpRound = GetLengthWithKoefUpRound(circuit);
            return activePower * lengthUpRound;
        }


        /// <summary>
        /// падение напряжения на цепи в процентах для 220 В
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается падение напряжения</param>
        /// <returns>double в процентах</returns>
        public double GetDUfor220V(ElectricalSystem circuit)
        {
            const double KOEFC = 12; // для 220В медь по таблице 12-9 Кнорринга
            double kilowattOnMeter = GetKilowattOnMeter(circuit);

            string nameParameter = "кабСечение";

            //записываем в поле Definition параметра один раз
            _defCabSection ??= _parameterDefinition.Get(circuit, nameParameter);


            //если не будет параметра или он будет пустым — покажет предупреждение пользователю и завершит код
            Parameter cableSectionParam = _validatorParameter.MissingAndEmptyWarning(circuit, _defCabSection);

            string sectionStr = cableSectionParam.AsString();
            // заменяем только если действительно есть запятая
            if (sectionStr.Contains(','))
                sectionStr = sectionStr.Replace(',', '.');

            //TryParse возвращает true, если строку удалось преобразовать в число и false, если преобразование не удалось.
            //А само значение числа записывает в переменную, переданную через out result(в нашем случае out double section).
            //_inv = CultureInfo.InvariantCulture инвариантная культура всегда ожидает точку как разделитель десятичной части, и она никак не зависит от системных настроек языка или региона
            if (double.TryParse(sectionStr, NumberStyles.Any, _inv, out double section))  // Пробуем получить сечение как число
                return kilowattOnMeter / KOEFC / section;

            // Если не удалось преобразовать в число, то выводим предупреждение пользователю и завершаем код
            _errorModel.UserWarning(new NotConvertToNumber().MessageForUser(circuit, nameParameter));
            return 0.0;  // эта строка не будет выполнена, так как выше вызывается _errorModel.UserWarning и код завершается
        }


        /// <summary>
        /// Из коллекции цепей возвращаетает цепь c максимальным моментом
        /// </summary>
        /// <param name="circuits">Коллекция цепей</param>
        /// <returns>Цепь с максимальным моментом, либо null, если коллекция пустая</returns>
        public ElectricalSystem GetCircuitWithMaxMoment(ICollection<ElectricalSystem> circuits)
        {
            ElectricalSystem maxCircuit = null;
            double maxMoment = double.MinValue;

            foreach (ElectricalSystem circuit in circuits)
            {
                double moment = GetKilowattOnMeter(circuit);
                if (moment > maxMoment)
                {
                    maxMoment = moment;
                    maxCircuit = circuit;
                }
            }

            return maxCircuit;

            //if (circuits == null || circuits.Count == 0) return null;
            ////LINQ создаёт временную коллекцию +делегат → неэффективно. Цикл в 20 раз быстрее на больших количествах элементов.
            //return circuits.OrderByDescending(circuit => GetKilowattOnMeter(circuit)).First();
        }
    }
}






//CircuitMetrics circuitMetrics = new(doc, errorModel);
//ElectricalSystem electricalSystem = null;

//FilteredElementCollector collector = new FilteredElementCollector(doc)
//        .OfCategory(BuiltInCategory.OST_ElectricalCircuit);

//foreach (ElectricalSystem elsystem in collector)
//{
//    if (elsystem.Id.IntegerValue == 14773954)
//    {
//        electricalSystem = elsystem;
//    }
//}

//string result = "";
//result += $"Id цепи: {electricalSystem.Id}\n";
//result += $"напряжение цепи в вольтах: {circuitMetrics.GetVoltage(electricalSystem)}\n";
//result += $"коэффициент мощности (cos φ): {circuitMetrics.GetCosf(electricalSystem)}\n";
//result += $"активная мощность в киловаттах: {circuitMetrics.GetActivePower(electricalSystem)}\n";
//result += $"полная мощность в киловаттах: {circuitMetrics.GetFullPower(electricalSystem)}\n";
//result += $"длина цепи с коэф: {circuitMetrics.GetLengthWithKoefUpRound(electricalSystem)}\n";
//result += $"момент мощности: киловатт на метр: {circuitMetrics.GetKilowattOnMeter(electricalSystem)}\n";
//result += $"падение напряжения: {circuitMetrics.GetDUfor220V(electricalSystem)}\n";
//result += $"Id цепи с максимальным моментом: {circuitMetrics.GetCircuitWithMaxMoment(elSystems).Id.IntegerValue}\n";

//TaskDialog.Show("Метрики цепи", $"цепь 14773954 {result}");








//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Electrical;
//using CalculationGroups.MyDll.UserWarningCalculationGroups;
//using CalculationGroups.MyDll.Work;
//using Libraries.ErrorModelLib;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;

//namespace CalculationGroups.MyDll
//{
//    /// <summary>
//    /// извлекает метрики, электрические величины цепи
//    /// </summary>
//    public class CircuitMetrics(Autodesk.Revit.DB.Document doc, ErrorModel errorModel)
//    {
//        private readonly Autodesk.Revit.DB.Document _doc = doc;
//        private readonly ErrorModel _errorModel = errorModel;
//        private const double KOEFLENGHTCIRCUIT = 1.05; // коэффициент запаса
//        private readonly ValidatorParameter _validatorParameter = new(doc, errorModel);
//        private readonly ParameterDefinition _parameterDefinition = new(doc, errorModel);



//        /// <summary>
//        /// напряжение цепи в вольтах
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        public double GetVoltage(ElectricalSystem circuit)
//        {
//            Parameter parameter = circuit.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE);
//            ForgeTypeId unit = parameter.GetUnitTypeId();
//            return UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), unit);
//        }


//        /// <summary>
//        /// коэффициент мощности (cos φ)
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        public double GetCosf(ElectricalSystem circuit)
//        {
//            return circuit.get_Parameter(BuiltInParameter.RBS_ELEC_POWER_FACTOR).AsDouble();
//        }


//        /// <summary>
//        /// активная мощность в киловаттах
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        public double GetActivePower(ElectricalSystem circuit)
//        {
//            Parameter parameter = circuit.get_Parameter(BuiltInParameter.RBS_ELEC_TRUE_LOAD);
//            ForgeTypeId unit = parameter.GetUnitTypeId();
//            return UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), unit) / 1000;
//        }


//        /// <summary>
//        /// полная мощность в киловаттах
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        public double GetFullPower(ElectricalSystem circuit)
//        {
//            Parameter parameter = circuit.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD);
//            ForgeTypeId unit = parameter.GetUnitTypeId();
//            return UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), unit) / 1000;
//        }


//        /// <summary>
//        /// длина цепи с коэф = 1.05 округлённая до большего целого в метрах
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        public double GetLengthWithKoefUpRound(ElectricalSystem circuit)
//        {
//            Parameter parameter = circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_LENGTH_PARAM);
//            ForgeTypeId unit = parameter.GetUnitTypeId();
//            double length = UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), unit);
//            return Math.Ceiling((length * KOEFLENGHTCIRCUIT) / 1000);
//        }


//        /// <summary>
//        /// момент мощности: киловатт на метр
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        public double GetKilowattOnMeter(ElectricalSystem circuit)
//        {
//            double activePower = GetActivePower(circuit);
//            double lengthUpRound = GetLengthWithKoefUpRound(circuit);
//            return activePower * lengthUpRound;
//        }


//        /// <summary>
//        /// падение напряжения на цепи в процентах для 220 В
//        /// </summary>
//        /// <param name="circuit"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        public double GetDUfor220V(ElectricalSystem circuit)
//        {
//            const double KOEFC = 12; // для 220В медь по таблице 12-9 Кнорринга
//            double kilowattOnMeter = GetKilowattOnMeter(circuit);

//            string nameParameter = "кабСечение";
//            Definition paramDefSech = _parameterDefinition.GetPathCircuits(circuit, "кабСечение");

//            //проверка наличия и заполненности параметра "кабСечение" и получение этого параметра
//            //если не будет параметра или он будет пустым — покажет предупреждение пользователю и завершит код
//            Parameter cableSectionParam = _validatorParameter.MissingAndEmptyWarning(circuit, paramDefSech);
//            ;

//            //если разделитель дробной части — запятая, то заменяем её на точку
//            string sectionStr = cableSectionParam.AsString()?.Replace(',', '.');

//            //TryParse возвращает true, если строку удалось преобразовать в число и false, если преобразование не удалось.
//            //А само значение числа записывает в переменную, переданную через out result(в нашем случае out double section).
//            //CultureInfo.InvariantCulture инвариантная культура всегда ожидает точку как разделитель десятичной части, и она никак не зависит от системных настроек языка или региона
//            if (double.TryParse(sectionStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double section))  // Пробуем получить сечение как число
//                return kilowattOnMeter / KOEFC / section;

//            // Если не удалось преобразовать в число, то выводим предупреждение пользователю и завершаем код
//            _errorModel.UserWarning(new NotConvertToNumber().MessageForUser(circuit, nameParameter));
//            return 0;  // эта строка не будет выполнена, так как выше вызывается _errorModel.UserWarning и код завершается
//        }


//        /// <summary>
//        /// Из коллекции цепей возвращаетает цепь c максимальным моментом
//        /// </summary>
//        /// <param name="circuits">Коллекция цепей</param>
//        /// <returns>Цепь с максимальным моментом, либо null, если коллекция пустая</returns>
//        public ElectricalSystem GetCircuitWithMaxMoment(ICollection<ElectricalSystem> circuits)
//        {
//            if (circuits == null || circuits.Count == 0) return null;
//            return circuits.OrderByDescending(circuit => GetKilowattOnMeter(circuit)).First();
//        }




//    }
//}

