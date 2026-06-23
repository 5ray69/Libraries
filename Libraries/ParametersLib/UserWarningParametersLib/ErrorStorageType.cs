using Autodesk.Revit.DB;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Не соответствует тип данных параметра у копируемого и у целевого элемента
    /// </summary>
    public class ErrorStorageType
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="sourceElement"></param>
        /// <param name="targetElement"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string MessageForUser(Element sourceElement, Element targetElement, string parameterName)
        {
            string message = $@"
Не соответствует тип данных параметра
{parameterName}

у копируемого элемента с именем:
{sourceElement.Name}
c Id элемента:
{sourceElement.Id.ToString}
категория элемента:
{sourceElement.Category?.Name ?? "у элемента нет категории"}


и у целевого элемента с именем:
{targetElement.Name}
c Id элемента:
{targetElement.Id.ToString}
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
