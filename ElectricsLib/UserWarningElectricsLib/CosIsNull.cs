using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class CosIsNull
    {
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
BDV_E000_cosφ равен нулю в семействе:

{familyInstance.Symbol.Family.Name}

с типоразмером: {familyInstance.Name}

c Id: {familyInstance.Id.IntegerValue}

Установите значение BDV_E000_cosφ отличное от нуля
и запустите код заново.";

            return message;
        }
    }
}
