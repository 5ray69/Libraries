using Autodesk.Revit.DB;

namespace Libraries.RoomsLib
{
    /// <summary>
    /// Точка размещения в помещении или точка размещения семейства
    /// </summary>
    /// <param name="elem"></param>
    public class PointSpatialElementOrPointLocation(FamilyInstance elem)
    {
        private readonly FamilyInstance _elem = elem;

        /// <summary>
        /// возвращает XYZ точки размещения в помещении, а если ее нет, то точки размещения семейства
        /// </summary>
        /// <returns></returns>
        public XYZ GetXYZSpatialElementOrPointLocation()
        {
            if (_elem.HasSpatialElementCalculationPoint)
                return _elem.GetSpatialElementCalculationPoint();
            else if (_elem.Location is LocationPoint locationPoint)
                return locationPoint.Point;
            else
                return null;
        }

        /// <summary>
        /// возвращает Point точки размещения в помещении, а если ее нет, то точки размещения семейства
        /// </summary>
        /// <returns></returns>
        public Autodesk.Revit.DB.Point GetPointSpatialElementOrPointLocation()
        {
            return Autodesk.Revit.DB.Point.Create(GetXYZSpatialElementOrPointLocation());
        }

        /// <summary>
        /// XYZ точки размещения в помещении
        /// </summary>
        /// <returns></returns>
        public XYZ GetXYZSpatialElement()
        {
            if (_elem.HasSpatialElementCalculationPoint)
                return _elem.GetSpatialElementCalculationPoint();
            else
                return null;
        }

        /// <summary>
        /// XYZ точки размещения семейства
        /// </summary>
        /// <returns></returns>
        public XYZ GetXYZSLocationPoint()
        {
            if (_elem.HasSpatialElementCalculationPoint)
                return _elem.GetSpatialElementCalculationPoint();
            else
                return null;
        }

        /// <summary>
        /// Point точки размещения в помещении
        /// </summary>
        /// <returns></returns>
        public Autodesk.Revit.DB.Point GetPointSpatialElement()
        {
            return Autodesk.Revit.DB.Point.Create(GetXYZSpatialElement());
        }

        /// <summary>
        /// Point точки размещения семейства
        /// </summary>
        /// <returns></returns>
        public Autodesk.Revit.DB.Point GetPointLocationPoint()
        {
            return Autodesk.Revit.DB.Point.Create(GetXYZSLocationPoint());
        }
    }
}
