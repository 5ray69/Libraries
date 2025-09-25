namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NoFamilyContainString
    {
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
