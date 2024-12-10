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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            analizator.ChangeInput(InputField.Text);
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            var res = analizator.StartProgram();
            if (res.IsSuccess)
            {
                ErrorLabel.Content = "Ошибок нет";
                string idents = string.Empty;
                res.Identificators.ForEach(s => idents += $"{s} \n");
                string consts = string.Empty;
                res.Constants.ForEach(s => consts += $"{s} \n");
                SemanticLabel.Content = idents + "\n" + consts;
            }
            else
            {
                ErrorLabel.Content = res.Message;
            }
        }
    }
}