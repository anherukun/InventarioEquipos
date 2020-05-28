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
    /// Lógica de interacción para FormularioUsuario.xaml
    /// </summary>
    public partial class FormularioUsuario : Window
    {
        private Usuario usuario = new Usuario();
        private bool isinsetrionComplete = false;
        public FormularioUsuario()
        {
            InitializeComponent();
        }

        public bool IsInsertionComplete() => isinsetrionComplete;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ACEPTAR
            if (txt_correo.Text != "Sin resultados" || txt_nombread.Text != "Sin resultados")
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
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // COMPROBACION CON DIRECTORIO ACTIVO
            try
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        txt_correo.Text = ActiveDirectoryManager.GetUserMail(ActiveDirectoryManager.SearchInActiveDirectory(txtbox_username.Text.Trim())).ToLower();
                    }));
                });

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        txt_nombread.Text = ActiveDirectoryManager.GetUserFullname(ActiveDirectoryManager.SearchInActiveDirectory(txtbox_username.Text.Trim())).ToUpper();
                    }));
                });

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        txtbox_trabajador.Text = ActiveDirectoryManager.GetUserFullname(ActiveDirectoryManager.SearchInActiveDirectory(txtbox_username.Text.Trim())).ToUpper();
                    }));
                });
            }
            catch (Exception ex)
            {
                ApplicationManager.ExceptionHandler(ex);
            }
            
        }
    }
}
