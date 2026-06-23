using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Параметр только для чтения
    /// </summary>
    public class ParameterIsReadOnly
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
Параметр
{str}
только для чтения

у элемента с именем:
{el.Name}

c Id элемента:
{el.Id}

категория:
{el.Category?.Name ?? "у элемента нет категории"}

Обратитесь к координатору для восстановления параметра.

После исправления параметров можно будет
пользоваться кодом заново.";

            return message;
        }
    }
}
