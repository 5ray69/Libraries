using Autodesk.Revit.DB;

namespace Libraries.RoomsLib.UserWarningRoomsLib
{
    /// <summary>
    /// У элемента нет свойства Location
    /// </summary>
    public class NoContainLocation
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

Id этого элемента: {element.Id.ToString}

нет свойства Location.

class LocationAnyObject
";

            return message;
        }
    }
}
