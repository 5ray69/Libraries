using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib.UserWarningParametersLib;

namespace Libraries.ParametersLib
{
    public class CopyValueParameter
    {
        private readonly ErrorModel _errorModel;
        private readonly ParameterValidatorIsMissing _parameterValidator;


        public CopyValueParameter()
        {
            _errorModel = new ErrorModel();
            _parameterValidator = new ParameterValidatorIsMissing(_errorModel);
        }


        /// <summary>
        /// <para>Копирует значение параметра из одного объекта
        /// <para>в значение такого же параметра другого объекта.
        /// <para>Перед копированием проверяется наличие параметра
        /// <para>в обеих объектах и не только ли для чтения они
        /// </summary>
        /// <param name="sourceElement"></param>
        /// <param name="targetElement"></param>
        /// <param name="nameParameter"></param>
        public void FromOneToAnother(Element sourceElement, Element targetElement, string nameParameter)
        {
            Parameter sourceParam = _parameterValidator.ValidateExistsParameter(sourceElement, nameParameter);
            Parameter targetParam = _parameterValidator.ValidateValueParameter(targetElement, nameParameter);

            if (sourceParam.StorageType != targetParam.StorageType)
            {
                _errorModel.UserWarning(
                    new ErrorStorageType().MessageForUser(sourceElement, targetElement, nameParameter)
                );
                return;
            }

            switch (sourceParam.StorageType)
            {
                case StorageType.String:
                    targetParam.Set(sourceParam.AsString());
                    break;

                case StorageType.ElementId:
                    targetParam.Set(sourceParam.AsElementId());
                    break;

                case StorageType.Double:
                    targetParam.Set(sourceParam.AsDouble());
                    break;

                case StorageType.Integer:
                    targetParam.Set(sourceParam.AsInteger());
                    break;
            }
        }



        /// <summary>
        /// <para>Копирует значение параметра из одного объекта
        /// <para>в значение такого же параметра другого объекта.
        /// <para>Перед копированием проверяется наличие параметра
        /// <para>в обеих объектах и не только ли для чтения они
        /// </summary>
        /// <param name="sourceElement"></param>
        /// <param name="parameterNameSource"></param>
        /// <param name="targetElement"></param>
        /// <param name="parameterNameTarget"></param>
        public void TwoDifferentParamers(Element sourceElement, string parameterNameSource, Element targetElement, string parameterNameTarget)
        {
            Parameter sourceParam = _parameterValidator.ValidateExistsParameter(sourceElement, parameterNameSource);
            Parameter targetParam = _parameterValidator.ValidateValueParameter(targetElement, parameterNameTarget);

            if (sourceParam.StorageType != targetParam.StorageType)
            {
                _errorModel.UserWarning(
                    new ErrorStorageType().MessageForUser(sourceElement, targetElement, $@"{parameterNameSource} и {parameterNameTarget}")
                );
                return;
            }

            switch (sourceParam.StorageType)
            {
                case StorageType.String:
                    targetParam.Set(sourceParam.AsString());
                    break;

                case StorageType.ElementId:
                    targetParam.Set(sourceParam.AsElementId());
                    break;

                case StorageType.Double:
                    targetParam.Set(sourceParam.AsDouble());
                    break;

                case StorageType.Integer:
                    targetParam.Set(sourceParam.AsInteger());
                    break;
            }
        }



    }
}



//public void FromOneToAnother(Element sourceElement, Element targetElement, string nameParameter)
//{
//    #region СОЗДАЕМ ЭКЗЕМПЛЯРЫ КЛАССОВ
//    ErrorModel errorModel = new();
//    ParameterValidatorIsMissing parameterValidatorIsMissing = new(errorModel);
//    #endregion

//    Parameter sourceParam = parameterValidatorIsMissing.ValidateValueParameter(sourceElement, nameParameter);
//    Parameter targetParam = parameterValidatorIsMissing.ValidateValueParameter(targetElement, nameParameter);


//    if (sourceParam.StorageType == targetParam.StorageType)
//    {
//        switch (sourceParam.StorageType)
//        {
//            case StorageType.String:
//                string strVal = sourceParam.AsString();
//                targetParam.Set(strVal);
//                break;

//            case StorageType.ElementId:
//                ElementId idVal = sourceParam.AsElementId();
//                targetParam.Set(idVal);
//                break;

//            case StorageType.Double:
//                double dblVal = sourceParam.AsDouble();
//                targetParam.Set(dblVal);
//                break;

//            case StorageType.Integer:
//                int intVal = sourceParam.AsInteger();
//                targetParam.Set(intVal);
//                break;
//        }
//    }

//    else
//    {
//        errorModel.UserWarning(new ErrorStorageType().MessageForUser(sourceElement, targetElement, nameParameter));
//    }

//}