using Autodesk.Revit.DB;

namespace Libraries.Collectors.UserWarningCollectors
{
    /// <summary>
    /// У FamilyInstance не найден уровень
    /// </summary>
    public class FamilyHasNoLevel
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
Не найден уровень
у элемента с именем:
{familyInstance.Name}

имя семейства элемента:
{familyInstance.Symbol.FamilyName}

Id элемента: {familyInstance.Id}

Обратитесь к координатору
для исправления ошибки в семействе или в коде.
";

            return message;
        }
    }
}
