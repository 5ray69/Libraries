namespace Libraries.LevelsLib.UserWarningLevelsLib
{
    /// <summary>
    /// В активном документе отсутствуют уровни
    /// </summary>
    public class LevelsNotInProject
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <returns></returns>
        public string MessageForUser()
        {
            string message = $@"
В активном документе отсутствуют уровни.

Или открыт документ без уровней
или вы не создали уровни в проекте.

После исправления ошибки
запустите код заново.";

            return message;
        }
    }
}
