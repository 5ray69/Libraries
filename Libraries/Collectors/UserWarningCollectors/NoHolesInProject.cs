namespace Libraries.Collectors.UserWarningCollectors
{
    /// <summary>
    /// В проекте не найдено ни одной заглушки/отверстия
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
