using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    public class ParameterWithoutValue
    {
        public string MessageForUser(string nameParameter, Element element)
        {
            string message = $@"
Не заполнен параметер {nameParameter}
у элемента: {element.Name}
с Id: {element.Id.IntegerValue}

Заполните значение параметра {nameParameter}
и запустите код заново.
";

            return message;
        }
    }
}
