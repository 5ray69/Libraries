using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Нет элемента с указанным ключом в спецификации
    /// </summary>
    public class NoElementKeyId
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="nameSchedule"></param>
        /// <returns></returns>
        public string MessageForUser(ElementId elementId, string nameSchedule)
        {
            string message = $@"
Элемент {elementId}
не найден в спецификации с именем
{nameSchedule}

Создайте такой элемент в указанной спецификации
И запустите код заново.
";

            return message;
        }
    }
}
