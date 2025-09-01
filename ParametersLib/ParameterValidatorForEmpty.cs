using Autodesk.Revit.DB;
using ErrorModelLib;
using ParametersLib.UserWarningParametersLib;

namespace ParametersLib
{
    public class ParameterValidatorForEmpty(ErrorModel errorModel)
    {
        private readonly ErrorModel _errorModel = errorModel;
        private readonly ParameterValidatorIsMissing _parameterValidatorIsMissing = new(errorModel);


        /// <summary>
        /// <para>LookupParameter, тип данных параметра string
        /// <para>проверяет существует ли параметр и что он не только для чтения
        /// <para>и присваивает параметру значение в том случае, если параметр не заполнен
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

            if (string.IsNullOrEmpty(parameter.AsString())) // if (paramElemFlat.AsString() == null || paramElemFlat.As<para>() == "")
            {
                parameter.Set(value); // Устанавливаем значение, если оно ещё не задано
            }
        }




        /// <summary>
        /// <para>LookupParameter
        /// <para>проверяет заполнен ли параметр
        /// <para>и если не заполнен, то выводит ошибку
        /// <para>с указанием элемента и этажа
        /// <para>и завершает код
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element"></param>
        /// <param name="parameterName"></param>
        public Parameter ElementAtLevel(Document doc, Element element, string parameterName)
        {
            Parameter parameter = _parameterValidatorIsMissing.ValidateExistsParameter(element, parameterName);

            if (IsParameterEmpty(parameter))
            {
                string message = new ParameterElementAtLevelEmpty().MessageForUser(doc, element, parameterName);
                _errorModel.UserWarning(message);
                return null; // Завершаем выполнение, если параметр не найден
            }

            return parameter;
        }


        /// <summary>
        /// Определяет, является ли параметр пустым в зависимости от его типа
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool IsParameterEmpty(Parameter parameter)
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

//switch (parameter.StorageType)
//{
//    case StorageType.Integer:
//        return parameter.AsInteger() == 0;

//    case StorageType.Double:
//        return parameter.AsDouble() == 0;

//    case StorageType.String:
//        string value = parameter.AsString();
//        return string.IsNullOrWhiteSpace(value);

//    case StorageType.ElementId:
//        return parameter.AsElementId() == ElementId.InvalidElementId;

//    default:
//        return true;
//}