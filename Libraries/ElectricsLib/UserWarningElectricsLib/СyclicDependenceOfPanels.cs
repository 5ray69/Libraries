using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Циклическая зависимость при построении цепей
    /// </summary>
    public class СyclicDependenceOfPanels
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyInstance"></param>
        /// <param name="groupStr"></param>
        /// <returns></returns>
        public string MessageForUser(Document doc, FamilyInstance familyInstance, string groupStr)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Обнаружена циклическая зависимость при построении цепей
в группе {groupStr}
.
у панели: {familyInstance.Symbol.FamilyName}
с Id: {familyInstance.Id.ToString}
на уровне: {levelAnyObject.GetLevel(familyInstance).Name}

Разорвите закольованное соединение так,
чтоб все соединения панелей имели одну головную панель,
являющуюся началом всех соединений.

После того как исправите соединения электрическими цепями
запустите код заново.
";

            return message;
        }


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
Обнаружена циклическая зависимость при построении цепей

у панели: {familyInstance.Symbol.FamilyName}
с Id: {familyInstance.Id.ToString}
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
