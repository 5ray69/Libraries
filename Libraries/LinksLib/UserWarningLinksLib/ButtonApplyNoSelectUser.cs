namespace Libraries.LinksLib.UserWarningLinksLib
{
    /// <summary>
    /// Пользователь нажал кнопку Применить, не сделав выбор
    /// </summary>
    public class ButtonApplyNoSelectUser
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Пользователь нажал кнопку Применить, не сделав выбор.

Закройте это окно и запустите код заново,
но уже сделав выбор в окне,
перед тем как нажимать кнопку Применить.";

            return message;
        }
    }
}
