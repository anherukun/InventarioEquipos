using SharedCode;
using SharedCode.Classes;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
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
        private Usuario usuario = new Usuario();
        private bool isinsetrionComplete = false;
        private SearchResult user;

        private bool isEditorMode;

        public FormularioUsuario()
        {
            InitializeComponent();
            progressbar.Visibility = Visibility.Hidden;
        }
        public FormularioUsuario(object usuario)
        {
            InitializeComponent();
            progressbar.Visibility = Visibility.Hidden;

            this.usuario = usuario as Usuario;
            this.isEditorMode = true;

            UpdateWithEditorMode();
        }

        public bool IsInsertionComplete() => isinsetrionComplete;

        private void UpdateWithEditorMode()
        {
            txtbox_username.IsEnabled = false;
            txtbox_username.Text = $"{usuario.Username}";
            txtbox_trabajador.Text = usuario.Trabajador.ToUpper();
            txtbox_categria.Text = usuario.Categoria != null ? usuario.Categoria.ToUpper() : "";
            txt_correo.Text = usuario.Correo;
            txt_nombread.Text = usuario.NombreAD.ToUpper();
            chkbox_migracioncorreo.IsChecked = usuario.BuzonMigrado;
            chkbox_migracionperfil.IsChecked = usuario.PerfilMigrado;
            txtbox_contrasena.Password = usuario.Contrasena != null ? ApplicationManager.DecodeFromBase64(usuario.Contrasena) : "SIN DATOS...";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ACEPTAR
            progressbar.Visibility = Visibility.Visible;

            if (isEditorMode)
            {
                usuario.NombreAD = txt_nombread.Text.Trim().ToUpper();
                usuario.Correo = txt_correo.Text.Trim();
                usuario.Trabajador = txtbox_trabajador.Text.Trim() != "" ? txtbox_trabajador.Text.Trim().ToUpper() : "";
                usuario.Categoria = txtbox_categria.Text.Trim() != "" ? txtbox_categria.Text.Trim().ToUpper() : "";
                usuario.BuzonMigrado = chkbox_migracioncorreo.IsChecked.Value;
                usuario.PerfilMigrado = chkbox_migracionperfil.IsChecked.Value;
                usuario.Contrasena = txtbox_contrasena.Password.Trim() != "" ? ApplicationManager.EncodeToBase64(txtbox_contrasena.Password.Trim()) : "";

                if (isinsetrionComplete = new DatabaseManager().InsertData(Usuario.GetUpdateSQL(usuario)))
                {
                    MessageBox.Show("Usuario agregado correctamente");
                    this.Close();
                }
                else
                    MessageBox.Show("No se pudo agregar el usuario");
            }

            else if (user != null)
            {
                usuario = new Usuario()
                {
                    Username = int.Parse(txtbox_username.Text.Trim()),
                    NombreAD = txt_nombread.Text.Trim().ToUpper(),
                    Correo = txt_correo.Text.Trim(),
                    Trabajador = txtbox_trabajador.Text.Trim() != "" ? txtbox_trabajador.Text.Trim().ToUpper() : "",
                    Categoria = txtbox_categria.Text.Trim() != "" ? txtbox_categria.Text.Trim().ToUpper() : "",
                    BuzonMigrado = chkbox_migracioncorreo.IsChecked.Value,
                    PerfilMigrado = chkbox_migracionperfil.IsChecked.Value,
                    Contrasena = txtbox_contrasena.Password.Trim() != "" ? ApplicationManager.EncodeToBase64(txtbox_contrasena.Password.Trim()) : ""
                };

                if (isinsetrionComplete = new DatabaseManager().InsertData(Usuario.GetInsertSQL(usuario)))
                {
                    MessageBox.Show("Usuario agregado correctamente");
                    this.Close();
                }                    
                else
                    MessageBox.Show("No se pudo agregar el usuario");
            }
            else
            {
                MessageBox.Show("Comprueba el usuario antes de guardarlo");
            }

            progressbar.Visibility = Visibility.Hidden;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // COMPROBACION CON DIRECTORIO ACTIVO
            progressbar.Visibility = Visibility.Visible;
            try
            {
                user = await Task.Run(() =>
                {
                    string username = "";
                    Application.Current.Dispatcher.Invoke(new Action(() => { username = txtbox_username.Text.Trim(); }));

                    return ActiveDirectoryManager.SearchInActiveDirectory(username);
                });

                if (user != null)
                {
                    txt_correo.Text = ActiveDirectoryManager.GetUserMail(user).ToLower();
                    txt_nombread.Text = ActiveDirectoryManager.GetUserFullname(user).ToUpper();
                    txtbox_trabajador.Text = ActiveDirectoryManager.GetUserFullname(user).ToUpper();
                }
                else
                {
                    txt_correo.Text = "";
                    txt_nombread.Text = "";
                    txtbox_trabajador.Text = "";

                    MessageBox.Show("No se pudo encontrar este usuario");
                }
            }
            catch (Exception ex)
            {
                ApplicationManager.ExceptionHandler(ex);
            }

            progressbar.Visibility = Visibility.Hidden;
        }
    }
}
