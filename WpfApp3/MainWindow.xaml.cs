using RPNcal;
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

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            //создание токин листа
            List<Token> TokenList = RPN.TokenListMake(tBoxInput.Text, txBoxVariable.Text);
            //переписываем токин лист в rpn
            List<Token> TokenListRPN = RPN.RewriteToRPN(TokenList);
            //считаем
            Number result = RPN.CalculateRPN(TokenListRPN);
            tBoxOutput.Text = (result.value).ToString();
        }
    }
}