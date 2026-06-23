namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// В проекте нет семейства содержащего строку
    /// </summary>
    public class NoFamilyContainString
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="stringFamilyName"></param>
        /// <returns></returns>
        public string MessageForUser(string stringFamilyName)
        {
            string message = $@"
В проекте нет семейства содержащего строку
{stringFamilyName}.

Стояки строятся именно из этих семейств.

Загрузите семейство в проект.";

            return message;
        }
    }
}
