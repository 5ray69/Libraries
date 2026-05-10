namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NoFamilyName
    {
        public string MessageForUser(string familyName)
        {
            string message = $@"
В проекте нет семейства с именем
{familyName}.

Загрузите семейство с таким именем или измените
имя существующего семейства на соответствующее.";

            return message;
        }
    }
}
