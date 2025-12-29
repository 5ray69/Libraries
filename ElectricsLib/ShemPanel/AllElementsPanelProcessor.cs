using Autodesk.Revit.DB;
using CreateShemPIcosf.MyDll.UserWarning;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;
using System;
using System.Collections.Generic;

namespace CreateShemPIcosf.MyDll
{
    public class AllElementsPanelProcessor
    {
        private readonly ErrorModel _errorModel;
        private readonly ParameterValidatorIsMissing _parameterValidatorIsMissing;

        private readonly double offsetForY = 2800;

        public AllElementsPanelProcessor(ErrorModel errorModel)
        {
            _errorModel = errorModel;
            _parameterValidatorIsMissing = new(_errorModel);
        }


        public AllElementsPanelResult Process(IList<FamilyInstance> elements)
        {

            double pA = 0, pB = 0, pC = 0;  // переменные для активной мощности каждой фазы
            double tA = 0, tB = 0, tC = 0;  // переменные для тока каждой фазы
            double apparentSum = 0;  // переменная для суммарной полной/кажущейся мощности

            Element minXElement = null;
            double minX = double.MaxValue;

            foreach (FamilyInstance familyInstance in elements)
            {
                // ---------- location ---------- находим семейство с минимальной координатой X в панели
                if (familyInstance.Location is LocationPoint lp)
                {
                    double x = lp.Point.X;
                    if (x < minX)
                    {
                        minX = x;
                        minXElement = familyInstance;
                    }
                }

                // ---------- parameters ----------
                //Если параметр отсутсвует, то показываем предупреждение пользователю и завершаем код
                Parameter pPower = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_Активная мощность кВт");

                Parameter pTok = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_Iном");

                double power = pPower.AsDouble();  // активная мощность одного элемента панели
                double tok = (double)pTok.AsInteger();  // ток одного элемента панели

                Parameter paramCos = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_cos φ");
                double cos = paramCos.AsDouble();  // cos одного элемента панели
                if (cos == 0)
                {
                    _errorModel.UserWarning(new CosIsNull().MessageForUser(familyInstance));
                }

                apparentSum += power / cos;  // полная/кажущаяся мощность

                Parameter param220 = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_220");
                bool is220 = param220.AsInteger() == 1;
                Parameter param380 = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_380");
                bool is380 = param380.AsInteger() == 1;

                // Получаем параметры фаз один раз
                Parameter paramPhaseA = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_фаза А");
                Parameter paramPhaseB = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_фаза В");
                Parameter paramPhaseC = _parameterValidatorIsMissing.ValidateExistsParameter(familyInstance, "BDV_E000_фаза С");

                // Считаем количество фаз здесь
                int phasesCount =
                    (paramPhaseA?.AsInteger() == 1 ? 1 : 0) +
                    (paramPhaseB?.AsInteger() == 1 ? 1 : 0) +
                    (paramPhaseC?.AsInteger() == 1 ? 1 : 0);


                if (is220)
                {
                    //Если phasesCount > 1 то показываем предупреждение пользователю и завершаем код
                    if (phasesCount > 1)
                    {
                        _errorModel.UserWarning(new MultiplePhases().MessageForUser(familyInstance));
                    }

                    if (paramPhaseA.AsInteger() == 1)
                    {
                        pA += power;  // активная мощность фаза А
                        tA += tok;
                    }
                    if (paramPhaseB.AsInteger() == 1)
                    {
                        pB += power;  // активная мощность фаза B
                        tB += tok;
                    }
                    if (paramPhaseC.AsInteger() == 1)
                    {
                        pC += power;  // активная мощность фаза C
                        tC += tok;
                    }
                }

                if (is380)
                {
                    //Если phasesCount == 0 то показываем предупреждение пользователю и завершаем код
                    if (phasesCount > 0)
                    {
                        _errorModel.UserWarning(new SetPhasesIn380().MessageForUser(familyInstance));
                    }

                    double part = power / 3.0;
                    pA += part; pB += part; pC += part;
                    tA += tok; tB += tok; tC += tok;
                }
            }

            XYZ location = null;
            if (minXElement != null)
            {
                XYZ p = ((LocationPoint)minXElement.Location).Point;
                double offset = UnitUtils.ConvertToInternalUnits(offsetForY, UnitTypeId.Millimeters);
                location = new XYZ(p.X, p.Y - offset, p.Z);
            }

            double activSum = pA + pB + pC;  // активная мощность, как сумма активных мощностей каждой из фаз
            double cosSum = Math.Round(activSum / apparentSum, 2);  // cos всей панели
            double tgSum = cosSum != 0
                ? Math.Sqrt(1 - cosSum * cosSum) / cosSum
                : 0;   // tg всей панели, полученный из cosSum

            return new AllElementsPanelResult(
                pA, pB, pC,
                tA, tB, tC,
                apparentSum,
                activSum,
                cosSum,
                tgSum,
                activSum * tgSum,
                Math.Max(tA, Math.Max(tB, tC)),  // трехфазный ток по максимально загруженной фазе всех одно и трехфазных элементов одной панели
                location
            );
        }
    }
}
