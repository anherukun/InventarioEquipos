using SharedCode;
using SharedCode.Classes;
using SharedCode.Classes.Misc;
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
    /// Lógica de interacción para FormularioConjuntoEquipo.xaml
    /// </summary>
    public partial class FormularioConjuntoEquipo : Window
    {
        private List<Dispositivo> procesadores = new List<Dispositivo>();
        private List<Departamento> departamentos = new List<Departamento>();

        public FormularioConjuntoEquipo()
        {
            InitializeComponent();
            UpdateCombos();
        }

        private new async void UpdateCombos()
        {
            progressbar.Visibility = Visibility.Visible;

            if (cmd_departamento.HasItems)
                cmd_departamento.Items.Clear();

            if (cmd_serieprocesador.HasItems)
                cmd_serieprocesador.Items.Clear();

            if (cmd_username.HasItems)
                cmd_username.Items.Clear();

            cmd_serieprocesador.Items.Add("(AÑADIR NUEVO)");
            cmd_departamento.Items.Add("(AÑADIR NUEVO)");
            cmd_username.Items.Add("(AÑADIR NUEVO)");

            

            procesadores = await Task.Run(() =>
            {
                return Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM DISPOSITIVOS WHERE DISPOSITIVOS.DISPOSITIVO LIKE \"PROCESADOR\" AND DISPOSITIVOS.SERIE NOT IN (SELECT DISTINCT PROCESADOR FROM CONJUNTO_EQUIPOS)"));
            });

            foreach (var item in procesadores)
                cmd_serieprocesador.Items.Add($"{item.Serie}");

            departamentos = await Task.Run(() =>
            {
                return Departamento.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM MISC_DEPARTAMENTOS ORDER BY MISC_DEPARTAMENTOS.NOMBRE ASC"));
            });

            foreach (var item in departamentos)
                cmd_departamento.Items.Add($"{item.Nombre}");

            progressbar.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // AGREGAR DISPOSITIVO
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // QUITAR DISPOSITIVO
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // GUARDAR
        }

        private void cmd_serieprocesador_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SELECCION PROCESADOR
            if (cmd_serieprocesador.SelectedIndex != 0)
            {
                if (cmd_serieprocesador.SelectedIndex > 0)
                {
                    txt_modelo.Text = procesadores[cmd_serieprocesador.SelectedIndex - 1].Modelo;
                    txt_marca.Text = procesadores[cmd_serieprocesador.SelectedIndex - 1].Marca;
                }

            }
            else
            {
                MessageBox.Show("Añadir nuevo");
                cmd_serieprocesador.SelectedIndex = -1;
            }
        }
    }
}
