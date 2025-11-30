using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace CalculationGroups.MyDll.UserWarningCalculationGroups
{
    public class СyclicDependenceOfPanels
    {
        public string MessageForUser(Document doc, FamilyInstance familyInstance, string groupStr)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Обнаружена циклическая зависимость при построении цепей
в группе {groupStr}
.
у панели: {familyInstance.Symbol.FamilyName}
с Id: {familyInstance.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(familyInstance).Name}

Разорвите закольованное соединение так,
чтоб все соединения панелей имели одну головную панель,
являющуюся началом всех соединений.

После того как исправите соединения электрическими цепями
запустите код заново.
";

            return message;
        }



        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Обнаружена циклическая зависимость при построении цепей

у панели: {familyInstance.Symbol.FamilyName}
с Id: {familyInstance.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(familyInstance).Name}

Разорвите закольованное соединение так,
чтоб все соединения панелей имели одну головную панель,
являющуюся началом всех соединений.

После того как исправите соединения электрическими цепями
запустите код заново.
";

            return message;
        }
    }
}
