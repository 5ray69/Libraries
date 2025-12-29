using Autodesk.Revit.DB;

namespace CreateShemPIcosf.MyDll.UserWarning
{
    public class SetPhasesIn380
    {
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
Поставлена галка фазы
при напряжении 380В в семействе:

{familyInstance.Symbol.Family.Name}

с типоразмером: {familyInstance.Name}

c Id: {familyInstance.Id.IntegerValue}

Уберите все галки у фазы А, фазы B и фазы C
и запустите код заново.";

            return message;
        }
    }
}
