namespace Libraries.Collectors.UserWarningCollectors
{
    /// <summary>
    /// Нет ни одного семейства заглушки
    /// </summary>
    public class NoHolesInProject
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
Код не был выполнен и завершил работу.
В проекте не найдено ни одной заглушки.
";

            return message;
        }
    }
}
