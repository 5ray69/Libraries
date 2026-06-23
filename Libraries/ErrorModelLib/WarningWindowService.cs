using System.Windows.Media.Imaging;

namespace Libraries.ErrorModelLib
{
    /// <summary>
    /// Срвис окон в системе предупреждений о ошибках
    /// </summary>
    public class WarningWindowService
    {
        /// <summary>
        /// <para>Показывает окно с текстом и изображением.</para>
        /// <para>Показывает окно с текстом и изображением.</para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="bitmapImage"></param>
        public void Show(string message, BitmapImage bitmapImage = null)
        {
            var window = new WarningWindow(message, bitmapImage);
            window.ShowDialog();
        }
    }
}
