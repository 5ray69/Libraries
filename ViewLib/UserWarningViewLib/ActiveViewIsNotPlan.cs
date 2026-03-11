namespace Libraries.ViewLib.UserWarningViewLib
{
    public class ActiveViewIsNotPlan
    {
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
