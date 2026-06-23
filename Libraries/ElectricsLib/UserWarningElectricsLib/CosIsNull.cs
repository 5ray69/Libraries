using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// cosφ равен нулю в семействе
    /// </summary>
    public class CosIsNull
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
BDV_E000_cosφ равен нулю в семействе:

{familyInstance.Symbol.Family.Name}

с типоразмером: {familyInstance.Name}

c Id: {familyInstance.Id}

Установите значение BDV_E000_cosφ отличное от нуля
и запустите код заново.";

            return message;
        }
    }
}
