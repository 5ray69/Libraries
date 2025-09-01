using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Libraries
{
    public class CountIntersectionsWithPolygon(ICollection<Line> roomBorders, Line projectLine)
    {
        public readonly ICollection<Line> _roomBorders = roomBorders;
        public readonly Line _projectLine = projectLine;


        /// <summary>
        /// <para>ВОЗВРАЩАЕТ СЛОВАРЬ ДЛЯ ПОДСЧЕТА КОЛИЧЕСТВА ПЕРЕСЕЧЕНИЙ С МНОГОУГОЛЬНИКОМ ГРАНИЦ ПОМЕЩЕНИЯ
        /// <para>количество пар ключ-значение равно количеству прохождений проекции линии через границу помещения
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetDictionary()
        {
            Dictionary<string, int> projectIntersect = [];

            foreach (Line roomLine in _roomBorders)
            {

                // если проекция линии пересекает границу помещения
                if (_projectLine.Intersect(roomLine, out IntersectionResultArray resultArray) == SetComparisonResult.Overlap)
                {
                    // Проходя через вершину, линия пересекает две линии границы помещения сразу и дает 2 пересечения.
                    // Если такая вершина уже есть в словаре, то второе пересечение ее же, но в другой линии,
                    // не будет добавлено в словарь и будет зачтено как одно пересечение
                    XYZ startXYZ = roomLine.GetEndPoint(0);
                    string sumStartXYZ = Math.Round(startXYZ.X + startXYZ.Y + startXYZ.Z, 7, MidpointRounding.AwayFromZero).ToString();  // округление до 7 знаков

                    // если projectLine проходит через вершину = начальную точку линии границы
                    if (_projectLine.Distance(startXYZ) == 0.0)
                    {
                        if (!projectIntersect.ContainsKey(sumStartXYZ))
                            projectIntersect.Add(sumStartXYZ, 1);
                    }

                    XYZ endXYZ = roomLine.GetEndPoint(1);
                    string sumEndXYZ = Math.Round(endXYZ.X + endXYZ.Y + endXYZ.Z, 7, MidpointRounding.AwayFromZero).ToString();  // округление до 7 знаков

                    // если projectLine проходит через вершину = конечную точку линии границы
                    if (_projectLine.Distance(endXYZ) == 0.0)
                    {
                        if (!projectIntersect.ContainsKey(sumEndXYZ))
                            projectIntersect.Add(sumEndXYZ, 1);
                    }


                    // если projectLine не проходит
                    // ни через вершину начальной точки линии границы,
                    // ни через вершину конечной точки линии границы (пересекает где-то в другом месте линию границы)
                    if (_projectLine.Distance(startXYZ) != 0.0 || _projectLine.Distance(endXYZ) != 0.0)
                    {
                        XYZ centerLineXYZ = roomLine.Evaluate(0.5, true);
                        string currentKey = Math.Round(centerLineXYZ.X + centerLineXYZ.Y + centerLineXYZ.Z, 7, MidpointRounding.AwayFromZero).ToString();  // округление до 7 знаков
                        if (!projectIntersect.ContainsKey(currentKey))
                            projectIntersect.Add(currentKey, 1);
                    }
                }
            }
            return projectIntersect;
        }
    }
}
