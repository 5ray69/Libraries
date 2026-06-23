using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Некорректное имя панели
    /// </summary>
    public class IncorrectNumberOfLetters
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="baseEquipment"></param>
        /// <returns></returns>
        public string MessageForUser(Document doc, FamilyInstance baseEquipment)
        {
            LevelAnyObject levelAnyObject = new(doc);


            string message = $@"
У панели
с именем: {baseEquipment.Symbol.FamilyName}
с Id: {baseEquipment.Id.ToString}
на уровне: {levelAnyObject.GetLevel(baseEquipment).Name}
в параметре Имя панели,
после последней точки,
больше или меньше двух сиволов.

Имя панели должно содержать название группы точка этаж из двух цифр.
Например, гр.1.01 здесь .01 это первый этаж.
Например, гр.1.U1 здесь .U1 это первый уровень подвала.
Например, гр.1.R1 здесь .R1 это первый уровень кровли.

Исправьте значение параметра Имя панели
и запустите код заново.
";

            return message;
        }
    }
}
