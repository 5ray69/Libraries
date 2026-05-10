using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class MultiplePhases
    {
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
Поставлены галки сразу у нескольких фаз
при напряжении 220В в семействе:

{familyInstance.Symbol.Family.Name}

с типоразмером: {familyInstance.Name}

c Id: {familyInstance.Id.IntegerValue}

Уберите лишние галки в семействе
и запустите код заново.";

            return message;
        }
    }
}
