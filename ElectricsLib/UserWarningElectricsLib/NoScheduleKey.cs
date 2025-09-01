namespace ElectricsLib.MyDll.UserWarningStrings
{
    internal class NoScheduleKey
    {
        public string MessageForUser(string nameSchedule)
        {
            string message = $@"
Спецификация с именем
{nameSchedule}
не найдена.

Создайте ключевую спецификацию с именем
{nameSchedule}
И запустите код заново.
";

            return message;
        }
    }
}
