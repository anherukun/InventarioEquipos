using SharedCode;
using SharedCode.Classes;
using SharedViews.Ventanas;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharedViews.FrameViews
{
    /// <summary>
    /// Lógica de interacción para BuscadorUsuarios.xaml
    /// </summary>
    public partial class BuscadorUsuarios : Page
    {
        private List<Usuario> Usuarios = new List<Usuario>();

        private ProgressBar progressBar;
        public BuscadorUsuarios(ProgressBar progressBar)
        {
            InitializeComponent();
            this.progressBar = progressBar;

            IniciarBusqueda(CrearSQL());
        }

        private string CrearSQL(string value = "")
        {
            if (value.Trim() != "")
                return $"SELECT * FROM USUARIOS WHERE USUARIOS.USERNAME LIKE {value.Trim().ToUpper()}";

            return $"SELECT * FROM USUARIOS";
        }

        private async void IniciarBusqueda(string sql)
        {
            progressBar.Visibility = Visibility.Visible;

            Usuarios = await Task.Run(() =>
            {
                return Usuario.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary(sql));
            });

            lst_registros.ItemsSource = Usuarios;

            progressBar.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // BUSCAR
            IniciarBusqueda(CrearSQL(txtbox_buscar.Text.Trim()));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // VER DETALLES
            VisorUsuario visor = new VisorUsuario(Usuarios[lst_registros.SelectedIndex]);
            visor.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            // EDITAR
            FormularioUsuario form = new FormularioUsuario(Usuarios[lst_registros.SelectedIndex]);
            form.ShowDialog();

            if (form.IsInsertionComplete())
            {
                txtbox_buscar.Clear();
                IniciarBusqueda(CrearSQL());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // NUEVO USUARIO
            FormularioUsuario form = new FormularioUsuario();
            form.ShowDialog();

            if (form.IsInsertionComplete())
            {
                txtbox_buscar.Clear();
                IniciarBusqueda(CrearSQL());
            }
        }
    }
}
