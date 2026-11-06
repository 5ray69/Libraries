using Autodesk.Revit.DB;

namespace Libraries.BoundingBoxs
{
    /// <summary>
    /// Пересекаются ли баундингбоксы
    /// </summary>
    public class BoundingBoxes
    {
        /// <summary>
        /// Пересекаются ли баундингбоксы
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public bool IsIntersect(BoundingBoxXYZ b1, BoundingBoxXYZ b2)
        {
            return b1.Min.X <= b2.Max.X && b1.Max.X >= b2.Min.X &&
                   b1.Min.Y <= b2.Max.Y && b1.Max.Y >= b2.Min.Y &&
                   b1.Min.Z <= b2.Max.Z && b1.Max.Z >= b2.Min.Z;
        }
    }
}
