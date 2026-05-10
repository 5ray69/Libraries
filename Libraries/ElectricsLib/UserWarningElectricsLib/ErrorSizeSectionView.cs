using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class ErrorSizeSectionView
    {
        public string MessageForUser(Document doc, FamilyInstance familyInstance)
        {
            LevelAnyObject levelAnyObject = new(doc);

            string message = $@"
Уменьшите ширину разреза так,
чтобы он захватывал только те дозы, 
между которыми строятся стояки. 
Сейчас попадает в разрез 
{familyInstance.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString()}

имя дозы/имя панели
{familyInstance.Name}

Id дозы
{familyInstance.Id.IntegerValue}

доза размещенная на уровне
{levelAnyObject.GetLevel(familyInstance).Name}

Это приведет к циклической ссылке. 
Ревит видит семейства не так как пользователь,
возможно попадание невидимых для пользователя
частей элемента в границы разреза.
Уменьшите ширину разреза и запустите код заново.

Можно попробовать скрыть дозу на разрезе
с помощью правой кнопки мыши
Скрыть при просмотре - Элементы
";
            return message;
        }
    }
}
