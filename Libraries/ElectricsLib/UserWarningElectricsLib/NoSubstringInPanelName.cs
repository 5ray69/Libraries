namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// В проекте нет панели с подстрокой в имени
    /// </summary>
    public class NoSubstringInPanelName
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="substringInName"></param>
        /// <returns></returns>
        public string MessageForUser(string substringInName)
        {
            string message = $@"
В проекте нет панели с подстрокой в имени
'{substringInName}'.

Разместите семейство с такой подстрокой
в имени или измените имя панели
существующего семейства.";

            return message;
        }
    }
}
