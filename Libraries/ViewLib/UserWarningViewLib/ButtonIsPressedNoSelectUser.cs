namespace Libraries.ViewLib.UserWarningViewLib
{
    /// <summary>
    /// Пользователь нажал кнопку Выбрать, не сделав выбор
    /// </summary>
    public class ButtonIsPressedNoSelectUser
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Пользователь нажал кнопку Выбрать, не сделав выбор.

Закройте это окно и запустите код заново,
но уже сделав выбор в окне,
перед тем как нажимать кнопку Выбрать.";

            return message;
        }
    }
}
