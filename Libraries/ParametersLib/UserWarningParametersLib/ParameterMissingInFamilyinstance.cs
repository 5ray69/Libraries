using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// У пользовательского семейства отсутствует параметр
    /// </summary>
    public class ParameterMissingInFamilyinstance
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyInstance"></param>
        /// <param name="nameParameter"></param>
        /// <returns></returns>
        public string MessageForUser(Document doc, FamilyInstance familyInstance, string nameParameter)
        {
            LevelAnyObject levelAnyObject = new(doc);

            string message = $@"
Отсутствует параметр
{nameParameter}

у элемента с именем:
{familyInstance.Name}

имя семейства элемента:
{familyInstance.Symbol.FamilyName}

Id элемента: {familyInstance.Id}

уровень элемента: {new LevelAnyObject(doc).GetLevel(familyInstance)?.Name ?? "у семейства нет уровня, оно или вложено в родительское или размещено на грани"}

Обратитесь к координатору, чтоб параметры
были добавлены в семейства.

После появления параметров в семействах
запустите код заново.";

            return message;
        }
    }
}
