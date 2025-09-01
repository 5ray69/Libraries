using Autodesk.Revit.DB;
using LevelsLib;

namespace ElectricsLib.UserWarningElectricsLib
{
    public class NotPointInPanelName
    {
        public string MessageForUser(Document doc, FamilyInstance baseEquipment)
        {
            LevelAnyObject levelAnyObject = new(doc);


            string message = $@"
У панели
с именем: {baseEquipment.Symbol.FamilyName}
с Id: {baseEquipment.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(baseEquipment).Name}
в параметре Имя панели отсутствует точка.

Имя панели должно содержать название группы точка этаж из двух цифр.
Например, гр.1.01 здесь .01 это первый этаж.

Исправьте значение параметра Имя панели
и запустите код заново.
";

            return message;
        }
    }
}
