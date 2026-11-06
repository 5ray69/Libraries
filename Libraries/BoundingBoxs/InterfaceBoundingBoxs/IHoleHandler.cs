using Autodesk.Revit.DB;

namespace Libraries.BoundingBoxs.InterfaceBoundingBoxs
{
    /// <summary>
    /// Интерфейс обработчика отверстий
    /// </summary>
    public interface IHoleHandler
    {
        /// <summary>
        /// Создает баундингбокс с измененными размерами
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="distance"></param>
        /// <param name="linkTransform"></param>
        /// <returns></returns>
        BoundingBoxXYZ ResizeBoundingBox(FamilyInstance familyInstance, double distance, Transform linkTransform);
    }
}
