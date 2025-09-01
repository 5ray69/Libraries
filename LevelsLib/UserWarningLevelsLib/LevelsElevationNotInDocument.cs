using System.Collections.Generic;

namespace LevelsLib.UserWarningLevelsLib
{
    public class LevelsElevationNotInDocument
    {
        public string MessageForUser(ICollection<string> noLevelNames)
        {
            string message = $@"
В основном файле отсутствуют следующие
уровни c высотными отметками,
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
