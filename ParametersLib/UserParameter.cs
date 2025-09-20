using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib.UserWarningParametersLib;

namespace Libraries.ParametersLib
{
    public class UserParameter
    {
        public void ValueToNull(Element el, string str)
        {
            Parameter parameter = el.LookupParameter(str);

            if (parameter != null)
            {
                StorageType storageType = parameter.StorageType;

                if (storageType == StorageType.Integer)
                    parameter.Set(0);
                if (storageType == StorageType.Double)
                    parameter.Set(0.0);
                if (storageType == StorageType.String)
                    parameter.Set("");
                if (storageType == StorageType.ElementId)
                    parameter.Set(new ElementId(-1));
            }

            else
            {
                ErrorModel errorModel = new();
                errorModel.UserWarning(new ParameterElementMissing().MessageForUser(el, str));
            }
        }
    }
}


//private bool IsParamEmpty(Parameter param)
//{
//    if (param == null || !param.HasValue)
//        return true;

//    switch (param.StorageType)
//    {
//        case StorageType.ElementId:
//            ElementId id = param.AsElementId();
//            return id == null || id == ElementId.InvalidElementId;

//        case StorageType.String:
//            string str = param.AsString();
//            return string.IsNullOrWhiteSpace(str);

//        case StorageType.Integer:
//            int intVal = param.AsInteger();
//            return intVal == 0;

//        case StorageType.Double:
//            double dblVal = param.AsDouble();
//            return dblVal == 0.0;

//        case StorageType.None:
//        default:
//            return true;
//    }
//}



//if (!parameter.HasValue)
//    return true;

//if (parameter.StorageType == StorageType.Integer)
//    return parameter.AsInteger() == 0;

//if (parameter.StorageType == StorageType.Double)
//    return parameter.AsDouble() == 0.0;

//if (parameter.StorageType == StorageType.String)
//    return string.IsNullOrWhiteSpace(parameter.AsString());

//if (parameter.StorageType == StorageType.ElementId)
//    return parameter.AsElementId() == ElementId.InvalidElementId;

//// если вдруг что-то новое — считаем "пустым"
//return true;