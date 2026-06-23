namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Активный вид не является разрезом
    /// </summary>
    public class ActiveViewNotViewSection
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Активный вид не является разрезом.

Перейдите на вид разреза,
щелкните левой кнопкой мыши
на виде разреза, это активирует вид разреза
и тогда запустите код.

Код работает на вид разреза.";

            return message;
        }
    }
}
