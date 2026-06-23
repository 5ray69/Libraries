using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Поставлены галки сразу у нескольких фаз
    /// </summary>
    public class MultiplePhases
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
Поставлены галки сразу у нескольких фаз
при напряжении 220В в семействе:

{familyInstance.Symbol.Family.Name}

с типоразмером: {familyInstance.Name}

c Id: {familyInstance.Id}

Уберите лишние галки в семействе
и запустите код заново.";

            return message;
        }
    }
}
