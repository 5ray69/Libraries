using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using Line = Autodesk.Revit.DB.Line;

namespace Libraries
{
    public class IsLocated(Room room, Line projectLine, Transform linkTransform)
    {
        public readonly double _levelZ = room.Level.Elevation;

        /// <summary>
        /// <para>ВОЗВРАЩАЕТ TRUE ЕСЛИ ПРОЕКЦИЯ ЛИНИИ ПЕРЕСЕКАЕТ ГРАНИЦУ ПОМЕЩЕНИЯ.
        /// <para>Принимает Line или XYZ(полученые из Location объектов)
        /// <para>Сколько бы пересечений с границей помещения ни было и где б они ни были,
        /// <para>то объект в помещении.
        /// <para>Даже если одна точка/край линии лежит на границе помещения, а вся остальная часть линии вне.
        /// <para>Если линия совпадает с линией границы помещения, то объект в помещении.
        /// </summary>
        /// <returns></returns>
        public bool InsideTheBorders()
        {
            ICollection<Line> bordersRoom = new BordersRoom(room, linkTransform).GetBordersToCenter();
            foreach (Line roomLine in bordersRoom)
                //ЕСЛИ ЕСТЬ ЛЮБОЕ СОВПАДЕНИЕ/НАЛОЖЕНИЕ ПРОЕКЦИИ НА ГРАНИЦУ ПОМЕЩЕНИЯ, ТО ОБЪЕКТ В ПОМЕЩЕНИИ
                //любое совпадение/наложение - это или наложение/совпадение точных копий,
                //или наложение/совпадение линий не равной длины с смещениями относительно начала или конца
                //if (projectLine.Intersect(roomLine, out IntersectionResultArray resultArray) == SetComparisonResult.Equal)
                if (projectLine.Intersect(roomLine, resultArray: out _) == SetComparisonResult.Equal)
                    return true;

            // ЕСЛИ ПРОЕКЦИЯ ЛИНИИ ПЕРЕСЕКАЕТ ГРАНИЦУ ПОМЕЩЕНИЯ, ТО ОБЪЕКТ В ПОМЕЩЕНИИ
            // проекция линии без удлинения нужна прежде удлиненной линии потому, что
            // линия может заходить извне и обрываться в помещении, тогда ее удлинение,
            // даст два пересечения границы помещения, что соотвествует условию
            // нахождения тестовой удлиненной линии вне помещения
            // ЗДЕСЬ ПРОЕКЦИЯ ЛИНИИ БЕЗ УДЛИНЕНИЯ
            CountIntersectionsWithPolygon countIntersectionsWithPolygon1 = new(bordersRoom, projectLine);
            Dictionary<string, int> projectIntersect = countIntersectionsWithPolygon1.GetDictionary();

            //если в словаре насобирали пересечения (cловарь не пуст), то объект в помещении
            if (projectIntersect.Count > 0)
                return true;

            //иначе словарь пуст, и тогда ПРОВЕРЯЕМ УДЛИНЕННОЙ ТЕСТОВОЙ ЛИНИЕЙ
            if (projectIntersect.Count == 0)
            {

                //ЕСЛИ КОРОБ/ПРОЕКЦИЯ ОСЕВЕЙ ЛИНИИ НЕ ПЕРЕСЕКАЕТ ГРАНИЦУ ПОМЕЩЕНИЯ,
                //то он или внутри, или вне границы помещения
                //поэтому из проекции строим удлиненную тестовую линию для проверки пересечений
                //количество пересечений = 0 - вне помещения
                //количество пересечений четное - вне помещения
                //количество пересечений нечетное в помещении

                //ТЕСТОВАЯ ЛИНИЯ
                //задаем в мм, удобно для человека, а код переводит во внутренние единицы
                //длина здания около 60000мм (длинный коридор) потому берем 70000мм
                ObjectToLine objectToLine = new(projectLine, room, 70000);
                Line testLength = objectToLine.GetCreatedLine();

                CountIntersectionsWithPolygon countIntersectionsWithPolygon2 = new(bordersRoom, testLength);
                Dictionary<string, int> testIntersect = countIntersectionsWithPolygon2.GetDictionary();

                //ЕСЛИ КОЛИЧЕСТВО ПЕРЕСЕЧЕНИЙ/ПАР КЛЮЧ-ЗНАЧЕНИЕ В СЛОВАРЕ РАВНО НУЛЮ = ВНЕ ПОМЕЩЕНИЯ
                //остаток от деления нуля на 2 тоже ноль = четное
                if (testIntersect.Count == 0)
                    return false;

                //ЕСЛИ КОЛИЧЕСТВО ПЕРЕСЕЧЕНИЙ/ПАР КЛЮЧ-ЗНАЧЕНИЕ В СЛОВАРЕ ЧЕТНОЕ = ВНЕ ПОМЕЩЕНИЯ
                if (testIntersect.Count % 2 == 0)
                    return false;


                //ЕСЛИ КОЛИЧЕСТВО ПЕРЕСЕЧЕНИЙ/ПАР КЛЮЧ-ЗНАЧЕНИЕ В СЛОВАРЕ НЕЧЕТНОЕ = ВНУТРИ ПОМЕЩЕНИЯ
                if (testIntersect.Count % 2 != 0)
                    return true;

            }

            // Возвращение значения по умолчанию в конце метода
            //return false;
            return false;
        }
    }
}

//projectLine.Intersect(roomLine, out IntersectionResultArray resultArray) == SetComparisonResult.Equal
//Equal - совпадение двух линий (частичное совпадение неодинаковых линий, но лежащих одна в другой или совпадение абсолютных копий двух линий)
//Overlap - пересечение (Перекрытие двух наборов/линий не является пустым и строгим подмножеством обоих наборов)
//Disjoint - непересекающиеся/несвязаны (Оба набора/линии не пусты и не перекрываются)


//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Architecture;
//using System.Collections.Generic;
//using Line = Autodesk.Revit.DB.Line;

//namespace Libraries.Models.MyDll
//{
//    public class IsLocated(Room room, Line projectLine, Transform linkTransform)
//    {
//        public readonly double _levelZ = room.Level.Elevation;

//        public bool InsideTheBorders()
//        {
//            ICollection<Line> bordersRoom = new BordersRoom(room, linkTransform).GetBordersToCenter();
//            foreach (Line roomLine in bordersRoom)
//                if (projectLine.Intersect(roomLine, out IntersectionResultArray resultArray) == SetComparisonResult.Equal)
//                    return true;

//            CountIntersectionsWithPolygon countIntersectionsWithPolygon1 = new(bordersRoom, projectLine);
//            Dictionary<string, int> projectIntersect = countIntersectionsWithPolygon1.GetDictionary();

//            if (projectIntersect.Count > 0)
//                return true;

//            if (projectIntersect.Count == 0)
//            {
//                ObjectToLine objectToLine = new(projectLine, room, 70000);
//                Line testLength = objectToLine.GetCreatedLine();

//                CountIntersectionsWithPolygon countIntersectionsWithPolygon2 = new(bordersRoom, testLength);
//                Dictionary<string, int> testIntersect = countIntersectionsWithPolygon2.GetDictionary();

//                if (testIntersect.Count == 0)
//                    return false;

//                if (testIntersect.Count % 2 == 0)
//                    return false;


//                if (testIntersect.Count % 2 != 0)
//                    return true;

//            }

//            return false;
//        }
//    }
//}
