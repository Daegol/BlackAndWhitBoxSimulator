using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiagramDesigner.Simulator;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public Window1()
        {
            InitializeComponent();
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int result =0;
            int.TryParse(TextBox.Text, out result);
            if(result!=0) TestManeger.Instance.SetNumberOfTests(result);
        }

        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox.Text = "0";
        }
    }
}
