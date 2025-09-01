using Autodesk.Revit.DB;

namespace LocationLib.UserWarningRoomsLib
{
    public class NoContainLocation
    {
        public string MessageForUser(Element element)
        {
            string message = $@"
У элемента с именем:
{element.Name}

Id этого элемента: {element.Id.IntegerValue}

нет свойства Location.

class LocationAnyObject
";

            return message;
        }
    }
}
