using Autodesk.Revit.UI;

namespace Libraries.ErrorModelLib
{
    public class ErrorModel
    {
        public void UserWarning(string message)
        {
            TaskDialog.Show("Внимание!", message);
            throw new UserNotificationException();  // Останавливает выполнение с исключением
        }
    }
}
