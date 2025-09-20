using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class ByProjectCircuitNaming
    {

        public string MessageForUser(Document doc, FamilyInstance baseEquipment, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);


            string message = $@"
У панели
с именем: {baseEquipment.Name}
с Id: {baseEquipment.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(baseEquipment).Name}
Обозначением цепей установлено По проекту.

Когда у панели Обозначение цепей По проекту,
то к ней должны подключаться только другие панели.
Розетки, выключатели, светильники, все что не категория Электрооборудование,
должны подключаться к такой панели
только через соединительную коробку (дозу).

Или замените Обзначение цепей на С префиксами,
или подключите нагрузку
с именем: {familyInstance.Name}
с Id: {familyInstance.Id.IntegerValue}
на уровне: {levelAnyObject.GetLevel(familyInstance).Name}
через соединительную коробку (дозу)
и запустите код заново.
";

            return message;
        }

    }
}
