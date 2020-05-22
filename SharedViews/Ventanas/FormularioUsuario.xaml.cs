using SharedCode;
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

namespace SharedViews.Ventanas
{
    /// <summary>
    /// Lógica de interacción para FormularioUsuario.xaml
    /// </summary>
    public partial class FormularioUsuario : Window
    {
        public FormularioUsuario()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txt_nombread.Text = ActiveDirectoryManager.GetADUserFullname(txtbox_username.Text.Trim());
            txt_correo.Text = ActiveDirectoryManager.GetDomainMail(txtbox_username.Text.Trim());
        }
    }
}
