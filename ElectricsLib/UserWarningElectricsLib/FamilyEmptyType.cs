using Autodesk.Revit.DB;
using LevelsLib;

namespace RenameCircuit.MyDll.UserWarningStrings
{
    public class FamilyEmptyType
    {
        public string MessageForUser(Element element, Document doc)
        {
            // у семейства не может быть уровня, только у разещенных экземпляров

            string message = $@"
Имя семейства:
{element.Name}

Id семейства: {element.Id.IntegerValue}

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
