using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace CalculationGroups.MyDll.UserWarningCalculationGroups
{
    public class FamilyIsNotMEP
    {
        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Семейство: {familyInstance.Symbol.FamilyName}
категории {familyInstance.Category?.Name ?? "у элемента нет категории"}
с Id: {familyInstance.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(familyInstance).Name}

Семейство не является MEP оборудованием.
Например, семейство категории Обобщенные модели.
Или у семейства отсутствует электрический соединитель.

Или не используйте это семейство в электрических цепях,
или поменяйте категорию семейства на MEP оборудование,
или разместите электрический соединитель в семействе.

После того как исправите запустите код заново.
";

            return message;
        }
    }
}
