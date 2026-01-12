using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using Libraries.RoomsLib.UserWarningRoomsLib;
using System;



namespace Libraries.RoomsLib
{
    public class LocationAnyObject()
    {
        public Object GetSpatialElementOrLocationXyzOrLine(Element element)
        {
            if (element is FamilyInstance familyInstance)
            {
                if (familyInstance.HasSpatialElementCalculationPoint)
                    return familyInstance.GetSpatialElementCalculationPoint();

                else if (familyInstance.Location is LocationPoint locationPoint)
                    return locationPoint.Point;

                else if (familyInstance.Location is LocationCurve locationCurve)
                    return locationCurve.Curve;
            }
            else
            {
                if (element.Location is LocationPoint locatPoint)
                {
                    return locatPoint.Point;
                }
                else if (element.Location is LocationCurve locationCurve)
                {
                    return locationCurve.Curve;
                }
            }


            if (element is ElectricalSystem electricalSystem)
            {
                var baseEquipment = electricalSystem.BaseEquipment;
                if (baseEquipment == null) // цепь не подключена "код завершил работу" с предупреждением пользователю
                {
                    ErrorModel errorModel = new();
                    errorModel.UserWarning(new NoConnectCircuit().MessageForUser(electricalSystem));
                }

                if (baseEquipment.HasSpatialElementCalculationPoint)
                    return baseEquipment.GetSpatialElementCalculationPoint();

                else if (baseEquipment.Location is LocationPoint locationPoint)
                    return locationPoint.Point;

                else if (baseEquipment.Location is LocationCurve locationCurve)
                    return locationCurve.Curve;
            }


            if (element.Location is null) // "код завершил работу" с предупреждением пользователю
            {
                ErrorModel errorModel = new();
                errorModel.UserWarning(new NoContainLocation().MessageForUser(element));
            }


            return null;
        }
    }
}
