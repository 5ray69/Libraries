namespace CalculationGroups.MyDll
{
    public class FamilySymbolNameNotInProject
    {
        public string MessageForUser(string nameFamilySymbol)
        {
            string message = $@"
Семейства с именем типоразмера
'{nameFamilySymbol}'
нет в проекте.

Обратитесь к координатоу, чтоб он загрузил
соответсвующее семейство в проект.
После того как семейство будет загружено
в проект запустите код заново.
";

            return message;
        }
    }
}
