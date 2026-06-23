using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// у семейства нет типоразмера
    /// </summary>
    public class FamilyEmptyType
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="element"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string MessageForUser(Element element, Document doc)
        {
            // у семейства не может быть уровня, только у разещенных экземпляров

            string message = $@"
Имя семейства:
{element.Name}

Id семейства: {element.Id.ToString}

Уровень элемента: {new LevelAnyObject(doc).GetLevel(element)?.Name ?? "у семейства нет уровня, есть только у размещенных экземпляров семейства"}

Суть ошибки - у семейства нет типоразмера.

Обратитесь к координатору, чтобы он добавил
хотя бы один типоразмер в это семейство и после того,
как типоразмер будет добавлен, код можно будет запустить заново.
";
            return message;
        }
    }
}
