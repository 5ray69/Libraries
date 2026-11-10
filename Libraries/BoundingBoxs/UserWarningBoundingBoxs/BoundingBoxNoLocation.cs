using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;

namespace IntersectionOfHoles.UserWarningIntersectionOfHoles
{
    /// <summary>
    /// У семейства нет баундингбокса
    /// </summary>
    public class BoundingBoxNoLocation(Document doc, ErrorModel errorModel)
    {
        private readonly LevelAnyObject _levelAnyObject = new(doc);
        private readonly Document _doc = doc;
        private readonly ErrorModel _errorModel = errorModel;



        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(FamilyInstance familyInstance)
        {
            Level level;  // объявляем переменную level, не назначив значение

            if (familyInstance.Host != null)  // если заглушка имеет значение параметра Рабочая плоскость
            {
                level = _levelAnyObject.GetLevel(familyInstance);
                if (level == null)
                {
                    _errorModel.UserWarning(new FamilyHasNoLevel().MessageForUser(familyInstance)); // завершаем код
                }
            }
            else  // (familyInstance.Host == null) если у заглушки значение параметра Рабочая плоскость 'не связано'
            {
                level = _levelAnyObject.GetСlosestLevel(familyInstance);
            }


            string message = $@"
У семейства с именем:
{familyInstance.Name}

имя семейства:
{familyInstance.Symbol.FamilyName}

Id семейства: {familyInstance.Id}

уровень семейства: {level?.Name ?? "Не найден"}

нет баундингбокса.
";

            return message;
        }

    }
}
