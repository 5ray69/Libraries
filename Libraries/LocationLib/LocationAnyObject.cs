using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using Libraries.LocationLib.UserWarningLocationLib;

namespace Libraries.LocationLib
{
    /// <summary>
    /// Точка размещения любого объекта
    /// </summary>
    public class LocationAnyObject()
    {
        /// <summary>
        /// Точка размещения любого объекта
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Object GetSpatialElementOrLocationXyzOrLine(Element element)
        {
            if (element is FamilyInstance familyInstance)
            {
                if (familyInstance.HasSpatialElementCalculationPoint)
                    return familyInstance.GetSpatialElementCalculationPoint();

                else if (familyInstance.Location is LocationPoint locationPoint)
                    return locationPoint.Point;

                else if (familyInstance.Location is LocationCurve locationCurve)
                    return locationCurve.Curve;
            }
            else
            {
                if (element.Location is LocationPoint locatPoint)
                {
                    return locatPoint.Point;
                }
                else if (element.Location is LocationCurve locationCurve)
                {
                    return locationCurve.Curve;
                }
            }


            if (element is ElectricalSystem electricalSystem)
            {
                var baseEquipment = electricalSystem.BaseEquipment;
                if (baseEquipment == null) // цепь не подключена "код завершил работу" с предупреждением пользователю
                {
                    ErrorModel errorModel = new();
                    errorModel.UserWarning(new NoConnectCircuit().MessageForUser(electricalSystem));
                }

                if (baseEquipment.HasSpatialElementCalculationPoint)
                    return baseEquipment.GetSpatialElementCalculationPoint();

                else if (baseEquipment.Location is LocationPoint locationPoint)
                    return locationPoint.Point;

                else if (baseEquipment.Location is LocationCurve locationCurve)
                    return locationCurve.Curve;
            }


            if (element.Location is null) // "код завершил работу" с предупреждением пользователю
            {
                ErrorModel errorModel = new();
                errorModel.UserWarning(new NoContainLocation().MessageForUser(element));
            }


            return null;
        }
    }
}


//public static class ElementElevationExtractor
//{
//    /// <summary>
//    /// Надёжно извлекает базовую высотную отметку Z элемента в проектных координатах (в футах).
//    /// Возвращает null, если элемент не имеет 3D-позиции (например, 2D-аннотации).
//    /// </summary>
//    public static double? GetElementZ(Element element)
//    {
//        if (element is null)
//            return null;

//        // 1. Перекрытия: привязываются к уровню верхней гранью (отметка верха плиты)
//        if (element is Floor floor)
//        {
//            Parameter topElevationParam = floor.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP);
//            if (topElevationParam is { HasValue: true })
//            {
//                return topElevationParam.AsDouble();
//            }
//        }

//        // 2. Элементы с точечной привязкой (колонны, окна, двери, оборудование, мебель)
//        if (element.Location is LocationPoint locationPoint)
//        {
//            return locationPoint.Point.Z;
//        }

//        // 3. Линейные элементы (стены, балки, трубы, воздуховоды, лотки)
//        if (element.Location is LocationCurve locationCurve && locationCurve.Curve is Curve curve)
//        {
//            // Для наклонных элементов (например, пандусов, труб с уклоном, балок)
//            // берём минимальную Z из двух концов кривой
//            double z0 = curve.GetEndPoint(0).Z;
//            double z1 = curve.GetEndPoint(1).Z;
//            return Math.Min(z0, z1);
//        }

//        // 4. Сложные элементы без Location (контекстные семейства, лестницы, крыши, обобщённые модели):
//        // ищем минимальную Z по реальным 3D-телам (Solid), игнорируя невидимые линии габаритов
//        double? solidMinZ = GetMinZFromSolids(element);
//        if (solidMinZ.HasValue)
//        {
//            return solidMinZ.Value;
//        }

//        // 5. Запасной вариант (fallback): общий BoundingBox модели
//        BoundingBoxXYZ bbox = element.get_BoundingBox(null);
//        if (bbox is not null)
//        {
//            return bbox.Min.Z;
//        }

//        return null;
//    }

//    /// <summary>
//    /// Вычисляет минимальную координату Z по всем непустым Solid геометрии элемента
//    /// </summary>
//    private static double? GetMinZFromSolids(Element element)
//    {
//        Options geomOptions = new Options
//        {
//            ComputeReferences = false,
//            DetailLevel = ViewDetailLevel.Coarse // Coarse работает быстрее и достаточен для габаритов
//        };

//        GeometryElement geomElem = element.get_Geometry(geomOptions);
//        if (geomElem is null)
//            return null;

//        double minZ = double.MaxValue;
//        bool hasGeometry = false;

//        ExtractMinZRecursive(geomElem, ref minZ, ref hasGeometry);

//        return hasGeometry ? minZ : null;
//    }

//    private static void ExtractMinZRecursive(GeometryElement geomElem, ref double minZ, ref bool hasGeometry)
//    {
//        foreach (GeometryObject geomObj in geomElem)
//        {
//            if (geomObj is Solid solid && solid.Volume > 1e-9)
//            {
//                BoundingBoxXYZ solidBox = solid.GetBoundingBox();
//                if (solidBox is not null)
//                {
//                    // В Revit BoundingBox у Solid возвращается в локальных координатах Solid,
//                    // поэтому применяем его Transform для перевода в координаты проекта
//                    XYZ globalMin = solidBox.Transform.OfPoint(solidBox.Min);
//                    if (globalMin.Z < minZ)
//                    {
//                        minZ = globalMin.Z;
//                        hasGeometry = true;
//                    }
//                }
//            }
//            else if (geomObj is GeometryInstance geomInstance)
//            {
//                // GetInstanceGeometry() уже возвращает геометрию, трансформированную в координаты проекта
//                GeometryElement instanceGeom = geomInstance.GetInstanceGeometry();
//                if (instanceGeom is not null)
//                {
//                    ExtractMinZRecursive(instanceGeom, ref minZ, ref hasGeometry);
//                }
//            }
//        }
//    }