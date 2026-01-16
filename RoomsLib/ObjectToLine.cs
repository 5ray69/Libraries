using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using Line = Autodesk.Revit.DB.Line;



namespace Libraries.RoomsLib
{
    public class ObjectToLine(double lengthOutLine = 100000)
    {
        //lengthOutLine = 100000, длина здания около 60000мм (длинный коридор)
        //public readonly double _lengthOutLine = UnitUtils.ConvertToInternalUnits(lengthOutLine, UnitTypeId.Millimeters);
        public readonly double _lengthOutLine = lengthOutLine / 304.8;

        private const double RotationAngleRad = 3 * Math.PI / 180;



        // ---------------- PUBLIC API ----------------

        /// <summary>
        /// <para>Создает линию из Line, или из XYZ,
        /// <para>длиной 100 метров, по умолчанию,
        /// <para>на уровне координаты Z уровня помещения
        /// </summary>
        /// <param name="lineOrXYZ">точка или линия</param>
        /// <param name="room">помещение</param>
        /// <returns></returns>
        public Line GetCreatedLine(object lineOrXYZ, Room room)
        {
            return GetCreatedLineInternal(lineOrXYZ, room.Level.Elevation);
        }



        /// <summary>
        /// <para>Создает линию из Line, или из XYZ,
        /// <para>длиной 100 метров, по умолчанию,
        /// <para>на уровне координаты Z уровня высотной отметки elevation.
        /// </summary>
        /// <param name="lineOrXYZ">точка или линия</param>
        /// <param name="elevation">высотная отметка</param>
        /// <returns></returns>
        public Line GetCreatedLine(object lineOrXYZ, double elevation)
        {
            return GetCreatedLineInternal(lineOrXYZ, elevation);
        }



        // ---------------- CORE LOGIC ----------------

        /// <summary>
        /// <para>создает линию из Line, или из XYZ,
        /// <para>длиной 100 метров,
        /// <para>на уровне координаты Z уровня помещения
        /// <param name="lineOrXYZ"></param>
        /// <param name="levelZ"></param>
        /// <returns></returns>
        private Line GetCreatedLineInternal(object lineOrXYZ, double levelZ)
        {
            Transform rotation = Transform.CreateRotation(XYZ.BasisZ, RotationAngleRad);
            Transform translation = Transform.CreateTranslation(new XYZ(0, _lengthOutLine, 0));
            Transform rotationTranslation = rotation.Multiply(translation);

            if (lineOrXYZ is Line line)
            {
                return CreateFromLine(line, levelZ, rotation);
            }

            if (lineOrXYZ is XYZ xyz)
            {
                return CreateFromPoint(xyz, levelZ, rotationTranslation);
            }

            return null;
        }



        // ---------------- HELPERS ----------------

        /// <summary>
        /// /// Создает линию из Line на уровне levelZ с применением трансформации rotation
        /// </summary>
        /// <param name="line">линия</param>
        /// <param name="levelZ">высотная отметка</param>
        /// <param name="rotation">трансформация</param>
        /// <returns></returns>
        private Line CreateFromLine(Line line, double levelZ, Transform rotation)
        {
            XYZ startPoint = line.GetEndPoint(0);
            XYZ endPoint = line.GetEndPoint(1);

            XYZ projectedStart = new(startPoint.X, startPoint.Y, levelZ);
            XYZ projectedEnd = new(endPoint.X, endPoint.Y, levelZ);

            //если линия расположена вертикально, параллельно оси Z, то её проекция будет точкой
            //if (line.Direction.Normalize().Z == 1)
            if (Math.Abs(line.Direction.Z) > 0.999)
            {
                XYZ directionXY = rotation
                    .OfVector(XYZ.BasisY)
                    .Normalize();

                XYZ end = projectedStart + directionXY * _lengthOutLine;
                return Line.CreateBound(projectedStart, end);
            }


            // обычная линия
            // Направление линии
            XYZ vectorOnPlane = (projectedEnd - projectedStart).Normalize();
            // Применяем вращение к вектору направления линии
            XYZ rotatedVector = rotation.OfVector(vectorOnPlane) * _lengthOutLine;

            return Line.CreateBound(projectedStart, projectedStart + rotatedVector);
        }


        /// <summary>
        /// Создает линию из точки XYZ на уровне levelZ с применением трансформации rotationTranslation
        /// </summary>
        /// <param name="xyz">объект XYZ = точка</param>
        /// <param name="levelZ">высотная отметка</param>
        /// <param name="rotationTranslation">трансформация</param>
        /// <returns></returns>
        private Line CreateFromPoint(XYZ xyz, double levelZ, Transform rotationTranslation)
        {
            XYZ start = new(xyz.X, xyz.Y, levelZ);
            XYZ end = start + rotationTranslation.OfVector(new XYZ(0, _lengthOutLine, 0));

            return Line.CreateBound(start, end);
        }



        // ---------------- INTERSECTION ----------------

        public double? GetIntersectionPointSum(Line projLine, Line lineRoom)
        {
            // Используем метод Intersect для определения пересечения
            IntersectionResultArray intersectionResult = null;
            projLine.Intersect(lineRoom, out intersectionResult);

            // Проверяем наличие пересечения
            if (intersectionResult != null && intersectionResult.Size > 0)
            {
                XYZ point = intersectionResult.get_Item(0).XYZPoint;

                // Возвращаем сумму координат точки пересечения
                //return point.X + point.Y + point.Z;
                return point.X + point.Y;
            }

            return null; // Пересечения нет
        }
    }
}





//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Architecture;
//using System;
//using Line = Autodesk.Revit.DB.Line;



//namespace Libraries.RoomsLib
//{
//    public class ObjectToLine(double lengthOutLine = 100000)
//    {
//        //lengthOutLine = 100000 длина здания около 60000мм (длинный коридор)
//        public readonly double _lengthOutLine = UnitUtils.ConvertToInternalUnits(lengthOutLine, UnitTypeId.Millimeters);


//        //        /// <summary>
//        //        /// <para>создает линию из Line, или из XYZ,
//        //        /// <para>длиной 100 метров,
//        //        /// <para>на уровне координаты Z уровня помещения
//        //        /// </summary>
//        //        /// <returns></returns>
//        public Line GetCreatedLine(Object lineOrXYZ, Room room)
//        {
//            double _levelZ = room.Level.Elevation;
//            Transform rotationDefault = Transform.CreateRotation(XYZ.BasisZ, 3 * (Math.PI / 180));// поворот на 3 градуса в радианах
//            Transform translationDefault = Transform.CreateTranslation(new XYZ(0, _lengthOutLine, 0));
//            Transform rotationTranslationDefault = rotationDefault.Multiply(translationDefault);

//            if (lineOrXYZ is Line line)
//            {
//                XYZ startPoint = line.GetEndPoint(0);
//                XYZ endPoint = line.GetEndPoint(1);

//                XYZ projectedStart = new(startPoint.X, startPoint.Y, _levelZ);
//                XYZ projectedEnd = new(endPoint.X, endPoint.Y, _levelZ);

//                XYZ direction = projectedEnd - projectedStart;
//                double distance = projectedStart.DistanceTo(projectedEnd);


//                //если линия расположена вертикально, параллельно оси Z, то её проекция будет точкой
//                //if (line.Direction.Normalize().Z == 1)
//                if (Math.Abs(line.Direction.Z) > 0.999)
//                {
//                    XYZ start = projectedStart;

//                    Transform rotation = Transform.CreateRotation(XYZ.BasisZ, 3 * Math.PI / 180);

//                    XYZ directionXY = rotation.OfVector(XYZ.BasisY).Normalize();

//                    XYZ end = start + directionXY * _lengthOutLine;

//                    return Line.CreateBound(start, end);
//                }
//                else
//                {
//                    // Направление линии
//                    XYZ vectorOnPlaneProect = (projectedEnd - projectedStart).Normalize();

//                    // Создаем вращение относительно направления линии
//                    Transform rotationRelativeToLine = Transform.CreateRotation(XYZ.BasisZ, 3 * (Math.PI / 180));

//                    // Применяем вращение к вектору направления линии
//                    XYZ rotatedVector = rotationRelativeToLine.OfVector(vectorOnPlaneProect) * _lengthOutLine;

//                    // Конечная точка после применения трансформации
//                    XYZ twoPoint = projectedStart + rotatedVector;

//                    return Line.CreateBound(projectedStart, twoPoint);

//                }
//            }


//            if (lineOrXYZ is XYZ xyz)
//            {

//                XYZ miniStartPoint = new(xyz.X, xyz.Y, _levelZ);
//                XYZ miniEndPoint = miniStartPoint + rotationTranslationDefault.OfVector(
//                    new XYZ(0, _lengthOutLine, 0)
//                    );
//                return Line.CreateBound(miniStartPoint, miniEndPoint);

//            }

//            return null;
//        }


//        public double? GetIntersectionPointSum(Line projLine, Line lineRoom)
//        {
//            // Используем метод Intersect для определения пересечения
//            IntersectionResultArray intersectionResult = null;
//            projLine.Intersect(lineRoom, out intersectionResult);

//            // Проверяем наличие пересечения
//            if (intersectionResult != null && intersectionResult.Size > 0)
//            {
//                XYZ point = intersectionResult.get_Item(0).XYZPoint;

//                // Возвращаем сумму координат точки пересечения
//                //return point.X + point.Y + point.Z;
//                return point.X + point.Y;
//            }

//            return null; // Пересечения нет
//        }
//    }
//}

