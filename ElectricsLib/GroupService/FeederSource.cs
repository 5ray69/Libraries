using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;

namespace Libraries.ElectricsLib.GroupService
{
    /// <summary>
    /// питающий источник
    /// </summary>
    /// <param name="document"></param>
    /// <param name="errorModel"></param>
    public class FeederSource(Document document, ErrorModel errorModel)
    {
        private readonly FindHeadPanel _findHeadPanel = new(document, errorModel);
        private readonly FeederCircuit _feederCircuit = new(document, errorModel);


        /// <summary>
        /// возвращает головную панель начиная с цепи
        /// </summary>
        /// <param name="circuit"></param>
        /// <returns></returns>
        public FamilyInstance GetHeadPanelFromCircuit(ElectricalSystem circuit)
        {
            FamilyInstance headPanel = _findHeadPanel.Get(circuit);

            return headPanel;
        }


        /// <summary>
        /// возвращает головную панель начиная с панели
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public FamilyInstance GetHeadPanelFromPanel(FamilyInstance panel)
        {
            ElectricalSystem circuit = _feederCircuit.Get(panel);

            //если у панели нет питающей цепи, то возвращаем панель как головную
            if (circuit == null)
            {
                return panel;
            }

            FamilyInstance headPanel = _findHeadPanel.Get(circuit);

            return headPanel;
        }
    }
}

