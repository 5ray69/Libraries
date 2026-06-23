using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.ShemPanel
{
    /// <summary>
    /// Все параметры сборки
    /// </summary>
    public class AllElementsPanelResult
    {
        /// <summary>
        /// активная мощность фазы А панели
        /// </summary>
        public double ActivPowA{get;}
        
        /// <summary>
        /// активная мощность фазы В панели
        /// </summary>
        public double ActivPowB{get;} 
        
        /// <summary>
        /// активная мощность фазы С панели
        /// </summary>
        public double ActivPowC{get;}

        /// <summary>
        /// ток фазы А панели
        /// </summary>
        public double TokA{get;}
        
        /// <summary>
        /// ток фазы В панели
        /// </summary>
        public double TokB{get;}
        
        /// <summary>
        /// ток фазы С панели
        /// </summary>
        public double TokC{get;}


        /// <summary>
        /// суммарная полная/кажущаяся мощность всех одно и трехфазных элементов одной панели
        /// </summary>
        public double ApparentPowerSum{get;}
        
        /// <summary>
        /// суммарная активная мощность всех одно и трехфазных элементов одной панели
        /// </summary>
        public double ActivPowerSum{get;}
        
        /// <summary>
        /// cos всей панели, как отношение суммарной активной мощности к суммарной полной мощности
        /// </summary>
        public double CosSum{get;}
        
        /// <summary>
        /// tg всей панели, полученный из CosSum
        /// </summary>
        public double TgSum{get;}
        
        /// <summary>
        /// суммарная реактивная мощность всех одно и трехфазных элементов одной панели
        /// </summary>
        public double ReactivPowerSum{get;}

       /// <summary>
       /// трехфазный ток по максимально загруженной фазе всех одно и трехфазных элементов одной панели
       /// </summary>
        public double TokFromMaxFaz{get;}
        
        /// <summary>
        /// XYZ под семейством с минимальной координатой X в панели
        /// </summary>
        public XYZ LocationXYZ{get;}

        /// <summary>
        /// процентное соотношение токов фаз А и В
        /// </summary>
        public double PercentAB{get;}
        
        /// <summary>
        /// процентное соотношение токов фаз А и С
        /// </summary>
        public double PercentAC{get;}
        
        /// <summary>
        /// процентное соотношение токов фаз В и С
        /// </summary>
        public double PercentBC{get;}


        /// <summary>
        /// Результат расчета всех параметров сборки панели
        /// </summary>
        /// <param name="pA"></param>
        /// <param name="pB"></param>
        /// <param name="pC"></param>
        /// <param name="tA"></param>
        /// <param name="tB"></param>
        /// <param name="tC"></param>
        /// <param name="apparent"></param>
        /// <param name="active"></param>
        /// <param name="cos"></param>
        /// <param name="tg"></param>
        /// <param name="reactive"></param>
        /// <param name="maxTok"></param>
        /// <param name="location"></param>
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
