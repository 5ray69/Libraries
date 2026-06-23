using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Ошибка в простановке галок у параметров фаз
    /// </summary>
    public class SetPhasesIn380
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
Поставлена галка фазы
при напряжении 380В в семействе:

{familyInstance.Symbol.Family.Name}

с типоразмером: {familyInstance.Name}

c Id: {familyInstance.Id}

Уберите все галки у фазы А, фазы B и фазы C
и запустите код заново.";

            return message;
        }
    }
}
