using SharedCode;
using SharedCode.Classes;
using SharedCode.Classes.Misc;
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
    /// Lógica de interacción para FormularioConjuntoEquipo.xaml
    /// </summary>
    public partial class FormularioConjuntoEquipo : Window
    {
        private List<Dispositivo> procesadores = new List<Dispositivo>();
        private List<Usuario> usuarios = new List<Usuario>();
        private List<string> departamentos = new List<string>();

        private List<Dispositivo> dispositivos = new List<Dispositivo>();
        private List<Dispositivo> dispositivosnuevos = new List<Dispositivo>();
        private List<Dispositivo> dispositivoseliminados = new List<Dispositivo>();

        public FormularioConjuntoEquipo()
        {
            InitializeComponent();
            UpdateCombos();
        }

        private async void UpdateCombos()
        {
            progressbar.Visibility = Visibility.Visible;

            if (cmd_departamento.HasItems)
                cmd_departamento.Items.Clear();

            if (cmd_serieprocesador.HasItems)
                cmd_serieprocesador.Items.Clear();

            if (cmd_username.HasItems)
                cmd_username.Items.Clear();

            cmd_serieprocesador.Items.Add("(AÑADIR NUEVO)");
            cmd_username.Items.Add("(AÑADIR NUEVO)");

            procesadores = await Task.Run(() =>
            {
                return Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM DISPOSITIVOS WHERE (DISPOSITIVOS.DISPOSITIVO LIKE \"PROCESADOR\" " +
                    "OR DISPOSITIVOS.DISPOSITIVO LIKE \"LAPTOP\") " +
                    "AND DISPOSITIVOS.SERIE NOT IN (SELECT DISTINCT PROCESADOR FROM CONJUNTO_EQUIPOS)"));
            });

            if (procesadores != null && procesadores.Count > 0)
                foreach (var item in procesadores)
                    cmd_serieprocesador.Items.Add($"{item.Serie}");

            departamentos = await Task.Run(() =>
            {
                List<Dictionary<string, object>> values1 = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT DEPTO FROM CONJUNTO_EQUIPOS");
                List<string> result = new List<string>();

                if (values1 != null && values1.Count > 0)
                    foreach (var item in values1)
                    {
                        result.Add((string)item["DEPTO"]);
                    }
                return result;
            });

            if (departamentos != null && departamentos.Count > 0)
                await Task.Run(() =>
                {
                    foreach (var item in departamentos)
                        Application.Current.Dispatcher.Invoke(new Action(() => { cmd_departamento.Items.Add(item); }));
                });

            usuarios = await Task.Run(() =>
            {
                return Usuario.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM USUARIOS ORDER BY USUARIOS.USERNAME"));
            });

            if (usuarios != null && usuarios.Count > 0)
                await Task.Run(() =>
                {
                    foreach (var item in usuarios)
                        Application.Current.Dispatcher.Invoke(new Action(() => { cmd_username.Items.Add($"{item.Username}"); }));
                });

            // await Task.Run(() =>
            // {
            // 
            //     Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary($"SELECT * FROM REL_CONJUNTOE_DISPOSITIVO WHERE REL_CONJUNTOE_DISPOSITIVO.PROCESADOR LIKE \"{dis}\""));
            // });

            progressbar.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // AGREGAR DISPOSITIVO
            SelectorDispositivo input = new SelectorDispositivo();
            input.Owner = this;
            input.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // QUITAR DISPOSITIVO
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // GUARDAR

        }

        private async void cmd_serieprocesador_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SELECCION PROCESADOR
            if (cmd_serieprocesador.SelectedIndex != 0)
            {
                if (cmd_serieprocesador.SelectedIndex > 0)
                {
                    txt_tdispositivo.Text= procesadores[cmd_serieprocesador.SelectedIndex - 1].TDispositivo;
                    txt_modelo.Text = procesadores[cmd_serieprocesador.SelectedIndex - 1].Modelo;
                    txt_marca.Text = procesadores[cmd_serieprocesador.SelectedIndex - 1].Marca;
                }
            }
            else
            {
                FormularioDispositivo form = new FormularioDispositivo(FormularioDispositivo.PROCESADOR);
                form.Owner = this;
                form.ShowDialog();

                cmd_serieprocesador.SelectedIndex = -1;

                if (form.isInsertionComplete())
                {
                    progressbar.Visibility = Visibility.Visible;
                    if (cmd_serieprocesador.HasItems)
                        cmd_serieprocesador.Items.Clear();

                    cmd_serieprocesador.Items.Add("(AÑADIR NUEVO)");

                    procesadores = await Task.Run(() =>
                    {
                        return Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM DISPOSITIVOS WHERE (DISPOSITIVOS.DISPOSITIVO LIKE \"PROCESADOR\" " +
                            "OR DISPOSITIVOS.DISPOSITIVO LIKE \"LAPTOP\") " +
                            "AND DISPOSITIVOS.SERIE NOT IN (SELECT DISTINCT PROCESADOR FROM CONJUNTO_EQUIPOS)"));
                    });

                    if (procesadores != null && procesadores.Count > 0)
                        foreach (var item in procesadores)
                            cmd_serieprocesador.Items.Add($"{item.Serie}");

                    progressbar.Visibility = Visibility.Hidden;
                }
            }
        }

        private async void cmd_username_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SELECCION USUARIO
            if (cmd_username.SelectedIndex != 0)
            {
                if (cmd_username.SelectedIndex > 0)
                {
                    txt_usuariodirectorio.Text = usuarios[cmd_username.SelectedIndex - 1].NombreAD;
                    txt_correo.Text = usuarios[cmd_username.SelectedIndex - 1].Correo;

                    txt_perfilmigrado.Text = usuarios[cmd_username.SelectedIndex - 1].PerfilMigrado ? "SI" : "NO";
                    txt_buzonmigrado.Text = usuarios[cmd_username.SelectedIndex - 1].BuzonMigrado ? "SI" : "NO";
                }
            }
            else
            {
                FormularioUsuario form = new FormularioUsuario();
                form.Owner = this;
                form.ShowDialog();
                if (form.IsInsertionComplete())
                {
                    progressbar.Visibility = Visibility.Visible;
                    if (cmd_username.HasItems)
                        cmd_username.Items.Clear();

                    cmd_username.Items.Add("(AÑADIR NUEVO)");

                    usuarios = await Task.Run(() =>
                    {
                        return Usuario.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM USUARIOS ORDER BY USUARIOS.USERNAME"));
                    });

                    if (usuarios != null && usuarios.Count > 0)
                        await Task.Run(() =>
                        {
                            foreach (var item in usuarios)
                                Application.Current.Dispatcher.Invoke(new Action(() => { cmd_username.Items.Add($"{item.Username}"); }));
                        });

                    progressbar.Visibility = Visibility.Hidden;
                }    
            }
        }
    }
}
