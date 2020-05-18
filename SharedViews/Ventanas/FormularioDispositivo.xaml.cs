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
    /// Lógica de interacción para FormularioDispositivo.xaml
    /// </summary>
    public partial class FormularioDispositivo : Window
    {
        private List<string> tdispostivos = new List<string>();
        private List<string> marcas = new List<string>();
        private List<string> modelos = new List<string>();

        private Dispositivo dispositivo;

        private bool isRedition = false;

        public FormularioDispositivo(object dispositivoOriginal = null)
        {
            InitializeComponent();

            if (dispositivoOriginal != null)
            {
                isRedition = true;
                dispositivo = dispositivoOriginal as Dispositivo;
            }

            UpdateLayout();
        }

        private new async void UpdateLayout()
        {
            progressbar.Visibility = Visibility.Visible;

            if (isRedition)
            {
                txtbox_serie.IsEnabled = false;
            }

            tdispostivos = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT DISPOSITIVO FROM DISPOSITIVOS");
                List<string> result = new List<string>();

                if (values != null && values.Count > 0)
                    foreach (var item in values)
                    {
                        result.Add((string)item["DISPOSITIVO"]);
                    }
                return result;
            });

            foreach (var item in tdispostivos)
                cmd_tdispositivo.Items.Add(item.Trim().ToUpper());

            marcas = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT MARCA FROM DISPOSITIVOS");
                List<string> result = new List<string>();

                if (values != null && values.Count > 0)
                    foreach (var item in values)
                    {
                        result.Add((string)item["MARCA"]);
                    }
                return result;
            });

            foreach (var item in marcas)
                cmd_marca.Items.Add(item.Trim().ToUpper());

            modelos = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT MODELO FROM DISPOSITIVOS");
                List<string> result = new List<string>();

                if (values != null && values.Count > 0)
                    foreach (var item in values)
                    {
                        if (item["MODELO"].GetType() != typeof(System.DBNull))
                            result.Add((string)item["MODELO"]);
                    }
                return result;
            });

            foreach (var item in modelos)
                cmd_modelo.Items.Add(item.Trim().ToUpper());

            progressbar.Visibility = Visibility.Hidden;
        }

        private bool FormIsComplete()
        {
            return txtbox_serie.Text.Trim().Length > 0 && cmd_marca.Text.Trim().Length > 0 && cmd_tdispositivo.Text.Trim().Length > 0;
        }

        private Dispositivo BuildDevice(string tdispositivo, string marca, string serie, string inventario = "", string modelo = "")
        {
            return new Dispositivo()
            {
                Serie = serie.Trim().ToUpper(),
                Inventario = inventario.Trim().ToUpper(),
                Marca = marca.Trim().ToUpper(),
                Modelo = modelo.Trim().ToUpper(),
                TDispositivo = tdispositivo.Trim().ToUpper()
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FormIsComplete())
            {
                string tdispositivo = "", marca = "", modelo = "";

                if (cmd_tdispositivo.SelectedIndex > -1)
                    tdispositivo = marcas[cmd_tdispositivo.SelectedIndex];
                else
                    tdispositivo = cmd_tdispositivo.Text;

                if (cmd_marca.SelectedIndex > -1)
                    marca = marcas[cmd_marca.SelectedIndex];
                else
                    marca = cmd_marca.Text;

                dispositivo = BuildDevice(tdispositivo, marca, txtbox_serie.Text.Trim(), txtbox_inventario.Text.Trim(), modelo);
            }
            else
                MessageBox.Show("Debe completar los campos obligatorios");
        }
    }
}
