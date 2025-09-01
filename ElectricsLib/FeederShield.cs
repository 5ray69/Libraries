using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace ElectricsLib
{
    public class FeederShield
    {
        private readonly Document _doc;
        private readonly ScheduleKey _scheduleKey;

        public FeederShield(Document doc)
        {
            _doc = doc;
            _scheduleKey = new ScheduleKey(doc, "Ключи спецификация кабелей");
        }

        public void SetTypeCable(FamilyInstance shieldFloor, string cable, string penCable, bool rewriteParameters)
        {
            if (shieldFloor == null)
                return; // Сам объект обязателен, иначе не к чему применять параметры

            int count = 1;

            foreach (Connector connector in shieldFloor.MEPModel.ConnectorManager.Connectors)
            {
                if (connector.ConnectorType != ConnectorType.End)
                    continue;

                foreach (Connector connectr in connector.AllRefs)
                {
                    if (connectr?.Owner is not ElectricalSystem feederElectricalSystem)
                        continue;

                    Parameter paramTypeCable = feederElectricalSystem.LookupParameter("Тип кабеля");
                    //if (paramTypeCable == null)
                    //    continue;

                    //или перезаписываем, или значение параметра пусто
                    bool shouldWrite = rewriteParameters || IsParameterEmpty(paramTypeCable);

                    //if (!shouldWrite)
                    //    continue;

                    // Пропуск, если значения не заданы
                    if (count < 5 && string.IsNullOrWhiteSpace(cable))
                        continue;

                    if (count >= 5 && string.IsNullOrWhiteSpace(penCable))
                        continue;

                    if (shouldWrite)
                    {
                        //в семействе ЩЭ пять реальных коннекторов
                        ElementId cableId = (count < 5)
                            ? _scheduleKey.GetIdElementKey(cable)
                            : _scheduleKey.GetIdElementKey(penCable);

                        paramTypeCable.Set(cableId);
                        count++;
                    }
                }
            }
        }

        private bool IsParameterEmpty(Parameter param)
        {
            ElementId id = param.AsElementId();
            //параметр не заполнялся или установлено значение (нет)
            return id == null || id == ElementId.InvalidElementId;
        }
    }
}


//ScheduleKey scheduleKey = new(doc, "Ключи спецификация кабелей");

//public void SetTypeCable(FamilyInstance shieldFloor, string cable, string penCable, bool rewriteParameters)
//{
//    int count = 1;
//    ConnectorSet connectorSet = shieldFloor.MEPModel.ConnectorManager.Connectors;
//    ConnectorSetIterator connectorSetIterator = connectorSet.ForwardIterator();
//    while (connectorSetIterator.MoveNext())
//    {
//        Connector connector = connectorSetIterator.Current as Connector;
//        //если коннектор реальный, а не виртуальный, то это питающая линия
//        if (connector.ConnectorType == ConnectorType.End)
//        {
//            //в семействе ЩЭ пять реальных коннекторов
//            ConnectorSetIterator iteratorEnd = connector.AllRefs.ForwardIterator();
//            while (iteratorEnd.MoveNext())
//            {
//                Connector connectorEnd = iteratorEnd.Current as Connector;
//                ElectricalSystem feederElectricalSystem = connectorEnd.Owner as ElectricalSystem;


//                Parameter paramTypeCable = feederElectricalSystem.LookupParameter("Тип кабеля");
//                if (rewriteParameters)
//                {
//                    if (count < 5)
//                    {
//                        paramTypeCable.Set(scheduleKey.GetIdElementKey(cable));
//                        count += 1;
//                    }
//                    else
//                    {
//                        paramTypeCable.Set(scheduleKey.GetIdElementKey(penCable));
//                    }
//                }

//                else
//                {
//                    //записываем только в пустые значения параметра
//                    if (paramTypeCable.AsElementId() == null || paramTypeCable.AsElementId() == ElementId.InvalidElementId)
//                    {
//                        if (count < 5)
//                        {
//                            paramTypeCable.Set(scheduleKey.GetIdElementKey(cable));
//                            count += 1;
//                        }
//                        else
//                        {
//                            paramTypeCable.Set(scheduleKey.GetIdElementKey(penCable));
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
