namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Активный вид не является чертежным видом
    /// </summary>
    public class ActiveViewNotViewDrafting
    {
        /// <summary>
        /// сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Активный вид не является чертежным видом.

Откройте чертежный вид, щелкните
левой кнопкой мыши на чертежном виде,
это активирует чертежный вид,
и тогда запустите код.

Код должен работать на чертежный вид.";

            return message;
        }
    }
}
