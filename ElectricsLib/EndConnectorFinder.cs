using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace CreateRisersShield.MyDll
{
    /// <summary>
    /// <para>Класс, выполняющий поиск последних не подключенных коннекторов,</para>
    /// <para>ближайших к заданной панели в пределах указанного расстояния.</para>
    /// </summary>
    public class EndConnectorFinder
    {
        /// <summary>
        /// <para>Возвращает словарь, где ключ — имя группы коннекторов,</para>
        /// <para>а значение — последний не подключенный коннектор,</para>
        /// <para>ближайший к панели в пределах заданного расстояния.</para>
        /// </summary>
        /// <param name="panelToConnection"></param>
        /// <param name="groupsOfConnectors"></param>
        /// <param name="lengthMillimeters"></param>
        /// <returns></returns>
        public Dictionary<string, Connector> GetEndConnector(
                                                FamilyInstance panelToConnection,
                                                GroupsOfConnectors groupsOfConnectors,
                                                int lengthMillimeters)
        {
            Dictionary<string, Connector> endConnector = new();

            // Расположение панели на плоскости XY, к которой подключаем цепи
            XYZ panelPlaneXY = panelToConnection.Location is LocationPoint lp
                ? new XYZ(lp.Point.X, lp.Point.Y, 0)
                : null;

            if (panelPlaneXY == null)
                return endConnector;

            // Конвертация расстояния из миллиметров во внутренние единицы (футы)
            double lengthFeets = UnitUtils.ConvertToInternalUnits(lengthMillimeters, UnitTypeId.Millimeters);

            // Итерируем по группам, чтобы из всех коннекторов найти
            // последний не подключенный, ближайший к панели
            foreach (var group in groupsOfConnectors)
            {
                string groupName = group.Key;               // имя группы коннекторов
                List<Connector> connectors = group.Value;   // все коннекторы группы
                List<Connector> endConnectors = new();      // список неподключенных конечных коннекторов

                foreach (Connector con in connectors)
                {
                    // не подключенный коннектор
                    if (!con.IsConnected)
                    {
                        // коннектор короба или соединительной детали
                        // должен находиться не далее lengthFeets от панели в плоскости XY
                        XYZ connectorXY = new XYZ(con.CoordinateSystem.Origin.X, con.CoordinateSystem.Origin.Y, 0);

                        if (panelPlaneXY.DistanceTo(connectorXY) < lengthFeets)
                        {
                            endConnectors.Add(con);
                        }
                    }
                }

                // Первый попавшийся подходящий коннектор (если их несколько)
                endConnector[groupName] = endConnectors.FirstOrDefault();
            }

            return endConnector;
        }
    }
}
