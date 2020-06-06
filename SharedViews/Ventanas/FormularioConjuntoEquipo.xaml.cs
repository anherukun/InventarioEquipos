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
        private ConjuntoEquipos conjunto;

        private List<Dispositivo> procesadores = new List<Dispositivo>();
        private List<Usuario> usuarios = new List<Usuario>();
        private List<string> departamentos = new List<string>();

        private List<Dispositivo> dispositivos = new List<Dispositivo>();
        private List<Dispositivo> dispositivosnuevos = new List<Dispositivo>();
        private List<Dispositivo> dispositivoseliminados = new List<Dispositivo>();

        private bool editorMode;
        private bool insertionComplete;

        public FormularioConjuntoEquipo()
        {
            InitializeComponent();
            UpdateCombos();
        }
        public FormularioConjuntoEquipo(object objeto)
        {
            InitializeComponent();
            
            conjunto = (ConjuntoEquipos)objeto;
            editorMode = true;

            UpdateCombosWithEditorMode();
        }

        public bool IsInsertionComplete() => insertionComplete;

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

            progressbar.Visibility = Visibility.Hidden;
        }

        private void UpdateCombosWithEditorMode()
        {
            progressbar.Visibility = Visibility.Visible;

            if (cmd_departamento.HasItems)
                cmd_departamento.Items.Clear();

            if (cmd_serieprocesador.HasItems)
                cmd_serieprocesador.Items.Clear();

            if (cmd_username.HasItems)
                cmd_username.Items.Clear();

            cmd_serieprocesador.IsEnabled = false;
            cmd_username.Items.Add("(AÑADIR NUEVO)");


            List<Dictionary<string, object>> values1 = new DatabaseManager().FromDatabaseToDictionary("SELECT DISTINCT DEPTO FROM CONJUNTO_EQUIPOS");
            
            if (values1 != null && values1.Count > 0)
                foreach (var item in values1)
                    departamentos.Add((string)item["DEPTO"]);
            
            procesadores.Add(Dispositivo.FromDictionarySingle(new DatabaseManager().FromDatabaseToSingleDictionary($"SELECT * FROM DISPOSITIVOS WHERE DISPOSITIVOS.SERIE LIKE \"{conjunto.Procesador}\"")));
            
            if (departamentos != null && departamentos.Count > 0)
                foreach (var item in departamentos)
                    Application.Current.Dispatcher.Invoke(new Action(() => { cmd_departamento.Items.Add(item); }));
            
            usuarios = Usuario.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary("SELECT * FROM USUARIOS ORDER BY USUARIOS.USERNAME"));
            
            if (usuarios != null && usuarios.Count > 0)
                foreach (var item in usuarios)
                    Application.Current.Dispatcher.Invoke(new Action(() => { cmd_username.Items.Add($"{item.Username}"); }));
            
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                RefreshWithExistentValues();
                progressbar.Visibility = Visibility.Hidden;
            }));
        }

        private void RefreshWithExistentValues()
        {
            txtbox_hostname.Text = conjunto.Hostname;
            txtbox_arquitectura.Text = $"{conjunto.Arquitectura}";
            txtbox_ubicacionfisica.Text = conjunto.UbicacionFisica;

            cmd_serieprocesador.Items.Add(conjunto.Procesador);
            cmd_serieprocesador.SelectedIndex = 0;

            for (int i = 0; i < departamentos.Count; i++)
                if (departamentos[i].ToString() == conjunto.Departamento)
                    cmd_departamento.SelectedIndex = i;

            for (int i = 0; i < usuarios.Count; i++)
                if (usuarios[i].Username == conjunto.Usuario)
                    cmd_username.SelectedIndex = i + 1;

            dispositivos = Dispositivo.FromDictionaryListToList(new DatabaseManager().FromDatabaseToDictionary($"SELECT * FROM DISPOSITIVOS " +
                    $"WHERE DISPOSITIVOS.SERIE " +
                    $"IN (SELECT DISTINCT DISPOSITIVO FROM REL_CONJUNTOE_DISPOSITIVO WHERE REL_CONJUNTOE_DISPOSITIVO.PROCESADOR LIKE \"{conjunto.Procesador}\")"));

            lst_dispositivos.ItemsSource = dispositivos;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // AGREGAR DISPOSITIVO
            SelectorDispositivo input = new SelectorDispositivo();
            input.Owner = this;
            input.ShowDialog();

            if (input.IsSelectedOption())
            {
                if (dispositivos == null)
                    dispositivos = new List<Dispositivo>();

                dispositivos.Add(input.RetriveSelection() as Dispositivo);
                dispositivosnuevos.Add(input.RetriveSelection() as Dispositivo);

                lst_dispositivos.ItemsSource = dispositivos;
                lst_dispositivos.Items.Refresh();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // QUITAR DISPOSITIVO

            // COMPRUEBA SI ES DE LOS DISPOSITIVOS QUE ESTA ALMACENADOS EN LA BASE DE DATOS
            if (!dispositivos.Contains(lst_dispositivos.SelectedItem as Dispositivo))
                Console.WriteLine("Application: This element is not in the database");
            else
            {
                Console.WriteLine("Application: This element is in the database, will be deleted");

                if (dispositivos.Remove(lst_dispositivos.SelectedItem as Dispositivo))
                    dispositivoseliminados.Add(lst_dispositivos.SelectedItem as Dispositivo);
            }

            // COMPRUEBA SI ES DE LOS DISPOSITIVOS RECIEN AGREGADOS
            if (dispositivosnuevos.Contains(lst_dispositivos.SelectedItem as Dispositivo))
            {
                Console.WriteLine("Application: This element is not in the database, will be removed from the new devices list");
                dispositivosnuevos.Remove(lst_dispositivos.SelectedItem as Dispositivo);
            }
            

            lst_dispositivos.Items.Refresh();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            insertionComplete = false;
            // GUARDAR
            progressbar.Visibility = Visibility.Visible;

            // SI ES UN CONJUNTO NUEVO
            if (editorMode != true)
            {
                conjunto = new ConjuntoEquipos()
                {
                    Procesador = procesadores[cmd_serieprocesador.SelectedIndex - 1].Serie,
                    Arquitectura = int.Parse(txtbox_arquitectura.Text.Trim()),
                    Departamento = departamentos[cmd_departamento.SelectedIndex],
                    Hostname = txtbox_hostname.Text.Trim().ToUpper(),
                    UbicacionFisica = txtbox_ubicacionfisica.Text.Trim().ToUpper(),
                    Usuario = cmd_username.SelectedIndex > 0 ? usuarios[cmd_username.SelectedIndex - 1].Username : 0
                };

                await Task.Run(() =>
                {
                    new DatabaseManager().InsertData(ConjuntoEquipos.GetInserSQL(conjunto));
                });
            }
            // SI ES UNA MODIFICACION DE UN CONJUNTO
            else
            {
                conjunto.Arquitectura = int.Parse(txtbox_arquitectura.Text.Trim());
                conjunto.Departamento = departamentos[cmd_departamento.SelectedIndex];
                conjunto.UbicacionFisica = txtbox_ubicacionfisica.Text.Trim().ToUpper();
                conjunto.Hostname = txtbox_hostname.Text.Trim().ToUpper();
                conjunto.Usuario = cmd_username.SelectedIndex > 0 ? usuarios[cmd_username.SelectedIndex - 1].Username : 0;

                await Task.Run(() =>
                {
                    new DatabaseManager().InsertData(ConjuntoEquipos.GetUpdateSQL(conjunto));
                });
            }
            

            if (dispositivosnuevos.Count > 0)
            {
                await Task.Run(() =>
                {
                    // INSERTA EN LA TABLA RELACION_CONJUNTO_DISPISITIVOS
                    foreach (var item in dispositivosnuevos)
                    {
                        new DatabaseManager().InsertData($"INSERT INTO REL_CONJUNTOE_DISPOSITIVO (PROCESADOR, DISPOSITIVO) VALUES (\"{conjunto.Procesador}\", \"{item.Serie}\")");
                    }
                });
            }
            else
                progressbar.Visibility = Visibility.Hidden;

            if (dispositivoseliminados.Count > 0)
            {
                progressbar.Visibility = Visibility.Visible;

                await Task.Run(() =>
                {
                    // ELIMINA EN LA TABLA RELACION_CONJUNTO_DISPISITIVOS
                    foreach (var item in dispositivoseliminados)
                    {
                        new DatabaseManager().InsertData($"DELETE * FROM REL_CONJUNTOE_DISPOSITIVO WHERE REL_CONJUNTOE_DISPOSITIVO.PROCESADOR LIKE \"{conjunto.Procesador}\" " +
                            $"AND REL_CONJUNTOE_DISPOSITIVO.DISPOSITIVO LIKE \"{item.Serie}\"");
                    }
                });

                insertionComplete = true;
            }
            else
            {
                progressbar.Visibility = Visibility.Hidden;
                insertionComplete = true;
            }

            if (insertionComplete)
            {
                this.Close();
            }
        }

        private async void cmd_serieprocesador_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SELECCION PROCESADOR
            if (editorMode == false)
            {
                if (cmd_serieprocesador.SelectedIndex != 0)
                {
                    if (cmd_serieprocesador.SelectedIndex > 0)
                    {
                        txt_tdispositivo.Text = procesadores[cmd_serieprocesador.SelectedIndex - 1].TDispositivo;
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
            else
            {
                if (cmd_serieprocesador.SelectedIndex > -1)
                {
                    txt_tdispositivo.Text = procesadores[cmd_serieprocesador.SelectedIndex].TDispositivo;
                    txt_modelo.Text = procesadores[cmd_serieprocesador.SelectedIndex].Modelo;
                    txt_marca.Text = procesadores[cmd_serieprocesador.SelectedIndex].Marca;
                }
            }
        }

        private async void cmd_username_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SELECCION USUARIO
            if (cmd_username.SelectedIndex == -1)
            {
                txt_usuariodirectorio.Text = "";
                txt_correo.Text = "";

                txt_perfilmigrado.Text = "";
                txt_buzonmigrado.Text = "";
            }
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
