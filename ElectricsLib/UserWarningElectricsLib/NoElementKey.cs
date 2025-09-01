namespace ElectricsLib.UserWarningElectricsLib
{
    public class NoElementKey
    {
        public string MessageForUser(string nameElement, string nameSchedule)
        {
            string message = $@"
Элемент {nameElement}
не найден в спецификации с именем
{nameSchedule}

Создайте такой элемент в указанной спецификации
И запустите код заново.
";

            return message;
        }
    }
}
