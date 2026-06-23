using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Отсутствует параметр
    /// </summary>
    public class ParameterElementMissing
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="el"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public string MessageForUser(Element el, string str)
        {
            string message = $@"
Отсутствует параметр
{str}

у элемента с именем:
{el.Name}

c Id элемента:
{el.Id.ToString}

Обратитесь к координатору, чтоб параметры
были добавлены в семейства.

После появления параметров в семействах
запустите код заново.";

            return message;
        }
    }
}
