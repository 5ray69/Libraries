using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NoDotLevelInNamePanel
    {
        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new LevelAnyObject(doc);

            string message = $@"
У семейства
с именем {familyInstance.Name}
категории {familyInstance.Category?.Name ?? "у элемента нет категории"}
с Id {familyInstance.Id.IntegerValue}
на уровне {levelAnyObject.GetLevel(familyInstance).Name}

в параметре Имя панели
либо отсутствует 'точка этаж',
либо 'точка этаж' не соответствует
уровню размещения семейства.
.";

            return message;
        }
    }
}
