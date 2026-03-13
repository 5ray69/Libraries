using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace Libraries.ErrorModelLib
{
    public class ErrorModel
    {
        /// <summary>
        /// Для простого текстового сообщения через TaskDialog
        /// </summary>
        /// <param name="message">Сообщение для пользователя</param>
        /// <exception cref="UserNotificationException"></exception>
        public void UserWarning(string message)
        {
            TaskDialog.Show("Внимание!", message);
            throw new UserNotificationException();  // Останавливает выполнение с исключением
        }


        /// <summary>
        /// Для окна с текстом и изображенияем
        /// </summary>
        /// <param name="message">Сообщение для пользователя</param>
        /// <param name="image">Изображение для окна (BitmapImage). Если null — окно без картинки.</param>
        /// <exception cref="UserNotificationException"></exception>
        public void UserWarningWindow(string message, BitmapImage image = null)
        {
            var windowService = new WarningWindowService();
            windowService.Show(message, image);

            throw new UserNotificationException();  // Останавливает выполнение с исключением
        }
    }
}
