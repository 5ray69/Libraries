using Autodesk.Revit.DB;

namespace ParametersLib.UserWarningParametersLib
{
    public class ErrorStorageType
    {
        public string MessageForUser(Element sourceElement, Element targetElement, string parameterName)
        {
            string message = $@"
Не соответствует тип данных параметра
{parameterName}

у копируемого элемента с именем:
{sourceElement.Name}
c Id элемента:
{sourceElement.Id.IntegerValue}
категория элемента:
{sourceElement.Category?.Name ?? "у элемента нет категории"}


и у целевого элемента с именем:
{targetElement.Name}
c Id элемента:
{targetElement.Id.IntegerValue}
категория элемента:
{targetElement.Category?.Name ?? "у элемента нет категории"}

Обратитесь к координатору,
чтобы типы данных параметра привести в соответстие.

После приведения в соответстие
запустите код заново.";

            return message;
        }
    }

}
