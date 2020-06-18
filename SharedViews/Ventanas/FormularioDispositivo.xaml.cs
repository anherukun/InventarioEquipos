using SharedCode;
using SharedCode.Classes;
using SharedViews.Ventanas.InputWindows;
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
        public const int ALLDEVICES = 0;
        public const int PROCESADOR = 1;

        private List<string> tdispostivos = new List<string>();
        private List<string> marcas = new List<string>();
        private List<string> modelos = new List<string>();

        private Dispositivo dispositivo;
        private int defaultDeviceSelection = 0;
        private bool isRedition = false;
        private bool isinsertionComplete = false;

        public FormularioDispositivo(int selectionMode = 0)
        {
            InitializeComponent();

            defaultDeviceSelection = selectionMode;

            UpdateCombos();
        }
        public FormularioDispositivo(object dispositivoOriginal = null, int selectionMode = 0)
        {
            InitializeComponent();

            defaultDeviceSelection = selectionMode;

            if (dispositivoOriginal != null)
            {
                isRedition = true;
                btn_autogenerar.IsEnabled = false;
                dispositivo = dispositivoOriginal as Dispositivo;
            }

            UpdateCombos();
        }

        private new void UpdateLayout()
        {
            if (isRedition)
            {
                txtbox_serie.IsEnabled = false;
                txtbox_serie.Text = dispositivo.Serie.Trim().ToUpper();
                txtbox_inventario.Text = dispositivo.Inventario != null ? dispositivo.Inventario.Trim().ToUpper() : "";

                for (int i = 0; i < marcas.Count; i++)
                    if (marcas[i] == dispositivo.Marca)
                        cmd_marca.SelectedIndex = i + 1;

                for (int i = 0; i < tdispostivos.Count; i++)
                    if (tdispostivos[i] == dispositivo.TDispositivo)
                        cmd_tdispositivo.SelectedIndex = i + 1;

                for (int i = 0; i < modelos.Count; i++)
                    if (modelos[i] == dispositivo.Modelo)
                        cmd_modelo.SelectedIndex = i + 1;
            }
        }

        private async void UpdateCombos()
        {
            progressbar.Visibility = Visibility.Visible;
            string sqldispositivos = "", sqlmarcas = "", sqlmodelos = "";

            switch (defaultDeviceSelection)
            {
                case PROCESADOR:
                    sqldispositivos = "SELECT DISTINCT DISPOSITIVO FROM DISPOSITIVOS WHERE DISPOSITIVOS.DISPOSITIVO LIKE \"PROCESADOR\" OR DISPOSITIVOS.DISPOSITIVO LIKE \"LAPTOP\"";
                    sqlmarcas = "SELECT DISTINCT MARCA FROM DISPOSITIVOS WHERE DISPOSITIVOS.DISPOSITIVO LIKE \"PROCESADOR\" OR DISPOSITIVOS.DISPOSITIVO LIKE \"LAPTOP\"";
                    sqlmodelos = "SELECT DISTINCT MODELO FROM DISPOSITIVOS WHERE DISPOSITIVOS.DISPOSITIVO LIKE \"PROCESADOR\" OR DISPOSITIVOS.DISPOSITIVO LIKE \"LAPTOP\"";
                    break;

                case ALLDEVICES:
                    sqldispositivos = "SELECT DISTINCT DISPOSITIVO FROM DISPOSITIVOS";
                    sqlmarcas = "SELECT DISTINCT MARCA FROM DISPOSITIVOS";
                    sqlmodelos = "SELECT DISTINCT MODELO FROM DISPOSITIVOS";
                    break;
            }

            cmd_tdispositivo.Items.Add("(AÑADIR NUEVO)");
            cmd_modelo.Items.Add("(AÑADIR NUEVO)");
            cmd_marca.Items.Add("(AÑADIR NUEVO)");

            tdispostivos = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values = new DatabaseManager().FromDatabaseToDictionary(sqldispositivos);
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
                List<Dictionary<string, object>> values = new DatabaseManager().FromDatabaseToDictionary(sqlmarcas);
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
                List<Dictionary<string, object>> values = new DatabaseManager().FromDatabaseToDictionary(sqlmodelos);
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

            if (tdispostivos.Count > 0 && marcas.Count > 0 && modelos.Count > 0)
                UpdateLayout();

            progressbar.Visibility = Visibility.Hidden;
        }

        private bool FormIsComplete()
        {
            // CHECA SI LOS CAMPOS OBLIGATORIOS YA FUERON CAPTURADOS
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

        public bool isInsertionComplete() => isinsertionComplete;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // GUARDAR
            if (FormIsComplete())
            {
                string tdispositivo = "", marca = "", modelo = "";
                if (!isRedition)
                {
                    if (cmd_tdispositivo.SelectedIndex > -1)
                        tdispositivo = tdispostivos[cmd_tdispositivo.SelectedIndex - 1];

                    if (cmd_marca.SelectedIndex > -1)
                        marca = marcas[cmd_marca.SelectedIndex - 1];

                    if (cmd_modelo.SelectedIndex > -1)
                        modelo = modelos[cmd_modelo.SelectedIndex - 1];

                    dispositivo = BuildDevice(tdispositivo, marca, txtbox_serie.Text.Trim(), txtbox_inventario.Text.Trim(), modelo);

                    if (isinsertionComplete = new DatabaseManager().InsertData(Dispositivo.GetInsertSQL(dispositivo)))
                        this.Close();
                }
                else
                {
                    if (cmd_tdispositivo.SelectedIndex > -1)
                        tdispositivo = tdispostivos[cmd_tdispositivo.SelectedIndex - 1];

                    if (cmd_marca.SelectedIndex > -1)
                        marca = marcas[cmd_marca.SelectedIndex - 1];

                    if (cmd_modelo.SelectedIndex > -1)
                        modelo = modelos[cmd_modelo.SelectedIndex - 1];

                    dispositivo.Modelo = modelo;
                    dispositivo.Marca = marca;
                    dispositivo.TDispositivo = tdispositivo;
                    dispositivo.Inventario = txtbox_inventario.Text.Trim().ToUpper();

                    if (isinsertionComplete = new DatabaseManager().InsertData(Dispositivo.GetUpdateSQL(dispositivo)))
                        this.Close();
                }
            }
            else
                MessageBox.Show("Debe completar los campos obligatorios");
        }

        private void cmd_tdispositivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmd_tdispositivo.SelectedIndex == 0)
            {
                cmd_tdispositivo.SelectedIndex = -1;

                TextInput input = new TextInput("Escribe el numero de modelo del dispositivo");
                input.ShowDialog();

                if (input.HasValue())
                {
                    tdispostivos.Add(input.RetriveValue().ToUpper());

                    cmd_tdispositivo.Items.Clear();
                    cmd_tdispositivo.Items.Add("(AÑADIR NUEVO)");

                    foreach (var item in tdispostivos)
                        cmd_tdispositivo.Items.Add(item);

                    cmd_tdispositivo.SelectedIndex = cmd_tdispositivo.Items.Count - 1;
                }
            }
        }

        private void cmd_marca_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmd_marca.SelectedIndex == 0)
            {
                cmd_marca.SelectedIndex = -1;

                TextInput input = new TextInput("Escribe el numero de modelo del dispositivo");
                input.ShowDialog();

                if (input.HasValue())
                {
                    marcas.Add(input.RetriveValue().ToUpper());

                    cmd_marca.Items.Clear();
                    cmd_marca.Items.Add("(AÑADIR NUEVO)");

                    foreach (var item in marcas)
                        cmd_marca.Items.Add(item);

                    cmd_marca.SelectedIndex = cmd_marca.Items.Count - 1;
                }
            }
        }

        private void cmd_modelo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmd_modelo.SelectedIndex == 0)
            {
                cmd_modelo.SelectedIndex = -1;

                TextInput input = new TextInput("Escribe el numero de modelo del dispositivo");
                input.ShowDialog();

                if (input.HasValue())
                {
                    modelos.Add(input.RetriveValue().ToUpper());

                    cmd_modelo.Items.Clear();
                    cmd_modelo.Items.Add("(AÑADIR NUEVO)");

                    foreach (var item in modelos)
                        cmd_modelo.Items.Add(item);

                    cmd_modelo.SelectedIndex = cmd_modelo.Items.Count - 1;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtbox_serie.Text = ApplicationManager.GenerateGUID();
        }
    }
}
