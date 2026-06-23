using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// У элемента нет свойства Location
    /// </summary>
    public class NoContainLocationElectricsLib
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string MessageForUser(Element element)
        {
            string message = $@"
У элемента с именем:
{element.Name}

Id этого элемента: {element.Id}

нет свойства Location.

class LocationAnyObject
";

            return message;
        }
    }
}
