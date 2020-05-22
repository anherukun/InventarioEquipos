using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharedViews.Ventanas.InputWindows
{
    /// <summary>
    /// Lógica de interacción para TextInput.xaml
    /// </summary>
    public partial class TextInput : Window
    {
        private string textInput;

        public TextInput(string Message)
        {
            InitializeComponent();

            txt_mensaje.Text = Message;
        }

        public bool HasValue() => txt_texto.Text.Trim().Length > 0;

        public string RetriveValue() => textInput;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txt_texto.Text.Trim().Length > 0)
            {
                textInput = txt_texto.Text.Trim();
                this.Close();
            }
        }
    }
}
