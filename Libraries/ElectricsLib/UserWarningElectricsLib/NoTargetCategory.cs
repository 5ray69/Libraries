using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Не является категорией Электрооборудование
    /// </summary>
    public class NoTargetCategory
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
Семейство с именем
{familyInstance.Name}

с Id
{familyInstance.Id.ToString}

размещенное на уровне
{levelAnyObject.GetLevel(familyInstance)}

не является категорией Электрооборудование.
";
            return message;
        }
    }
}
