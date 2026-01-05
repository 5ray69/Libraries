using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;
using System;

namespace Libraries.LevelsLib
{
    public class LevelAnyObject(Document doc)
    {
        private readonly Document _doc = doc;


        /// <summary>
        /// <para>извлекает уровень, на котором находится любой объект электрика,
        /// <para>включая семейства на грани (не привязанные к уровню) и аннотации
        /// </summary>
        /// <returns></returns>
        public Level GetLevel(Element element)
        {
            //НЕЛЬЗЯ МЕНЯТЬ ПОРЯДОК СЛЕДОВАНИЯ УСЛОВИЙ if
            //некоторые if подпадают под условия друг друга (семейства имеющие Host и HostFace)


            // Проверка на наличие свойства LevelId и извлечение его значения
            //familyinstance, соед.детали кабельных лотков, соед.детали коробов
            // LevelId у тех, у кого семейство НЕ на основе рабочей плоскости/НЕ на грани
            if (element.LevelId != null && element.LevelId.IntegerValue != -1)
            {
                return _doc.GetElement(element.LevelId) as Level;
            }


            // HostFace is null у тех семейств, у которых рабочая плоскость это уровень, а не на грани геометрии
            // Светильники, выключатели, розетки... и здесь полоса заземления, которая размещена на уровне
            if (
                element is FamilyInstance familyInstance
                && familyInstance.HostFace is null
                && element.Category != null
                && element.Category.Id.IntegerValue != (int)BuiltInCategory.OST_DetailComponents  // элементы узлов
                && element.Category.Id.IntegerValue != (int)BuiltInCategory.OST_GenericAnnotation  // типовые аннотации (стрелка выносок)
                )
            {
                return familyInstance.Host as Level;
            }


            // короба свойство ReferenceLevel
            if (element is Conduit conduit && conduit.ReferenceLevel is Level refLevel)
            {
                return refLevel;
            }


            // кабельные лотки свойство ReferenceLevel
            if (element is CableTray cableTray && cableTray.ReferenceLevel is Level referenceLevel)
            {
                return referenceLevel;
            }


            if (element is ElectricalSystem electricalSystem)
            {
                var baseEquipment = electricalSystem.BaseEquipment;
                if (baseEquipment == null) // цепь не подключена "код завершил работу" с предупреждением пользователю
                {
                    ErrorModel errorModel = new();
                    errorModel.UserWarning(new NoConnectCircuit().MessageForUser(electricalSystem));
                }

                if (baseEquipment?.LevelId != null && baseEquipment.LevelId.IntegerValue != -1)
                {
                    return _doc.GetElement(baseEquipment.LevelId) as Level;
                }

                if (baseEquipment?.Host is Level baseHostLevel)
                {
                    return baseHostLevel;
                }
            }


            // полоса заземления и другие семейства, размещенные на грани
            // если element это FamilyInstance и его свойство HostFace возвращает тип Reference (не null) - если возвращает тип Reference значит размещено на грани
            // синтаксис if использует шаблоны с условиями C# (8.0 и выше)
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#property-patterns
            if (element is FamilyInstance { HostFace: Reference })
            {
                double zElement = 0;

                if (element.Location is LocationCurve locationCurve)
                {
                    //нижняя точка линии, та у которой значение высотной отметки меньше
                    double minZ = Math.Round(
                        Math.Min(locationCurve.Curve.GetEndPoint(0).Z, locationCurve.Curve.GetEndPoint(1).Z),
                        3, MidpointRounding.AwayFromZero);
                    zElement = minZ;
                }
                else if (element.Location is LocationPoint locationPoint)
                {
                    double zPoint = Math.Round(locationPoint.Point.Z, 3, MidpointRounding.AwayFromZero);
                    zElement = zPoint;
                }

                return new ElevationDouble(_doc, zElement).AboveLevel();
            }


            // Проверка на наличие OwnerViewId, принадлежности виду и тем, что OwnerView это ViewPlan исключаем возможность,
            // что аннотация не находится на ViewDrafting, ViewSection или ViewSheet

            // текстовые примечания, линии детализации, марки электрооборудования, марки осветительных приборов, марки помещений
            // марки нескольких категорий, марки коробов, типовые аннотации (стрелка выносок), элементы узлов, размеры
            if (element.OwnerViewId != null && _doc.GetElement(element.OwnerViewId) is ViewPlan ownerView)
            {
                return ownerView.GenLevel as Level;
            }


            return null;
        }




        /// <summary>
        /// <para>над каким уровнем находится элемент
        /// <para>у вертикальных объектов берется нижняя точка
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Level GetСlosestLevel(Element element)
        {
            double zElement = 0;

            if (element.Location is LocationCurve locationCurve)
            {
                //нижняя точка линии, та у которой значение высотной отметки меньше
                double minZ = Math.Round(
                    Math.Min(locationCurve.Curve.GetEndPoint(0).Z, locationCurve.Curve.GetEndPoint(1).Z),
                    3, MidpointRounding.AwayFromZero);
                zElement = minZ;
            }
            else if (element.Location is LocationPoint locationPoint)
            {
                double zPoint = Math.Round(locationPoint.Point.Z, 3, MidpointRounding.AwayFromZero);
                zElement = zPoint;
            }

            return new ElevationDouble(_doc, zElement).AboveLevel();

        }

    }
}





//public Level GetLevel(Element element)
//{
//    if (element == null)
//        return null;

//    // 1. Прямой LevelId (самый быстрый путь)
//    if (element.LevelId != null && element.LevelId.IntegerValue != -1)
//        return _doc.GetElement(element.LevelId) as Level;

//    // 2. Type-based логика
//    switch (element)
//    {
//        case FamilyInstance fi
//            when fi.HostFace == null
//            && fi.Category?.CategoryType == CategoryType.Model:
//            return fi.Host as Level;

//        case Conduit conduit
//            when conduit.ReferenceLevel is Level refLevel:
//            return refLevel;

//        case CableTray tray
//            when tray.ReferenceLevel is Level trayLevel:
//            return trayLevel;

//        case ElectricalSystem system:
//            return GetElectricalSystemLevel(system);

//        case FamilyInstance fi
//            when fi.HostFace != null:
//            return GetLevelFromElevation(fi);

//        default:
//            break;
//    }

//    // 3. View-specific элементы → ТОЛЬКО ViewPlan
//    if (element.OwnerViewId != ElementId.InvalidElementId
//        && _doc.GetElement(element.OwnerViewId) is ViewPlan viewPlan)
//    {
//        return viewPlan.GenLevel;
//    }

//    return null;
//}


//private Level GetElectricalSystemLevel(ElectricalSystem system)
//{
//    var baseEquipment = system.BaseEquipment;

//    if (baseEquipment == null)
//    {
//        new ErrorModel()
//            .UserWarning(new NoConnectCircuit().MessageForUser(system));
//        return null;
//    }

//    if (baseEquipment.LevelId != null && baseEquipment.LevelId.IntegerValue != -1)
//        return _doc.GetElement(baseEquipment.LevelId) as Level;

//    return baseEquipment.Host as Level;
//}


//private Level GetLevelFromElevation(FamilyInstance fi)
//{
//    double z = fi.Location switch
//    {
//        LocationCurve lc =>
//            Math.Round(
//                Math.Min(
//                    lc.Curve.GetEndPoint(0).Z,
//                    lc.Curve.GetEndPoint(1).Z),
//                3, MidpointRounding.AwayFromZero),

//        LocationPoint lp =>
//            Math.Round(lp.Point.Z, 3, MidpointRounding.AwayFromZero),

//        _ => 0
//    };

//    return new ElevationDouble(_doc, z).AboveLevel();
//}
