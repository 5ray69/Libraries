using Autodesk.Revit.DB;

namespace ParametersLib.UserWarningParametersLib
{
    public class ParameterElementMissing
    {
        public string MessageForUser(Element el, string str)
        {
            string message = $@"
Отсутствует параметр
{str}

у элемента с именем:
{el.Name}

c Id элемента:
{el.Id.IntegerValue}

Обратитесь к координатору, чтоб параметры
были добавлены в семейства.

После появления параметров в семействах
запустите код заново.";

            return message;
        }
    }
}
