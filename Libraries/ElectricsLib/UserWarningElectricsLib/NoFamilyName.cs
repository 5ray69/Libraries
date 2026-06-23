namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// В проекте нет семейства с именем
    /// </summary>
    public class NoFamilyName
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public string MessageForUser(string familyName)
        {
            string message = $@"
В проекте нет семейства с именем
{familyName}.

Загрузите семейство с таким именем или измените
имя существующего семейства на соответствующее.";

            return message;
        }
    }
}
