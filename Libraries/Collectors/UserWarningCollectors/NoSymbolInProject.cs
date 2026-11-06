namespace Libraries.Collectors.UserWarningCollectors
{
    /// <summary>
    /// Если типоразмер не найден в проекте
    /// </summary>
    public class NoSymbolInProject
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public string MessageForUser(string familyName, string builtInCategory)
        {
            string message = $@"
Не найден типоразмер семейства
'{familyName}'

в категории
{builtInCategory}

Загрузите семейство c типоразмером
в проект и запустите код заново.
";

            return message;
        }
    }
}
