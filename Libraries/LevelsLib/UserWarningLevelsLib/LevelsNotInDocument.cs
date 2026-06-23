namespace Libraries.LevelsLib.UserWarningLevelsLib
{
    /// <summary>
    /// В основном файле отсутствуют уровни, которые есть в файле связи
    /// </summary>
    public class LevelsNotInDocument
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="noLevelNames"></param>
        /// <returns></returns>
        public string MessageForUser(ICollection<string> noLevelNames)
        {
            string message = $@"
В основном файле отсутствуют уровни,
которые есть в файле связи:
{string.Join(", ", noLevelNames)}

Обратитесь к координатору для
копирования недостающих уровней
из файла связи.

После исправления ошибки
запустите код заново.";

            return message;
        }
    }
}
