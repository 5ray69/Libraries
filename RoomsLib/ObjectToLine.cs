using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using Line = Autodesk.Revit.DB.Line;

namespace Libraries.RoomsLib
{
    public class ObjectToLine(Object lineOrXYZ, Room room, double lengthOutLine = 7)
    {
        //public readonly double lengthOutLine = 7;
        public readonly double levelZ = room.Level.Elevation;

        /// <summary>
        /// <para>создает линию из Line, или из XYZ, нужной длины
        /// <para>на уровне координаты Z уровня помещения
        /// <para>по умолчанию создается минимальная линия 7мм
        /// </summary>
        /// <returns></returns>
        public Line GetCreatedLine()
        {
            Transform transformDefault = Transform.CreateTranslation(
                new XYZ(
                    0,
                    //минимальная длина для короба 3мм, а мы по умолчанию зададим 7мм
                    UnitUtils.ConvertToInternalUnits(lengthOutLine, UnitTypeId.Millimeters),
                    0
                )
            );

            if (lineOrXYZ is Line line)
            {
                XYZ minimumStartPoint = new(
                                            line.GetEndPoint(0).X,
                                            line.GetEndPoint(0).Y,
                                            levelZ
                                            );

                //если линия расположена вертикально, её проекция будет точкой
                if (line.Direction.Normalize().Z == 1)
                    return Line.CreateBound(
                                minimumStartPoint,
                                Point.Create(transformDefault.OfPoint(minimumStartPoint)).Coord
                                );
                else
                {
                    //если линия горизонтальная или не строго вертикальная
                    XYZ startXYZ = line.GetEndPoint(0);
                    XYZ endXYZ = line.GetEndPoint(1);
                    XYZ projectStartXYZ = new(startXYZ.X, startXYZ.Y, levelZ);
                    XYZ projectEndXYZ = new(endXYZ.X, endXYZ.Y, levelZ);
                    XYZ vectorOnPlaneProect = (projectEndXYZ - projectStartXYZ);

                    //создаем трансляцию смещения по вектору направления линии
                    Transform transformAtAnAngle = Transform.CreateTranslation(
                        vectorOnPlaneProect.Normalize() * UnitUtils.ConvertToInternalUnits(lengthOutLine, UnitTypeId.Millimeters)
                        );

                    //если между проекциями точек начала и конца линии расстояние меньше 7мм
                    if (projectStartXYZ.DistanceTo(projectEndXYZ) < UnitUtils.ConvertToInternalUnits(7, UnitTypeId.Millimeters))
                        return Line.CreateBound(
                                                minimumStartPoint,
                                                //Point.Create(transformAtAnAngle.OfPoint(minimumStartPoint)).Coord
                                                Point.Create(transformDefault.OfPoint(minimumStartPoint)).Coord
                                                );
                    else
                    {
                        //если между проекциями точек расстояние больше либо равно 7мм

                        //если длина линии по умолчанию
                        if (lengthOutLine == 7)
                            return Line.CreateBound(projectStartXYZ, projectEndXYZ);

                        //если длина линии по умолчанию
                        else
                            return Line.CreateBound(
                                                    minimumStartPoint,
                                                    Point.Create(transformAtAnAngle.OfPoint(minimumStartPoint)).Coord
                                                    );
                    }
                }
            }

            if (lineOrXYZ is XYZ xyz)
            {
                XYZ miniStartPoint = new(xyz.X, xyz.Y, levelZ);

                return Line.CreateBound(
                                        miniStartPoint,
                                        Point.Create(transformDefault.OfPoint(miniStartPoint)).Coord
                                        );
            }

            return null;
        }
    }
}
