namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Семейства нет в проекте
    /// </summary>
    public class FamilySymbolNameNotInProject
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="nameFamilySymbol"></param>
        /// <returns></returns>
        public string MessageForUser(string nameFamilySymbol)
        {
            string message = $@"
Семейства с именем типоразмера
'{nameFamilySymbol}'
нет в проекте.

Обратитесь к координатоу, чтоб он загрузил
соответсвующее семейство в проект.
После того как семейство будет загружено
в проект запустите код заново.
";

            return message;
        }
    }
}
