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
        /// <param name="image">путь к изображению</param>
        public WarningWindow(string message, BitmapImage image = null)
        {
            InitializeComponent();
            TextMessage.Text = message;

            if (image != null)
                DialogImage.Source = image;
            else
                DialogImage.Visibility = Visibility.Collapsed;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
