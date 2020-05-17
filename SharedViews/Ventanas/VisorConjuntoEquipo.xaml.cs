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
    /// Lógica de interacción para VisorConjuntoEquipo.xaml
    /// </summary>
    public partial class VisorConjuntoEquipo : Window
    {
        private ConjuntoEquipos ConjuntoEquipo;
        private Dispositivo Procesador;
        private Usuario Usuario;
        public VisorConjuntoEquipo(Object equipo)
        {
            InitializeComponent();
            ConjuntoEquipo = (ConjuntoEquipos)equipo;

            RetriveAllData();
        }

        private async void RetriveAllData()
        {
            progressbar.Visibility = Visibility.Visible;
            Procesador = await Task.Run(() =>
            {
                return Dispositivo.FromDictionarySingle(new DatabaseManager().FromDatabaseToSingleDictionary($"SELECT * FROM DISPOSITIVOS WHERE DISPOSITIVOS.SERIE LIKE \"{ConjuntoEquipo.Procesador}\""));
            });

            Usuario = await Task.Run(() =>
            {
                return Usuario.FromDictionarySingle(new DatabaseManager().FromDatabaseToSingleDictionary($"SELECT * FROM USUARIOS WHERE USUARIOS.USERNAME LIKE {ConjuntoEquipo.Usuario}"));
            });
            UpdateLayout();
        }

        private new void UpdateLayout()
        {
            txt_hostname.Text = ConjuntoEquipo.Hostname.ToUpper();
            txt_departamento.Text = ConjuntoEquipo.Departamento.ToUpper();
            txt_ubicacionfisica.Text = ConjuntoEquipo.UbicacionFisica.ToUpper();
            
            txt_serieProcesador.Text = Procesador.Serie.ToUpper();
            txt_marca.Text = Procesador.Marca.ToUpper();
            txt_modelo.Text = Procesador.Modelo.ToUpper();
            txt_arquitectura.Text = ConjuntoEquipo.Arquitectura == 0 ? "SIN DATOS..." : $"{ConjuntoEquipo.Arquitectura} BITS";

            if (Usuario != null)
            {
                txt_username.Text = $"PEMEX\\{Usuario.Username}";
                txt_usuariodirectorio.Text = Usuario.NombreAD.ToUpper();
                txt_correo.Text = Usuario.Correo;
                txt_perfilmigrado.Text = Usuario.PerfilMigrado ? "SI" : "NO";
                txt_buzonmigrado.Text = Usuario.BuzonMigrado ? "SI" : "NO";
            }

            progressbar.Visibility = Visibility.Hidden;
        }
    }
}
