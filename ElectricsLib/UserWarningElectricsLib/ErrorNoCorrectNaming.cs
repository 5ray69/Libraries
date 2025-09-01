using Autodesk.Revit.DB;
using LevelsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricsLib.UserWarningElectricsLib
{
    public class ErrorNoCorrectNaming
    {
        public string MessageForUser(Document doc, FamilyInstance baseEquipment)
        {
            LevelAnyObject levelAnyObject = new(doc);

            string message = $@"
У панели
с именем: {baseEquipment.Symbol.FamilyName}
с Id: {baseEquipment.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(baseEquipment).Name}
в параметре Обозначение цепей установлено значение 
и не По проекту, и не С префиксами.

Должно быть или По проекту, или С префиксами.

Когда значение По проекту, то нагрузкой должен быть
только элемент категории Электрооборудование
(например, соединительная коробка).

Розетки, выключатели и светильники напрямую к щиту
не должны быть подключены, они подключаются к щиту только
через соединительную коробку (категория Электрооборудование).

У соединительной коробки в параметре Обозначение цепей
должно быть установлено значение С префиксами.
У щита значение По проекту.

Исправьте значение параметра Обозначение цепей
и запустите код заново.

";
            return message;
        }
    }
}
