using Autodesk.Revit.DB;
using ElectricsLib.UserWarningElectricsLib;
using ErrorModelLib;

namespace ElectricsLib
{
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
            if (!string.IsNullOrEmpty(panelName))
            {
                int lastPointIndex = panelName.LastIndexOf('.'); // индекс последней точки

                if (lastPointIndex == -1)
                {
                    _errorModel.UserWarning(new NotPointInPanelName().MessageForUser(doc, baseEquipment));
                    return ""; // завершаем выполнение, если в строке нет точки
                }
                else
                    return panelName.Substring(0, lastPointIndex);
            }

            else
            {
                _errorModel.UserWarning(new EmptyParameter().MessageForUser(doc, baseEquipment));
                return ""; // завершаем выполнение, если строка пустая
            }
        }
    }
}
