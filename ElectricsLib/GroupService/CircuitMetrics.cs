using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Libraries.ElectricsLib.GroupService
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
        private Definition _defCurrentApparent;

        //Кэшируем Definition LookupParameter
        private Definition _defCabSection;
        private Definition _defMaterial;

        // Все данные из Справочника по проектированию электроснабжения линий электропередачи и сетей
        // Под редакцией Я.М.Большама, В.И.Круповича, М.Л.Самовера стр.670-676
        private const double Cʋ = 1.12;  // температурный коэффициент, учитывающий изменение активного удельного сопротивления проводника при его температуре ʋ = 50°C, отличной от 20°C, таблица 4-71
        private const double Cc = 1.02;  // Для многопроволочных жил Cc = 1.02, для шин и однопроволочных/монолитных проводов Cc = 1, таблица 4-74
        private const double Cпэ = 1.0;  // Коэффициент поверхностного эффекта начинается для 150 мм кв с Cпэ = 1.01 и выше (не значительный), таблица 4-75


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
            // Напряжение цепи зависит от напряжения нагрузки этой цепи
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
        /// ток от полной установленной мощности
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается длина</param>
        /// <returns>double в амперах</returns>
        public double GetCurrentApparent(ElectricalSystem circuit)
        {
            BuiltInParameter builtInParameter = BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PARAM;

            // записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defCurrentApparent ??= _parameterDefinition.Get(circuit, builtInParameter);

            Parameter parameter = circuit.get_Parameter(_defCurrentApparent);  //параметр по Definition из поля

            return parameter.AsDouble();
        }


        /// <summary>
        /// сечение фазного проводника кабеля в миллиметрах квадратных
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается падение напряжения</param>
        /// <returns>double в миллиметрах квадратных</returns>
        public double GetPhaseSection(ElectricalSystem circuit)
        {
            string nameParameter = "кабСечение";

            // записываем в поле Definition параметра один раз
            _defCabSection ??= _parameterDefinition.Get(circuit, nameParameter);


            // если не будет параметра или он будет пустым — покажет предупреждение пользователю и завершит код
            Parameter cableSectionParam = _validatorParameter.MissingAndEmptyWarning(circuit, _defCabSection);

            string sectionStr = cableSectionParam.AsString();
            // заменяем только если действительно есть запятая
            if (sectionStr.Contains(','))
                sectionStr = sectionStr.Replace(',', '.');

            // TryParse возвращает true, если строку удалось преобразовать в число и false, если преобразование не удалось.
            // А само значение числа записывает в переменную, переданную через out result(в нашем случае out double section).
            // _inv = CultureInfo.InvariantCulture инвариантная культура всегда ожидает точку как разделитель десятичной части, и она никак не зависит от системных настроек языка или региона
            if (double.TryParse(sectionStr, NumberStyles.Any, _inv, out double section))  // Пробуем получить сечение как число
                return section;

            // Если не удалось преобразовать в число, то выводим предупреждение пользователю и завершаем код
            _errorModel.UserWarning(new NotConvertToNumber().MessageForUser(circuit, nameParameter));
            return 0.0;  // эта строка не будет выполнена, так как выше вызывается _errorModel.UserWarning и код завершается
        }


        public string GetMaterial(ElectricalSystem circuit)
        {
            string nameParameter = "кабМатериал";

            // записываем в поле Definition параметра один раз
            _defMaterial ??= _parameterDefinition.Get(circuit, nameParameter);

            // если не будет параметра или он будет пустым — покажет предупреждение пользователю и завершит код
            Parameter cableMaterialParam = _validatorParameter.MissingAndEmptyWarning(circuit, _defMaterial);

            return cableMaterialParam.AsString();
        }


        /// <summary>
        /// падение напряжения на цепи в процентах для 220 В
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается падение напряжения</param>
        /// <returns>double в процентах</returns>
        public double GetDUfor220V(ElectricalSystem circuit)
        {
            string material = GetMaterial(circuit);

            // По таблице 12-9 Кнорринга, стр 348, для Cu С=12, , для Al С=7.4 в формуле (12-17) на стр 346
            // Сравнение без учета регистра, лучше использовать метод Equals() с параметром StringComparison, чтобы явно указать правила сравнения
            double C = material.Equals("Cu", StringComparison.OrdinalIgnoreCase) ? 12.0 : 7.4;

            double kilowattOnMeter = GetKilowattOnMeter(circuit);

            double section = GetPhaseSection(circuit);

            return kilowattOnMeter / C / section;
        }


        /// <summary>
        /// падение напряжения на цепи в процентах для 380 В
        /// </summary>
        /// <param name="circuit">цепь из которой извлекается падение напряжения</param>
        /// <returns>double в процентах</returns>
        public double GetDUfor380V(ElectricalSystem circuit)
        {
            // Все данные из Справочника по проектированию электроснабжения линий электропередачи и сетей
            // Под редакцией Я.М.Большама, В.И.Круповича, М.Л.Самовера стр.670-676

            string material = GetMaterial(circuit);

            // активное удельное сопротивление постоянному току при температуре 20°C, Ом·мм²/м, для Cu = 0.0175, для Al=0.0295, стр.671
            double ρ20 = material.Equals("Cu", StringComparison.OrdinalIgnoreCase) ? 0.0175 : 0.0295;

            // числитель активного сопротивления проводника на единицу длины линии Ом/км
            double r20n = (Cʋ * Cc * Cпэ * ρ20 * 1000);

            // индуктивное сопротивление проводника на единицу длины линии Ом/км; для кабелей Xо =  0.0590483, формула 4-69 и
            // мой пересчет для кабеля из формулы 4-70 (потом можно будет взять инфо из таблицы для разных кабелей)
            double Xо = 0.0590483;

            // номинальное напряжение в кВ, так как расчет только для 380, то всегда 0.38
            double Uc380 = Math.Sqrt(3) / (10 * 0.38);  // для трехфазной системы, равномерно загружены фазы, нагрузка в конце кабеля

            // сечение фазного проводника
            double section = GetPhaseSection(circuit);

            double cosF = GetCosf(circuit);
            double sinF = Math.Sqrt(1 - (cosF * cosF));

            double e = Uc380 * (((r20n / section) * cosF) + (Xо * sinF));  // удельная потеря напряжения в %, часть формулы из таблицы 4-71, пункт 3, подпункт г)

            double I = GetCurrentApparent(circuit);  // ток, А
            double L = GetLengthWithKoefUpRound(circuit) / 1000;  // длина в км

            double dU = I * L * e;  // падение напряжения в %, формула из таблицы 4-71, пункт 3, подпункт г)

            return dU;
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
//result += $"падение напряжения для 220в: {circuitMetrics.GetDUfor220V(electricalSystem)}\n";
//result += $"падение напряжения для 380в: {circuitMetrics.GetDUfor380V(electricalSystem)}\n";
//result += $"Id цепи с максимальным моментом: {circuitMetrics.GetCircuitWithMaxMoment(elSystems).Id.IntegerValue}\n";

//TaskDialog.Show("Метрики цепи", $"цепь 14773954 {result}");
