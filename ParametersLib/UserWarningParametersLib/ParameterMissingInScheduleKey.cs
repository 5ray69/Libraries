using Autodesk.Revit.DB;
using LevelsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametersLib.UserWarningParametersLib
{
    public class ParameterMissingInScheduleKey
    {
        public string MessageForUser(string nameScheduleKey, string nameParameter)
        {

            string message = $@"
Отсутствует параметр
{nameParameter}

у ключевой cпецификации с именем:
{nameScheduleKey}

Обратитесь к координатору, чтоб параметр
был добавлен в ключевую спецификацию.

После появления параметра в ключевой спецификации
запустите код заново.";

            return message;
        }
    }
}
