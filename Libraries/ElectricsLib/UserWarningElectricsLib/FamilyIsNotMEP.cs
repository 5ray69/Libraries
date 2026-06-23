using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Семейство не является MEP оборудованием
    /// </summary>
    public class FamilyIsNotMEP
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Семейство: {familyInstance.Symbol.FamilyName}
категории {familyInstance.Category?.Name ?? "у элемента нет категории"}
с Id: {familyInstance.Id.ToString}
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
