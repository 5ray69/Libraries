using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// Нагрузки панели
    /// </summary>
    public class LoadsPanel
    {
        ErrorModel errorModel = new();
        // на аргумент load к чему хотим подключить нагрузку

        /// <summary>
        /// <para>Проверяет, является ли load нагрузкой у panel (true), иначе false.</para>
        /// <para>Циклическая ссылка, когда мы пытаемся подключить панель к ее нагрузе.</para>
        /// <para>Для проверки циклической ссылки на аргумент panel подаем то, что хотим подключить,</para>
        /// <para>а на аргумент load - к чему хотим подключить нагрузку</para>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="panel"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        public bool IsLoad(Document doc, FamilyInstance panel, FamilyInstance load)
        {
            if (panel == null || load == null)
                return false;

            if (panel.Category.Id.IntegerValue != (int)BuiltInCategory.OST_ElectricalEquipment)
            {
                errorModel.UserWarning(new NoTargetCategory().MessageForUser(doc, panel));
            }


            // Список Id всех нагрузок panel
            List<ElementId> listIdLoads = new List<ElementId>();

            ConnectorSet connectors = panel.MEPModel.ConnectorManager.Connectors;

            // Перебираем коннекторы отходящих цепей у panel/панели 
            foreach (Connector connector in connectors)
            {
                if (connector.Domain == Domain.DomainUndefined) //коннектор отходящей цепи, у питающей цепи Domain = DomainElectrical
                {
                    //получаем коннектор на другом конце цепи
                    //один или несколько, так как нагрузок может быть несколько
                    ConnectorSet allRefs = connector.AllRefs;

                    // Перебираем коннекторы, в основном один, но может быть и несколько
                    foreach (Connector conRef in allRefs)
                    {
                        ElectricalSystem electricalSystem = conRef.Owner as ElectricalSystem;

                        ElementSetIterator iterator = electricalSystem.Elements.ForwardIterator();
                        while (iterator.MoveNext())
                        {
                            if (iterator.Current is FamilyInstance familyInstance)
                            {
                                listIdLoads.Add(familyInstance.Id);
                            }
                        }
                    }
                }
            }

            // Проверка: load содержится в списке нагрузок?
            return listIdLoads.Contains(load.Id);
        }
    }
}
