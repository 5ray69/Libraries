using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib.UserWarningParametersLib;
using System;

namespace CalculationGroups.MyDll.Work
{
    /// <summary>
    /// <para>для получения Definition один раз,</para>
    /// <para>чтоб не получать значение параметра по LookupParameter</para>
    /// </summary>
    /// <param name="document"></param>
    /// <param name="errorModel"></param>
    public class ParameterDefinition
    {
        //private readonly Document _doc = doc;
        //private readonly ErrorModel _errorModel = errorModel;
        private readonly ValidatorParameter _validatorParameter;

        public ParameterDefinition(Document doc, ErrorModel errorModel)
        {
            //_doc = doc;
            //_errorModel = errorModel;
            _validatorParameter = new ValidatorParameter(doc, errorModel);
        }



        /// <summary>
        /// получаем Definition один раз, чтоб не получать значение параметра по LookupParameter
        /// </summary>
        /// <param name="element">элемент у которого должен извлекаться параметр</param>
        /// <param name="paramName">имя параметра</param>
        /// <returns>Definition параметра</returns>
        public Definition Get(Element element, string paramName)
        {
            Parameter param = _validatorParameter.MissingWarning(element, paramName);

            return param.Definition;
        }



        /// <summary>
        /// получаем Definition один раз, чтоб не получать значение параметра по BuiltInParameter
        /// </summary>
        /// <param name="element">элемент у которого должен извлекаться параметр</param>
        /// <param name="builtInParameter">имя параметра</param>
        /// <returns>Definition параметра</returns>
        public Definition Get(Element element, BuiltInParameter builtInParameter)
        {

            //string paramName = LabelUtils.GetLabelFor(builtInParameter);
            //Parameter param = element.get_Parameter(builtInParameter);
            Parameter param = _validatorParameter.MissingWarning(element, builtInParameter);

            //string bipName = "RBS_ELEC_VOLTAGE";
            //if (Enum.TryParse(bipName, out BuiltInParameter bip))
            //{
            //    Parameter p = element.get_Parameter(bip);
            //    if (p != null)
            //    {
            //        // всё работает
            //        Definition def = p.Definition;
            //    }
            //}
            //else
            //{
            //    // Ошибка: такого BuiltInParameter нет
            //}

            return param.Definition;

        }

    }
}
