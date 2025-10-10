using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    public class ParameterIsMissing
    {
        public string MessageForUser(Element el, string nameParameter)
        {
            string message = $@"
Отсутствует параметр
{nameParameter}

у элемента с именем:
{el.Name}

c Id элемента:
{el.Id.IntegerValue}

категория элемента:
{el.Category?.Name ?? "у элемента нет категории"}

Обратитесь к координатору, чтоб параметры
были добавлены в семейства.

После появления параметров в семействах
запустите код заново.";

            return message;
        }
    }
}
