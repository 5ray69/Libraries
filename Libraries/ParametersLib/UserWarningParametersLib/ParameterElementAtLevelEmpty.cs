using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Не заполнен параметер
    /// </summary>
    public class ParameterElementAtLevelEmpty
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string MessageForUser(Document doc, Element element, string parameterName)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Не заполнен параметер
{parameterName}

у элемента: {element.Name}
с Id: {element.Id.ToString}
на уровне: {levelAnyObject.GetLevel(element)?.Name?? "уровень не определен"}

Заполните параметр
и запустите код заново.
";

            return message;
        }
    }
}
