using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NotConvertToNumber
    {
        public string MessageForUser(Element element, string parameterName)
        {
            string message = $@"
У элемента: {element.Name}
c Id элемента: {element.Id.IntegerValue}

категория элемента:
{element.Category?.Name ?? "у элемента нет категории"}

Значение параметра
{parameterName}
не удалось привести к числовому типу данных.

Обратитесь к координатору,
чтобы значение параметра
{parameterName}
можно было привести к числу.

Возможно, на месте точки стоит запятая.

После приведения в соответстие
запустите код заново.";

            return message;
        }
    }
}
