using SharedCode;
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
using System.Windows.Threading;

namespace InventarioEquipos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int secCounter = 60;
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerExecution;
            timer.Start();

            UpdateMenuInformation();
        }

        private async void UpdateMenuInformation()
        {
            progressbar.Visibility = Visibility.Visible;
            control_1.Title = "Custodias";
            control_2.Title = "Equipos";
            control_3.Title = "Dispositivos";
            control_4.Title = "Usuarios";

            string sub1 = "", sub2 = "", sub3 = "", sub4 = "";
            sub1 = await Task.Run(() =>
            {
                return $"Custodias creadas: {new DatabaseManager().FromDatabaseToSingleDictionary("SELECT COUNT (*) AS TOTAL FROM ASIGNACIONES")["TOTAL"]}";
            });

            sub2 = await Task.Run(() =>
            {
                return $"Equipos registrados: {new DatabaseManager().FromDatabaseToSingleDictionary("SELECT COUNT(*) AS TOTAL FROM CONJUNTO_EQUIPOS")["TOTAL"]}";
            });

            sub3 = await Task.Run(() =>
            {
                return $"Dispositivos registrados: {new DatabaseManager().FromDatabaseToSingleDictionary("SELECT COUNT(*) AS TOTAL FROM DISPOSITIVOS")["TOTAL"]}";
            });

            sub4 = await Task.Run(() =>
            {
                return $"Usuarios registrados: {new DatabaseManager().FromDatabaseToSingleDictionary("SELECT COUNT(*) AS TOTAL FROM USUARIOS")["TOTAL"]}";
            });

            control_1.Subtitle = sub1;
            control_2.Subtitle = sub2;
            control_3.Subtitle = sub3;
            control_4.Subtitle = sub4;

            progressbar.Visibility = Visibility.Hidden;
        }

        private void TimerExecution(object sender, EventArgs e)
        {
            if (secCounter == 0)
            {
                UpdateMenuInformation();
                secCounter = 60;
            }
            else
                secCounter--;

            txt_refreshlog.Text = $"La informacion se actualizara en: {secCounter} segundo(s)";
        }
    }
}
