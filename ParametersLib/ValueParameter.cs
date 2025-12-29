using Autodesk.Revit.DB;

namespace CreateShemPIcosf.MyDll
{
    public class ValueParameter
    {
        public object Get(Parameter parameter)
        {
            if (parameter == null)
                return null;

            if (!parameter.HasValue)
                return null;

            switch (parameter.StorageType)
            {
                case StorageType.Integer:
                    return parameter.AsInteger();

                case StorageType.Double:
                    return parameter.AsDouble();

                case StorageType.String:
                    return parameter.AsString();

                case StorageType.ElementId:
                    return parameter.AsElementId();

                case StorageType.None:
                default:
                    return null;
            }
        }
    }
}
