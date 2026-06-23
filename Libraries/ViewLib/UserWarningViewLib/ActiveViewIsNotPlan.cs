namespace Libraries.ViewLib.UserWarningViewLib
{
    /// <summary>
    /// Активный вид не является планом.
    /// </summary>
    public class ActiveViewIsNotPlan
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Активный вид не является планом.

Откройте план этажа и запустите код заново.
";

            return message;
        }
    }
}
