using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;


namespace Libraries.RoomsLib
{
    /// <summary>
    /// Границы помещения на уровне координаты Z уровня помещения с учетом трансформации связи
    /// </summary>
    /// <param name="room"></param>
    /// <param name="linkTransform"></param>
    public class BordersRoom(Room room, Transform linkTransform)
    {

        /// <summary>
        /// Высотная отметка координаты Z уровня, к которому привязано помещение
        /// </summary>
        public readonly double _levelZ = room.Level.Elevation;

        /// <summary>
        /// <para>ЛИНИИ ГРАНИЦ ПОМЕЩЕНИЯ</para>
        /// <para>на уровне координаты Z уровня помещения</para>
        /// <para>с учетом трансформации связи</para>
        /// </summary>
        /// <returns></returns>
        public List<Line> GetBordersToCenter()
        {
            //назначаем переменную на свойство
            SpatialElementBoundaryOptions roomoptions = new()
            {
                //установили свойство границы помещения по осевой линии / центру
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
            };

            //ГРАНИЦЫ ПОМЕЩЕНИЯ в уровне координаты Z того уровня, к которому привязано помещение
            List<Line> roomLines = [];
            foreach (IList<BoundarySegment> segmentList in room.GetBoundarySegments(roomoptions))
            {
                foreach (BoundarySegment boundarySegment in segmentList)
                {
                    Curve curveSegment = boundarySegment.GetCurve();

                    XYZ startXYZ = curveSegment.GetEndPoint(0);
                    XYZ endXYZ = curveSegment.GetEndPoint(1);
                    roomLines.Add(Line.CreateBound(
                                    linkTransform.OfPoint(new XYZ(startXYZ.X, startXYZ.Y, _levelZ)),
                                    linkTransform.OfPoint(new XYZ(endXYZ.X, endXYZ.Y, _levelZ))
                                    )
                                );
                }
            }
            return roomLines;
        }


        /// <summary>
        /// <para>XYZ ВЕРШИН УГЛОВ ГРАНИЦ ПОМЕЩЕНИЯ</para>
        /// <para>образованы только началами линий</para>
        /// <para>на уровне координаты Z уровня помещения</para>
        /// <para>с учетом трансформации связи</para>
        /// </summary>
        /// <returns></returns>
        public ICollection<XYZ> GetXYZVerticesBorders()
        {
            ICollection<XYZ> verticesRoomLines = [];

            foreach (Line linesBorder in this.GetBordersToCenter())
                verticesRoomLines.Add(linkTransform.OfPoint(new XYZ(
                                linesBorder.GetEndPoint(0).X, linesBorder.GetEndPoint(0).Y, _levelZ
                                )));

            return verticesRoomLines;
        }
    }
}
