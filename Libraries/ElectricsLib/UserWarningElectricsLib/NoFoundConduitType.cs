namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Нет типоразмера в семействе
    /// </summary>
    public class NoFoundConduitType
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="conduitTypeName"></param>
        /// <returns></returns>
        public string MessageForUser(string familyName, string conduitTypeName)
        {
            string message = $@"
У семейства с именем:
{familyName}

тиморазмер с именем:
{conduitTypeName}
не найден.

Создайте типоразмер с именем:
{conduitTypeName}
в указанном выше семействе
и запустите код заново.
";

            return message;
        }
    }
}
