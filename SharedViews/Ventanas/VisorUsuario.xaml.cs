using SharedCode;
using SharedCode.Classes;
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
    /// Lógica de interacción para VisorUsuario.xaml
    /// </summary>
    public partial class VisorUsuario : Window
    {
        private Usuario Usuario;
        public VisorUsuario(object usuario)
        {
            InitializeComponent();
            this.Usuario = usuario as Usuario;

            UpdateLayout();
        }

        private new void UpdateLayout()
        {
            progressbar.Visibility = Visibility.Collapsed;

            txt_username.Text = $"{Usuario.Username}";
            txt_nombread.Text = $"{Usuario.NombreAD}";
            txt_trabajador.Text = $"{Usuario.Trabajador}";
            txt_categoria.Text = $"{Usuario.Categoria}";
            txt_correo.Text = $"{Usuario.Correo}";
            txt_perfilmigrado.Text = Usuario.PerfilMigrado ? "SI" : "NO";
            txt_buzonmigrado.Text = Usuario.BuzonMigrado ? "SI" : "NO";
            txt_contrasena.Text = Usuario.Contrasena;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // REVELAR CONTRASEÑA
            txt_contrasena.Text = Usuario.Contrasena != null ? ApplicationManager.DecodeFromBase64(Usuario.Contrasena) : "SIN DATOS...";
        }
    }
}
