using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.ParametersLib.UserWarningParametersLib
{
    public class ParameterIsReadOnly
    {
        public string MessageForUser(Element el, string str)
        {
            string message = $@"
Параметр
{str}
только для чтения

у элемента с именем:
{el.Name}

c Id элемента:
{el.Id.IntegerValue}

категория:
{el.Category?.Name ?? "у элемента нет категории"}

Обратитесь к координатору для восстановления параметра.

После исправления параметров можно будет
пользоваться кодом заново.";

            return message;
        }
    }
}
