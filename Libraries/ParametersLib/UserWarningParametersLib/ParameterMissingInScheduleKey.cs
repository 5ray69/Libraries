namespace Libraries.ParametersLib.UserWarningParametersLib
{
    /// <summary>
    /// Отсутствует параметр в ключевой cпецификации
    /// </summary>
    public class ParameterMissingInScheduleKey
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="nameScheduleKey"></param>
        /// <param name="nameParameter"></param>
        /// <returns></returns>
        public string MessageForUser(string nameScheduleKey, string nameParameter)
        {

            string message = $@"
Отсутствует параметр
{nameParameter}

у ключевой cпецификации с именем:
{nameScheduleKey}

Обратитесь к координатору, чтоб параметр
был добавлен в ключевую спецификацию.

После появления параметра в ключевой спецификации
запустите код заново.";

            return message;
        }
    }
}
