using Autodesk.Revit.DB;
using LevelsLib;
using System.Collections.Generic;
using System.Text;

namespace ElectricsLib.UserWarningElectricsLib
{
    public class ErrorConnectEquipment
    {
        public string MessageForUser(Document doc, FamilyInstance baseEquipment, List<FamilyInstance> loadsMismatch)
        {
            LevelAnyObject levelAnyObject = new(doc);
            StringBuilder stringBuilder = new();


            string message1 = $@"
У панели
с именем: {baseEquipment.Symbol.FamilyName}
с Id: {baseEquipment.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(baseEquipment).Name}
в параметре Обозначение цепей установлено значение По проекту.

Когда значение По проекту, то нагрузкой должен быть
только элемент категории Электрооборудование
(например, соединительная коробка).

Розетки, выключатели и светильники напрямую к щиту
не должны быть подключены, они подключаются к щиту только
через соединительную коробку (категория Электрооборудование).
У соединительной коробки в параметре Обозначение цепей
должно быть установлено значение С префиксами.
У щита значение По проекту.

Исправьте подключение и запустите код заново.

";
            stringBuilder.Append(message1);

            stringBuilder.AppendLine($"Элементы с отличающейся категорией:");

            foreach (var load in loadsMismatch)
            {
                stringBuilder.AppendLine($"категория: {load.Category.Name}   Id: {load.Id.IntegerValue}   уровень: {levelAnyObject.GetLevel(load).Name}");
            }

            return stringBuilder.ToString();
        }
    }
}
