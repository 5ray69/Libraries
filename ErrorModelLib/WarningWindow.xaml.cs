using System.Windows;
using System.Windows.Media.Imaging;

namespace Libraries.ErrorModelLib
{
    /// <summary>
    /// Логика взаимодействия для WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        /// <summary>
        /// конструктор класса с двумя аргуменами
        /// </summary>
        /// <param name="message">текст предупреждения пользователю</param>
        /// <param name="bitmapImage">путь к изображению</param>
        public WarningWindow(string message, BitmapImage bitmapImage = null)
        {
            InitializeComponent();
            TextMessage.Text = message;

            if (bitmapImage != null)
                DialogImage.Source = bitmapImage;
            else
                DialogImage.Visibility = Visibility.Collapsed;

            // Пользователи могут работать на очень разных мониторах — FullHD, 2K, 4K,
            // ноутбуки с масштабированием Windows 125–200 %.
            // Если окно просто ограничить фиксированными значениями(MaxWidth= "900"),
            // то на маленьком экране оно может выйти за пределы, а на большом будет выглядеть слишком маленьким.
            // Ограничиваем размер окна относительно экрана:
            // максимум 80 % высоты экрана
            // максимум 80% ширины экрана
            MaxWidth = SystemParameters.WorkArea.Width * 0.8;
            MaxHeight = SystemParameters.WorkArea.Height * 0.8;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
