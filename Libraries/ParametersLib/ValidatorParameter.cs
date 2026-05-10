using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib.UserWarningParametersLib;

namespace Libraries.ParametersLib
{
    public class ValidatorParameter
    {
        private readonly Document _doc;
        private readonly ErrorModel _errorModel;

        public ValidatorParameter(Document doc, ErrorModel errorModel)
        {
            _doc = doc;
            _errorModel = errorModel;
        }


        /// <summary>
        /// <para> Использует LookupParameter </para>
        /// <para> Проверяет существует ли параметр </para>
        /// <para> выводит предупреждение пользователю и завершает код. </para>
        /// </summary>
        /// <param name="element">элемент у которого извлекается параметр</param>
        /// <param name="nameParameter"> имя параметра</param>
        /// <returns>Parameter</returns>
        public Parameter MissingWarning(Element element, string nameParameter)
        {
            Parameter param = element.LookupParameter(nameParameter);

            //если у элемента нет параметра, то выведет предупреждение пользователю и завершит код
            if (param == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, nameParameter));
            }

            return param;
        }



        /// <summary>
        /// <para> Использует BuiltInParameter.</para>
        /// <para> Проверяет существует ли параметр.</para>
        /// </summary>
        /// <param name="element">элемент у которого извлекается параметр</param>
        /// <param name="builtInParameter">аргумент в виде BuiltInParameter.RBS_ELEC_VOLTAGE</param>
        /// <returns>Parameter</returns>
        public Parameter MissingWarning(Element element, BuiltInParameter builtInParameter)
        {
            Parameter param = element.get_Parameter(builtInParameter);

            string nameParameter = LabelUtils.GetLabelFor(builtInParameter);

            //если у элемента нет параметра, то выведет предупреждение пользователю и завершит код
            if (param == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, nameParameter));
            }

            return param;
        }



        /// <summary>
        /// <para> Использует LookupParameter </para>
        /// <para> Проверяет: 1.существует ли параметр и 2.что у него есть значение. </para>
        /// <para> В каждом соответствующем случае выводит </para>
        /// <para> предупреждение пользователю и завершает код. </para>
        /// </summary>
        /// <param name="element">элемент у которого извлекается параметр</param>
        /// <param name="nameParameter"> имя параметра</param>
        /// <returns>Parameter</returns>
        public Parameter MissingAndEmptyWarning(Element element, string nameParameter)
        {
            Parameter param = element.LookupParameter(nameParameter);

            //если у элемента нет параметра, то выведет предупреждение пользователю и завершит код
            if (param == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, nameParameter));
            }

            //если у параметра элемента нет значения, то выведет предупреждение пользователю и завершит код
            if (IsParameterEmpty(param))
            {
                _errorModel.UserWarning(new ParameterElementAtLevelEmpty().MessageForUser(_doc, element, nameParameter));
            }

            return param;
        }



        /// <summary>
        /// <para> Использует Definition </para>
        /// <para> Проверяет: 1.существует ли параметр и 2.что у него есть значение. </para>
        /// <para> В каждом соответствующем случае выводит </para>
        /// <para> предупреждение пользователю и завершает код. </para>
        /// </summary>
        /// <param name="element">элемент у которого извлекается параметр</param>
        /// <param name="nameParameter"> имя параметра</param>
        /// <returns>Parameter</returns>
        public Parameter MissingAndEmptyWarning(Element element, Definition paramDefinition)
        {
            Parameter param = element.get_Parameter(paramDefinition);

            string nameParameter = paramDefinition.Name;

            //если у элемента нет параметра, то выведет предупреждение пользователю и завершит код
            if (param == null)
            {
                _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(element, nameParameter));
            }

            //если у параметра элемента нет значения, то выведет предупреждение пользователю и завершит код
            if (IsParameterEmpty(param))
            {
                _errorModel.UserWarning(new ParameterElementAtLevelEmpty().MessageForUser(_doc, element, nameParameter));
            }

            return param;
        }



        /// <summary>
        /// Определяет, является ли параметр пустым или равным нулю в зависимости от его типа
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool IsParameterEmpty(Parameter parameter)
        {
            if (!parameter.HasValue)
                return true;

            if (parameter.StorageType == StorageType.Integer)
                return parameter.AsInteger() == 0;

            if (parameter.StorageType == StorageType.Double)
                return parameter.AsDouble() == 0.0;

            if (parameter.StorageType == StorageType.String)
                return string.IsNullOrWhiteSpace(parameter.AsString());

            if (parameter.StorageType == StorageType.ElementId)
                return parameter.AsElementId() == ElementId.InvalidElementId;

            // если вдруг что-то новое в StorageType — считаем "пустым"
            return true;
        }

    }
}
