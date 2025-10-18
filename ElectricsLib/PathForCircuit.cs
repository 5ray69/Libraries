using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System.Collections.Generic;
using System.Linq;

namespace CreateRisersShield.MyDll
{
    /// <summary>
    /// <para>Класс, выполняющий построение пути от панели до щита через соединённые элементы.</para>
    /// <para>Первый узел = координаты/location панели.</para>
    /// <para>Последний узел = координаты одного из 5 коннекторов щита.</para>
    /// </summary>
    public class PathForCircuit
    {
        /// <summary>
        /// Метод возвращает список узлов пути цепи.
        ///   currentCon - это один из 5 коннекторов щита.
        ///   conPanel - это реальный коннектор панели, к которой подключаем, указанной пользователем.
        ///   firstAfterPanelCon - это конечный неподключенный коннектор пути для группы текущего щита, первый после "точки подключений".
        /// </summary>
        /// <param name="currentCon"></param>
        /// <param name="conPanel"></param>
        /// <param name="firstAfterPanelCon"></param>
        /// <returns></returns>
        public IList<XYZ> GetPath(Connector currentCon, Connector conPanel, Connector firstAfterPanelCon)
        {
            //УЗЛЫ ПУТИ
            List<XYZ> listPath = new List<XYZ>();

            // ПЕРВЫЙ УЗЕЛ = xyz коннектора панели, к которой подключаем conPanel.Domain == DB.Domain.DomainElectrical
            // Location.Point панели и положения коннектора не одно и тоже, пробуем положение коннектора.
            LocationPoint locPoint = conPanel.Owner.Location as LocationPoint;
            XYZ node1 = locPoint.Point;
            listPath.Add(node1);

            // ВТОРОЙ УЗЕЛ = под коннектором панели на высоте осевой линии короба
            // И ТРЕТИЙ УЗЕЛ = проекция второго узла на осевую линию короба
            Connector startCon = null;

            // ЕСЛИ БЛИЖАЙШИЙ К ПАНЕЛИ "ТОЧКА ПОДКЛЮЧЕНИЙ" ЭТО КОРОБ
            if (firstAfterPanelCon.Owner is Conduit conduit)
            {
                //первый после панели "точка подключений" коннектор
                // не каждый ближайший до панели элемент является коробом
                // boundLine.Origin.Z = десятичное число с 13 или 14 знаков после
                // запятой, по разному - могут быть неточности

                // осевая линия короба
                LocationCurve locationLineC = conduit.Location as LocationCurve;
                Line boundLineC = locationLineC.Curve as Line;
                Line unboundLineC = Line.CreateUnbound(boundLineC.Origin, boundLineC.Direction);

                XYZ node2 = new XYZ(0, 0, 0);

                // Z меньше плюс-минус 1мм, короб лежит в плоскости XY
                // ЕСЛИ ГОРИЗОНТАЛЬНЫЙ КОРОБ СОВМЕЩАЕМ ПО ОСИ Z в node2
                // если короб идет горизонтально, то Z = 0 (с отклонениями в 10 знаке после запятой)
                if (System.Math.Abs(boundLineC.Direction.Z) < 0.003)
                {
                    // ВТОРОЙ УЗЕЛ над точкой размещения панели на уровне осевой линии короба
                    node2 = new XYZ(node1.X, node1.Y, boundLineC.Origin.Z);
                }

                // ЕСЛИ ВЕРТИКАЛЬНЫЙ КОРОБ СОВМЕЩАЕМ ПО ОСИ Х в node2
                //если короб идет вниз, то Z = -1, вверх Z = 1
                if (System.Math.Abs(boundLineC.Direction.Z) > 0.997)
                {
                    // ВТОРОЙ УЗЕЛ напротив размещения панели на уровне осевой линии короба по оси X
                    node2 = new XYZ(boundLineC.Origin.X, node1.Y, node1.Z);
                }

                // ТРЕТИЙ УЗЕЛ = xyz проекции на ось короба
                XYZ node3 = unboundLineC.Project(node2).XYZPoint;

                listPath.Add(node2);
                listPath.Add(node3);

                // коннектор противоположного конца короба
                Connector nextCon = conduit.ConnectorManager.Connectors
                    .OfType<Connector>()
                    .FirstOrDefault(c => c.Id != firstAfterPanelCon.Id && c.ConnectorType == ConnectorType.End);

                // этот коннектор уже соединительной детали и в следующем if он уже будет обрабатываться
                startCon = nextCon.AllRefs
                    .OfType<Connector>()
                    .FirstOrDefault(sCon => nextCon.Owner.Id != sCon.Owner.Id);
            }

            // ЕСЛИ БЛИЖАЙШИЙ К ПАНЕЛИ "ТОЧКА ПОДКЛЮЧЕНИЙ" ЭТО СОЕДИНИТЕЛЬНАЯ ДЕТАЛЬ
            // то переходим к подключенному к нему коробу и получаем точку пересечения с осью короба
            if (firstAfterPanelCon.Owner.Category.Id == new ElementId(BuiltInCategory.OST_ConduitFitting))
            {
                FamilyInstance fitting = (FamilyInstance)firstAfterPanelCon.Owner;
                Connector nextCon = fitting.MEPModel.ConnectorManager.Connectors
                    .OfType<Connector>()
                    .FirstOrDefault(c => c.Id != firstAfterPanelCon.Id && c.ConnectorType == ConnectorType.End);

                // это коннектор уже короба
                Connector conduitCon = nextCon.AllRefs.OfType<Connector>().First();

                LocationCurve locationboundLine = ((Conduit)conduitCon.Owner).Location as LocationCurve;
                Line boundLine = locationboundLine.Curve as Line;
                Line unboundLine = Line.CreateUnbound(boundLine.Origin, boundLine.Direction);

                // ВТОРОЙ УЗЕЛ под коннектором щита на уровне осевой линии короба
                XYZ node2 = new XYZ(node1.X, node1.Y, boundLine.Origin.Z);
                listPath.Add(node2);

                // ТРЕТИЙ УЗЕЛ = xyz проекции на ось короба
                XYZ node3 = unboundLine.Project(node2).XYZPoint;
                listPath.Add(node3);

                // получаем коннектор этого же короба для продолжения обхода
                startCon = nextCon.AllRefs.OfType<Connector>().First();
            }

            // цикл повторяется пока выполняется условие = коннектор подключен
            while (startCon != null && startCon.IsConnected)
            {
                if (startCon.Owner is Conduit ownerConduit)
                {
                    Connector nextCon = ownerConduit.ConnectorManager.Connectors
                        .OfType<Connector>()
                        .FirstOrDefault(c => c.Id != startCon.Id && c.ConnectorType == ConnectorType.End);

                    if (nextCon.IsConnected)
                    {
                        startCon = nextCon.AllRefs
                            .OfType<Connector>()
                            .FirstOrDefault(nCon => nextCon.Owner.Id != nCon.Owner.Id);
                    }
                    else
                    {
                        // подключаем к одному из 5 коннекторов щита
                        XYZ nodeEndPath = currentCon.CoordinateSystem.Origin;

                        // осевая линия короба
                        LocationCurve locationLineC = ownerConduit.Location as LocationCurve;
                        Line boundLineC = locationLineC.Curve as Line;
                        Line unboundLineC = Line.CreateUnbound(boundLineC.Origin, boundLineC.Direction);

                        XYZ node_2;

                        // Z меньше плюс - минус 1мм, короб лежит в плоскости XY
                        // если короб идет вниз, то Z = -1
                        // ГОРИЗОНТАЛЬНЫЙ КОРОБ
                        if (System.Math.Abs(boundLineC.Direction.Z) < 0.003)
                        {
                            // ПРЕДПОСЛЕДНИЙ(-2) УЗЕЛ над точкой размещения панели на уровне осевой линии короба
                            node_2 = new XYZ(nodeEndPath.X, nodeEndPath.Y, boundLineC.Origin.Z);
                        }

                        // ВЕРТИКАЛЬНЫЙ КОРОБ
                        // если вертикальный короб совмещаем по оси Х в node_2
                        // если короб идет вниз, то Z = -1, вверх Z = 1
                        else
                        {
                            // ПРЕДПОСЛЕДНИЙ(-2) УЗЕЛ напротив размещения панели на уровне осевой линии короба по оси X
                            node_2 = new XYZ(boundLineC.Origin.X, nodeEndPath.Y, nodeEndPath.Z);
                        }

                        // ПРЕДПРЕДПОСЛЕДНИЙ(-3) УЗЕЛ = xyz проекции на ось короба
                        XYZ node_3 = unboundLineC.Project(node_2).XYZPoint;

                        listPath.Add(node_3);
                        listPath.Add(node_2);
                        listPath.Add(nodeEndPath);
                        startCon = nextCon;
                    }
                }

                if (startCon.Owner.Category.Id == new ElementId(BuiltInCategory.OST_ConduitFitting))
                {
                    FamilyInstance fitting = (FamilyInstance)startCon.Owner;
                    Connector nextCon = fitting.MEPModel.ConnectorManager.Connectors
                        .OfType<Connector>()
                        .FirstOrDefault(c => c.Id != startCon.Id && c.ConnectorType == ConnectorType.End);

                    if (nextCon.IsConnected)
                    {
                        // ДОБАВЛЯЕМ В ПУТЬ УЗЕЛ КАК xyz точки размещения соед.детали
                        // так как она является пересечением осевых линий коробов
                        XYZ fittingPoint = ((LocationPoint)fitting.Location).Point;
                        listPath.Add(fittingPoint);

                        startCon = nextCon.AllRefs
                            .OfType<Connector>()
                            .FirstOrDefault(nConf => nConf.Owner.Id != nextCon.Owner.Id);
                    }
                    else
                    {
                        // startCon получили как раз с предыдущего короба
                        Connector conConduit = startCon.AllRefs.OfType<Connector>().First();  // коннектор короба
                        //подключаем к одному из 5 коннекторов щита
                        XYZ nodeEndPath = currentCon.CoordinateSystem.Origin;

                        // осевая линия короба
                        LocationCurve locationBoundLineC = ((Conduit)conConduit.Owner).Location as LocationCurve;
                        Line boundLineC = locationBoundLineC.Curve as Line;
                        Line unboundLineC = Line.CreateUnbound(boundLineC.Origin, boundLineC.Direction);

                        XYZ node_2;

                        // ГОРИЗОНТАЛЬНЫЙ КОРОБ
                        //если короб горизонтальный, то совмещаем по оси Z в node_2
                        //Z меньше плюс - минус 1мм, короб лежит в плоскости XY
                        //если короб идет горизонтально, то Z = 0(с отклонениями в 10 знаке после запятой)
                        if (System.Math.Abs(boundLineC.Direction.Z) < 0.003)
                        {
                            //ПРЕДПОСЛЕДНИЙ(-2) УЗЕЛ над точкой размещения панели на уровне осевой линии короба
                            node_2 = new XYZ(nodeEndPath.X, nodeEndPath.Y, boundLineC.Origin.Z);
                        }
                        // ВЕРТИКАЛЬНЫЙ КОРОБ
                        //если короб вертикальный, то совмещаем по оси Х в node_2
                        //если короб идет вниз, то Z = -1, вверх Z = 1
                        else
                        {
                            //ПРЕДПОСЛЕДНИЙ(-2) УЗЕЛ напротив размещения панели на уровне осевой линии короба по оси X.
                            node_2 = new XYZ(boundLineC.Origin.X, nodeEndPath.Y, nodeEndPath.Z);
                        }

                        //ПРЕДПРЕДПОСЛЕДНИЙ(-3) УЗЕЛ = xyz проекции на ось короба
                        XYZ node_3 = unboundLineC.Project(node_2).XYZPoint;

                        listPath.Add(node_3);
                        listPath.Add(node_2);
                        listPath.Add(nodeEndPath);
                        startCon = nextCon;
                    }
                }
            }

            return listPath;
        }
    }
}
