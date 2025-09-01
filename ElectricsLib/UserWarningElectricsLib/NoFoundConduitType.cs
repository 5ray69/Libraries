using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricsLib.UserWarningElectricsLib
{
    public class NoFoundConduitType
    {

        /// <summary>
        /// в качестве аргумента метода - имя типоразмера
        /// </summary>
        /// <param name="conduitTypeName"></param>
        /// <returns></returns>
        public string MessageForUser(string familyName, string conduitTypeName)
        {
            string message = $@"
У семейства с именем:
{familyName}

тиморазмер с именем:
{conduitTypeName}
не найден.

Создайте типоразмер с именем:
{conduitTypeName}
в указанном выше семействе
и запустите код заново.
";

            return message;
        }
    }
}
