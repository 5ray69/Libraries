using Autodesk.Revit.DB;
using System;

namespace CreateShemPIcosf.MyDll
{
    public class AllElementsPanelResult
    {
        public double ActivPowA{get;}  // активная мощность фазы А панели
        public double ActivPowB{get;}  // активная мощность фазы В панели
        public double ActivPowC{get;}  // активная мощность фазы С панели

        public double TokA{get;}  // ток фазы А панели
        public double TokB{get;}  // ток фазы В панели
        public double TokC{get;}  // ток фазы С панели


        public double ApparentPowerSum{get;} // суммарная полная/кажущаяся мощность всех одно и трехфазных элементов одной панели
        public double ActivPowerSum{get;}  // суммарная активная мощность всех одно и трехфазных элементов одной панели
        public double CosSum{get;}  // cos всей панели, как отношение суммарной активной мощности к суммарной полной мощности
        public double TgSum{get;}  // tg всей панели, полученный из CosSum
        public double ReactivPowerSum{get;}  // суммарная реактивная мощность всех одно и трехфазных элементов одной панели

        public double TokFromMaxFaz{get;}  // трехфазный ток по максимально загруженной фазе всех одно и трехфазных элементов одной панели
        public XYZ LocationXYZ{get;}  // XYZ под семейством с минимальной координатой X в панели


        public double PercentAB{get;}  // процентное соотношение токов фаз А и В
        public double PercentAC{get;}  // процентное соотношение токов фаз А и С
        public double PercentBC{get;}  // процентное соотношение токов фаз В и С

        public AllElementsPanelResult(
            double pA, double pB, double pC,
            double tA, double tB, double tC,
            double apparent, double active,
            double cos, double tg, double reactive,
            double maxTok, XYZ location)
        {
            ActivPowA = pA;
            ActivPowB = pB;
            ActivPowC = pC;
            TokA = tA;
            TokB = tB;
            TokC = tC;
            ApparentPowerSum = apparent;
            ActivPowerSum = active;
            CosSum = cos;
            TgSum = tg;
            ReactivPowerSum = reactive;
            TokFromMaxFaz = maxTok;
            LocationXYZ = location;

            PercentAB = CalcPercent(TokA, TokB);
            PercentAC = CalcPercent(TokA, TokC);
            PercentBC = CalcPercent(TokB, TokC);
        }
        private double CalcPercent(double tok1, double tok2)
        {
            if (tok2 != 0)
                return Math.Round(Math.Abs(tok1 / tok2 - 1.0) * 100.0, 1);

            return tok1 > 0 ? 100.0 : 0.0;
        }
    }
}
