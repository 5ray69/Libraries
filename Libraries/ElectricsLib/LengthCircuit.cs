using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// Возвращает значения в зависимости от длины цепи, идущей от ЩЭ до ЩК
    /// </summary>
    public class LengthCircuit
    {
        /// <summary>
        /// цвет марки панели черный
        /// </summary>
        /// <param name="leng"></param>
        /// <returns></returns>
        public int ForBlackTag(double leng)
        {
            if (leng < 25)
                return 1;
            else if (leng >= 25 && leng < 40)
                return 0;
            else //оставшиеся варианты (leng >= 40)
                return 0;
        }

        /// <summary>
        /// цвет марки панели красный
        /// </summary>
        /// <param name="leng"></param>
        /// <returns></returns>
        public int ForRedTag(double leng)
        {
            if (leng < 25)
                return 0;
            else if (25 <= leng && leng < 40)
                return 1;
            else //оставшиеся варианты (leng >= 40)
                return 0;
        }

        /// <summary>
        /// цвет марки панели зеленый
        /// </summary>
        /// <param name="leng"></param>
        /// <returns></returns>
        public int ForGreenTag(double leng)
        {
            if (leng < 25)
                return 0;
            else if (25 <= leng && leng < 40)
                return 0;
            else //оставшиеся варианты (leng >= 40)
                return 1;
        }

        public ElementId ElementIdCableForLength(Document doc, double leng)
        {
            ScheduleKey scheduleKeyInMethod = new(doc, "Ключи спецификация кабелей");

            if (leng < 25)
                return scheduleKeyInMethod.GetIdElementKey("ВВГнг-LS 3х10");
            else if (25 <= leng && leng < 40)
                return scheduleKeyInMethod.GetIdElementKey("ВВГнг-LS 3х16");
            else //оставшиеся варианты (leng >= 40)
                return scheduleKeyInMethod.GetIdElementKey("ВВГнг-LS 3х25");
        }
    }
}
