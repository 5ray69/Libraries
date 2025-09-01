using Autodesk.Revit.DB;
using ErrorModelLib;
using ParametersLib.UserWarningParametersLib;

namespace ParametersLib
{
    public class ParameterValidatorIsMissing(ErrorModel errorModel)
    {
        private readonly ErrorModel _errorModel = errorModel;


        /// <summary>
        /// <para>проверяет существует ли параметр и что он не только для чтения
        /// <para>и присваиваем параметру значение, LookupParameter
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>

        public void ValidateAndSetParameter(Element element, string parameterName, string value)
        {
            Parameter parameter = element.LookupParameter(parameterName);

            if (parameter == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, parameterName));
                return; // Завершаем выполнение, если параметр не найден
            }

            if (parameter.IsReadOnly)
            {
                _errorModel.UserWarning(new ParameterIsReadOnly().MessageForUser(element, parameterName));
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
