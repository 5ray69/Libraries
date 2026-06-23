using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Отсутствует параметр
    /// </summary>
    public class ParameterIsMissing
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="el"></param>
        /// <param name="nameParameter"></param>
        /// <returns></returns>
        public string MessageForUser(Element el, string nameParameter)
        {
            string message = $@"
Отсутствует параметр
{nameParameter}

у элемента с именем:
{el.Name}

c Id элемента:
{el.Id}

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
