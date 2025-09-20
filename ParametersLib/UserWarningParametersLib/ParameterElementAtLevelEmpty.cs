using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    public class ParameterElementAtLevelEmpty
    {
        public string MessageForUser(Document doc, Element element, string parameterName)
        {
            LevelAnyObject levelAnyObject = new(doc);
            string message = $@"
Не заполнен параметер
{parameterName}

у элемента: {element.Name}
с Id: {element.Id}
на уровне: {levelAnyObject.GetLevel(element).Name}

Заполните параметр
и запустите код заново.
";

            return message;
        }
    }
}
