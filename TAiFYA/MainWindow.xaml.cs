using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TAiFYA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Analizator analizator = new Analizator();

        AnalizatorResponse? res = null;
        public MainWindow()
        {
            
            InitializeComponent();
            AnalizeButton.IsEnabled = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            analizator.ChangeInput(InputField.Text);
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            res = analizator.StartProgram();
            if (res.IsSuccess)
            {
                AnalizeButton.IsEnabled = true;
                ErrorLabel.Content = "Ошибок нет";
                
            }
            else
            {
                AnalizeButton.IsEnabled = false;
                ErrorLabel.Content = $"Ошибка в: {res.ErrorIndex} \nСообщение: {res.Message}";
                SemanticLabel.Content = string.Empty;
                MoveCaretToPosition(InputField, res.ErrorIndex);
                ShowError(res.ErrorIndex);
            }
        }

        private void ShowError(int errorIndex)
        {
            // Assuming your TextBlock has a fixed font size
            double charWidth = InputField.ActualWidth / InputField.Text.Length;
            errorIndicator.Margin = new Thickness(errorIndex * charWidth, InputField.ActualHeight, 0, 0);
            errorIndicator.Visibility = System.Windows.Visibility.Visible;
        }

        public void MoveCaretToPosition(TextBox textBox, int position)
        {
            if (textBox == null || position < 0 || position > textBox.Text.Length)
            {
                return; // Or throw an ArgumentException, depending on your error handling preference.
            }

            textBox.SelectionStart = position;
            textBox.Focus();
        }

        private void AnalizeButton_Click(object sender, RoutedEventArgs e)
        {
            string idents = string.Empty;
            res!.Identificators.ForEach(s => idents += $"{s} \n");
            string consts = string.Empty;
            res.Constants.ForEach(s => consts += $"{s} \n");
            SemanticLabel.Content = idents + "\n" + consts;
        }
    }
}