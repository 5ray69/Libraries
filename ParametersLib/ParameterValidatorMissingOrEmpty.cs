using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib.UserWarningParametersLib;
using System.Windows.Controls;

namespace Libraries.ParametersLib
{
    /// <summary>
    /// Проверяет существует ли параметр и есть ли у него значение
    /// </summary>
    public class ParameterValidatorMissingOrEmpty
    {
        private readonly Document _doc;
        private readonly ErrorModel _errorModel;
        private readonly ParameterValidatorForEmpty _emptyValueValidator;


        public ParameterValidatorMissingOrEmpty(Document doc, ErrorModel errorModel)
        {
            _doc = doc;
            _errorModel = errorModel;
            _emptyValueValidator = new ParameterValidatorForEmpty(errorModel);
        }


        /// <summary>
        /// <para> Проверяет существует ли параметр и что у него есть значение. </para>
        /// <para> В каждом соответствующем случае выводит </para>
        /// <para> предупреждение пользователю и завершает код </para>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameParameter"></param>
        public void ValidateAndWarning(Element element, string nameParameter)
        {
            Parameter parameter = element.LookupParameter(nameParameter);

            //если у элемента нет параметра, то выведет предупреждение пользователю и завершит код
            if (parameter == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, nameParameter));
            }

            bool validatorEmptyValue = _emptyValueValidator.IsParameterEmpty(parameter);
            //если у параметра элемента нет значения, то выведет предупреждение пользователю и завершит код
            if (validatorEmptyValue)
            {
                _errorModel.UserWarning(new ParameterElementAtLevelEmpty().MessageForUser(_doc, element, nameParameter));
            }
        }
    }
}