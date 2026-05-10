namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NoSubstringInPanelName
    {
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
