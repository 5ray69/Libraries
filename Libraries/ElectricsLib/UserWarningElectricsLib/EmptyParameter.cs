using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class EmptyParameter
    {
        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Не заполнен параметер Имя панели
у элемента: {familyInstance.Symbol.FamilyName}
с Id: {familyInstance.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(familyInstance).Name}

Имя панели должно содержать название группы точка этаж из двух цифр.
Например, гр.1.01 здесь .01 это первый этаж.

Исправьте значение параметра Имя панели
и запустите код заново.
";

            return message;
        }
    }
}
