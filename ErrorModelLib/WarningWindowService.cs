using System.Windows.Media.Imaging;

namespace Libraries.ErrorModelLib
{
    public class WarningWindowService
    {
        /// <summary>
        /// <para>Показывает окно с текстом и изображением.</para>
        /// <para>BitmapImage создаётся вне сервиса.</para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="image"></param>
        public void Show(string message, BitmapImage image = null)
        {
            var window = new WarningWindow(message, image);
            window.ShowDialog();
        }
    }
}
