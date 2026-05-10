using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NoContainLocationElectricsLib
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
