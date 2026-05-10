using System.Collections.Generic;

namespace Libraries.LevelsLib.UserWarningLevelsLib
{
    public class LevelsNotInDocument
    {
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
