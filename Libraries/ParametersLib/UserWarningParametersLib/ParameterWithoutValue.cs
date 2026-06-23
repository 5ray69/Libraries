using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Не заполнен параметер
    /// </summary>
    public class ParameterWithoutValue
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="nameParameter"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public string MessageForUser(string nameParameter, Element element)
        {
            string message = $@"
Не заполнен параметер {nameParameter}
у элемента: {element.Name}
с Id: {element.Id.ToString}

Заполните значение параметра {nameParameter}
и запустите код заново.
";

            return message;
        }
    }
}
