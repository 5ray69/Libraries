namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class DuplicateGroups
    {
        public string MessageForUser(string stringWarning)
        {
            string message = $@"
Внимание!
Есть группы с одинаковым именем.
{stringWarning}
 

Или соедините их в одну группу,
или задайте им различные имена,
и после этого запустите код заново.
";
            return message;
        }
    }
}
