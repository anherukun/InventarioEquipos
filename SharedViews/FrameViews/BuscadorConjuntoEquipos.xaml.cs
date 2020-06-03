using SharedCode;
using SharedCode.Classes;
using SharedCode.Classes.Misc;
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
    /// Lógica de interacción para BuscadorConjuntoEquipos.xaml
    /// </summary>
    public partial class BuscadorConjuntoEquipos : Page
    {
        class BindingRegister
        {
            public string Departamento { get; set; }
            public string UbicacionFisica { get; set; }
            public string Serie { get; set; }
            public string Hostname { get; set; }
        }
        private List<ConjuntoEquipos> equipos = new List<ConjuntoEquipos>();
        private List<string> departamentos = new List<string>();

        private ProgressBar progressBar;

        public BuscadorConjuntoEquipos(ProgressBar progressBar)
        {
            InitializeComponent();
            this.progressBar = progressBar;
            
            UpdateLayout();
        }

        private new async void UpdateLayout()
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;

            departamentos = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values1 = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT DEPTO FROM CONJUNTO_EQUIPOS");
                List<string> result = new List<string>();
                result.Add("(TODOS)");

                if (values1 != null && values1.Count > 0)
                    foreach (var item in values1)
                    {
                        result.Add((string)item["DEPTO"]);
                    }
                return result;
            });

            await Task.Run(() =>
            {
                foreach (var item in departamentos)
                    Application.Current.Dispatcher.Invoke(new Action(() => { cmd_departamento.Items.Add(item); }));
            });

            cmd_departamento.SelectedIndex = 0;

            IniciarBusqueda(CrearSQL());
        }

        private async void IniciarBusqueda(string sql)
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = false;            

            equipos = await Task.Run(() =>
            {
                return ConjuntoEquipos.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary(sql));
            });

            progressBar.Maximum = equipos.Count;
            progressBar.Value = 0;

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    lst_registros.ItemsSource = equipos;
                }));
            });

            progressBar.Visibility = Visibility.Hidden;
            progressBar.Value = 0;
        }

        private string CrearSQL(int departamentosIndex = 0,  string serie = "", string hostname = "")
        {
            string sql = "SELECT * FROM CONJUNTO_EQUIPOS WHERE {param-1} AND {param-2} AND {param-3} ORDER BY CONJUNTO_EQUIPOS.DEPTO ASC, CONJUNTO_EQUIPOS.UBICACION_FISICA ASC";

            if (departamentosIndex == 0 && hostname.Trim().Length == 0 && serie.Trim().Length == 0)
                sql = "SELECT * FROM CONJUNTO_EQUIPOS ORDER BY CONJUNTO_EQUIPOS.DEPTO ASC, CONJUNTO_EQUIPOS.UBICACION_FISICA ASC";
            else
            {
                if (departamentosIndex != 0)
                    sql = sql.Replace("{param-1}", $"CONJUNTO_EQUIPOS.DEPTO LIKE \"{departamentos[departamentosIndex]}\" ");
                else
                    sql = sql.Replace("{param-1} AND", "").Replace("  ", " ");

                if (hostname.Trim().Length != 0)
                    sql = sql.Replace("{param-2}", $"CONJUNTO_EQUIPOS.HOSTNAME LIKE \'%{hostname.Trim().ToUpper()}%\'");
                else
                    sql = sql.Replace("{param-2} AND", "").Replace("  ", " ");

                if (serie.Trim().Length != 0)
                    sql = sql.Replace("{param-3}", $"CONJUNTO_EQUIPOS.PROCESADOR LIKE \'%{serie.Trim().ToUpper()}%\'");
                else
                    sql = sql.Replace("AND {param-3}", "").Replace("  ", " ");
            }


            return sql;
        }

        private void OpenDetails(ConjuntoEquipos equipo)
        {
            VisorConjuntoEquipo visor = new VisorConjuntoEquipo(equipo);
            visor.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ACTUALIZAR
            string sql = CrearSQL(cmd_departamento.SelectedIndex, txtbox_serie.Text.Trim().ToUpper(), txt_hostname.Text.Trim().ToUpper());
            IniciarBusqueda(sql);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // ABRIR DETALLES
            OpenDetails(equipos[lst_registros.SelectedIndex]);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            // EDITAR CONJUNTO
            FormularioConjuntoEquipo form = new FormularioConjuntoEquipo(equipos[lst_registros.SelectedIndex]);
            form.Show();

            UpdateLayout();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // ABRIR FORMULARIOCONJUNTOEQUIPO
            FormularioConjuntoEquipo form = new FormularioConjuntoEquipo();
            form.Show();

            UpdateLayout();
        }
    }
}
