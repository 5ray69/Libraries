namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Нет такого ключа в ключевой спецификации
    /// </summary>
    public class NoElementKey
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="nameElement"></param>
        /// <param name="nameSchedule"></param>
        /// <returns></returns>
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
