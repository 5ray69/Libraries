using Autodesk.Revit.DB.Electrical;

namespace Libraries.LevelsLib.UserWarningLevelsLib
{
    /// <summary>
    /// Цепь не подключена
    /// </summary>
    public class NoConnectCircuit
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <returns></returns>
        public string MessageForUser(ElectricalSystem electricalSystem)
        {
            string message = $@"
Электрическая цепь,
Id которой {electricalSystem.Id}
не подключена.

Удалите все неподключенные цепи или
подключите каждую из них к панели/щиту.
И запустите код заново.

Неподключенные цепи можно найти
в Диспетчере инженерных систем (F9).
У всех неподключенных цепей будет <без имени>.
";

            return message;
        }
    }
}
