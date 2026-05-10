using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.ElectricsLib.UserWarningElectricsLib;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// валидатор правильности заполнения значения параметра Имя панели
    /// </summary>
    /// <param name="errorModel"></param>
    public class PanelNameValidator(ErrorModel errorModel)
    {
        private readonly ErrorModel _errorModel = errorModel;


        /// <summary>
        /// <para>из исходной строки возвращает подстроку,
        /// <para>отбрасив все после последей точки в строке,
        /// <para>включая саму точку
        /// </summary>
        /// <returns></returns>
        public string GetGroupName(Document doc, FamilyInstance baseEquipment, string panelName)
        {
            if (string.IsNullOrEmpty(panelName))  // никогда не заполнялась или пустая строка
            {
                _errorModel.UserWarning(new EmptyParameter().MessageForUser(doc, baseEquipment));
            }

            int lastPointIndex = panelName.LastIndexOf('.');

            if (lastPointIndex == -1)  // в строке нет точки
            {
                _errorModel.UserWarning(new NotPointInPanelName().MessageForUser(doc, baseEquipment));
            }

            if (panelName.Length - lastPointIndex - 1 != 2)  // после последней точки больше или меньше двух символов
            {
                _errorModel.UserWarning(new IncorrectNumberOfLetters().MessageForUser(doc, baseEquipment));
            }

            return panelName.Substring(0, lastPointIndex);

        }
    }
}
