namespace Libraries.LinksLib.UserWarningLinksLib
{
    /// <summary>
    /// В проекте нет ни одного экземпляра связи
    /// </summary>
    public class NoLinks
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
В проекте нет ни одного экземпляра связи.
Если они выгружены в диспетчере проекта,
то щелкните по связи правой кнопкой мыши
и выберите Обновить.";

            return message;
        }
    }
}
