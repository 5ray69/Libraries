using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NoTargetCategory
    {
        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);

            string message = $@"
Семейство с именем
{familyInstance.Name}

с Id
{familyInstance.Id.IntegerValue}

размещенное на уровне
{levelAnyObject.GetLevel(familyInstance)}

не является категорией Электрооборудование.
";
            return message;
        }
    }
}
