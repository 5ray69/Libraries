namespace Libraries.ViewLib.UserWarningViewLib
{
    public class ButtonIsPressedNoSelectUser
    {
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
