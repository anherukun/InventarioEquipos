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

namespace SharedViews.Ventanas.InputWindows
{
    /// <summary>
    /// Lógica de interacción para SelectorDispositivo.xaml
    /// </summary>
    public partial class SelectorDispositivo : Window
    {
        List<Dispositivo> Dispositivos = new List<Dispositivo>();

        public SelectorDispositivo()
        {
            InitializeComponent();
            UpdateCombo();
        }

        private async void UpdateCombo()
        {
            if (cmd_serie.HasItems)
                cmd_serie.Items.Clear();

            cmd_serie.Items.Add("(AÑADIR NUEVO)");

            Dispositivos = await Task.Run(() =>
            {
                return Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM DISPOSITIVOS WHERE DISPOSITIVOS.DISPOSITIVO NOT LIKE \"PROCESADOR\" AND DISPOSITIVOS.DISPOSITIVO NOT LIKE \"LAPTOP\" ORDER BY DISPOSITIVOS.DISPOSITIVO"));
            });

            foreach (var item in Dispositivos)
                cmd_serie.Items.Add(item.Serie);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmd_serie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // AL SELECCIONAR UN DISPOSITIVO

            if (cmd_serie.SelectedIndex > 0)
            {
                txt_dispositivo.Text = Dispositivos[cmd_serie.SelectedIndex - 1].TDispositivo.Trim().ToUpper();
                txt_marca.Text = Dispositivos[cmd_serie.SelectedIndex - 1].Marca.Trim().ToUpper();
                txt_modelo.Text = Dispositivos[cmd_serie.SelectedIndex - 1].Modelo != null ? Dispositivos[cmd_serie.SelectedIndex - 1].Modelo.Trim().ToUpper() : "SIN DATOS...";
            }
        }
    }
}
