using Autodesk.Revit.DB;

namespace ElectricsLib.UserWarningStrings
{
    public class NoElementKeyId
    {
        public string MessageForUser(ElementId elementId, string nameSchedule)
        {
            string message = $@"
Элемент {elementId}
не найден в спецификации с именем
{nameSchedule}

Создайте такой элемент в указанной спецификации
И запустите код заново.
";

            return message;
        }
    }
}
