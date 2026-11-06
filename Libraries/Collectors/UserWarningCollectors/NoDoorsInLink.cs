namespace Libraries.Collectors.UserWarningCollectors
{
    /// <summary>
    /// Если в связанном файле не найдено ни одной двери
    /// </summary>
    public class NoDoorsInLink
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Код не был выполнен и завершил работу.
В связанном файле не найдено ни одной двери.
";

            return message;
        }
    }
}
