namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class ActiveViewNotViewDrafting
    {
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
