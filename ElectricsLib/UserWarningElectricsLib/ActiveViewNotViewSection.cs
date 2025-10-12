namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class ActiveViewNotViewSection
    {
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
