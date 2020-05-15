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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharedViews.FrameViews
{
    /// <summary>
    /// Lógica de interacción para BuscadorDispositivos.xaml
    /// </summary>
    public partial class BuscadorDispositivos : Page
    {
        private List<Dispositivo> dispositivos = new List<Dispositivo>();
        private List<string> tdispositivos = new List<string>();
        private List<string> marcas = new List<string>();

        private ProgressBar progressBar;
        public BuscadorDispositivos(ProgressBar progressBar)
        {
            InitializeComponent();
            this.progressBar = progressBar;
            UpdateLayout();
        }

        private new async void UpdateLayout()
        {
            progressBar.Visibility = Visibility.Visible;

            tdispositivos = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values1 = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT DISPOSITIVO FROM DISPOSITIVOS");
                List<string> result = new List<string>();
                result.Add("(TODOS)");
            
                if (values1 != null && values1.Count > 0)
                    foreach (var item in values1)
                    {
                        result.Add((string)item["DISPOSITIVO"]);
                    }
                return result;
            });

            await Task.Run(() =>
            {
                foreach (var item in tdispositivos)
                    Application.Current.Dispatcher.Invoke(new Action(() => { cmd_dispositivo.Items.Add(item); }));
            });

            cmd_dispositivo.SelectedIndex = 0;

            marcas = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values2 = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT MARCA FROM DISPOSITIVOS");
                List<string> result = new List<string>();
                result.Add("(TODOS)");

                if (values2 != null && values2.Count > 0)
                    foreach (var item in values2)
                    {
                        if (item["MARCA"].GetType() != typeof(System.DBNull))
                            result.Add((string)item["MARCA"]);
                    }
                return result;
            });

            await Task.Run(() =>
            {
                foreach (var item in marcas)
                    Application.Current.Dispatcher.Invoke(new Action(() => { cmd_marca.Items.Add(item); }));
            });

            
            cmd_marca.SelectedIndex = 0;

            IniciarBusqueda(CrearSQL());
        }

        private async void IniciarBusqueda(string sql)
        {
            progressBar.Visibility = Visibility.Visible;

            dispositivos = await Task.Run(() =>
            {
                return Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary(sql));
            });

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    lst_registros.ItemsSource = dispositivos;
                }));
            });

            progressBar.Visibility = Visibility.Hidden;
        }

        private string CrearSQL(int dispositivosIndex = 0, int marcasindex = 0, string serie = "")
        {
            string sql = "SELECT * FROM DISPOSITIVOS WHERE {param-1} AND {param-2} AND {param-3} ORDER BY DISPOSITIVOS.DISPOSITIVO ASC, DISPOSITIVOS.MARCA ASC, DISPOSITIVOS.MODELO ASC";

            if (dispositivosIndex == 0 && marcasindex == 0 && serie.Trim().Length == 0)
                sql = "SELECT * FROM DISPOSITIVOS ORDER BY DISPOSITIVOS.DISPOSITIVO ASC, DISPOSITIVOS.MARCA ASC, DISPOSITIVOS.MODELO ASC";
            else
            {
                if (dispositivosIndex != 0)
                    sql = sql.Replace("{param-1}", $"DISPOSITIVOS.DISPOSITIVO LIKE \"{tdispositivos[dispositivosIndex]}\" ");
                else
                    sql = sql.Replace("{param-1} AND", "").Replace("  ", " ");
                
                if (marcasindex != 0)
                    sql = sql.Replace("{param-2}", $"DISPOSITIVOS.MARCA LIKE \"{marcas[marcasindex]}\"");
                else
                    sql = sql.Replace("{param-2} AND", "").Replace("  ", " ");

                if (serie.Trim().Length != 0)
                    sql = sql.Replace("{param-3}", $"DISPOSITIVOS.SERIE LIKE \'%{serie.Trim().ToUpper()}%\'");
                else
                    sql = sql.Replace("AND {param-3}", "").Replace("  ", " ");
            }


            return sql;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ACTUALIZAR
            string sql = CrearSQL(cmd_dispositivo.SelectedIndex, cmd_marca.SelectedIndex, txtbox_serie.Text.Trim().ToUpper());
            IniciarBusqueda(sql);
        }
    }
}
