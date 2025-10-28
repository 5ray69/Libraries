using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib.UserWarningParametersLib;

namespace Libraries.ParametersLib
{
    public class ParameterValidatorIsMissing(ErrorModel errorModel)
    {
        private readonly ErrorModel _errorModel = errorModel;


        /// <summary>
        /// <para> проверяет существует ли параметр и что он не только для чтения </para>
        /// <para> и присваиваем параметру значение, LookupParameter </para>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameParameter"></param>
        /// <param name="value"></param>
        public void ValidateAndSetParameter(Element element, string nameParameter, string value)
        {
            Parameter parameter = element.LookupParameter(nameParameter);

            if (parameter == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, nameParameter));
                return; // Завершаем выполнение, если параметр не найден
            }

            if (parameter.IsReadOnly)
            {
                _errorModel.UserWarning(new ParameterIsReadOnly().MessageForUser(element, nameParameter));
                return; // Завершаем выполнение, если параметр только для чтения
            }

            parameter.Set(value);
        }



        /// <summary>
        /// проверяет существует ли параметр и что он не только для чтения, LookupParameter
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public Parameter ValidateValueParameter(Element element, string parameterName)
        {
            Parameter parameter = element.LookupParameter(parameterName);

            if (parameter == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, parameterName));
                return null; // Завершаем выполнение, если параметр не найден
            }

            if (parameter.IsReadOnly)
            {
                _errorModel.UserWarning(new ParameterIsReadOnly().MessageForUser(element, parameterName));
                return null; // Завершаем выполнение, если параметр только для чтения
            }

            return parameter;
        }




        /// <summary>
        /// проверяет существует ли параметр, LookupParameter
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public Parameter ValidateExistsParameter(Element element, string parameterName)
        {
            Parameter parameter = element.LookupParameter(parameterName);

            if (parameter == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, parameterName));
                return null; // Завершаем выполнение, если параметр не найден
            }

            return parameter;
        }



    }
}
