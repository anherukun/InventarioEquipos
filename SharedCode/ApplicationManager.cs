using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace SharedCode
{
    class ApplicationManager
    {
        /// <summary>Ejecuta el recolector de basura para liberar memoria que ya no se usa</summary>
        static public void InitGB()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForFullGCComplete();
        }

        /// <summary>Verifica de que exista el archivo de configuracion en la ruta preestablecida: <c>C:\Users\%USERNAME%\AppData\Local\ControlAcceso</c></summary>
        /// <param name="filename">Nombre del archivo con extension</param>
        /// <example><code>
        /// if (ApplicationManager.FileExistOnAppdata("Settings.data"))
        /// {
        ///     /// Do something...
        /// }
        /// ...</code></example>
        static public bool FileExistOnAppdata(string filename) => File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/Inventarios/{filename}") ? true : false;
        // Es lo mismo que escribir esto:
        // { 
        //     if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/ControlAcceso/{filename}"))
        //     {
        //         return true;
        //     }
        //     return false;
        // }

        /// <summary>Escribe un arreglo de bytes en la ruta preestablecida: <c>C:\Users\%USERNAME%\AppData\Local\ControlAcceso</c></summary>
        /// <param name="bytes">Objeto serializado en un arreglo de bytes</param>
        /// <param name="filename">Nombre del archivo con extension</param>
        /// <returns><see cref="true"/> Si el archivo pudo escribirse correctamente. <see cref="false"/> Si ocurrio alguna excepcion, mostrara un mensaje en pantalla con el error</returns>
        static public bool WriteBinaryFileOnAppdata(byte[] bytes, string filename)
        {
            // Se comprueba de que exista la rita donde se escribira el archivo, cuando no la encuentre, se encargara de crearla
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios\\{filename}"))
            {
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios");
            }

            try
            {
                // Haciendo el uso de FileStream creara el archivo con el arreglo de bytes en la ruta de LocalApplicationData
                // Y cuando concluya el proceso ratornara un True
                using (FileStream stream = File.OpenWrite($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios\\{filename}"))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    return true;
                }
                //MessageBox.Show($"Configuraciones Guardadas");
            }
            catch (Exception ex)
            {
                // Cuando ocurra algo inesperado, mandara el error en pantalla y retornara un False
                ApplicationManager.ExceptionHandler(ex);
                return false;
            }
        }

        /// <summary>Lee un los bytes de un archivo binario ubicado en la ruta preestablecida: <c>C:\Users\%USERNAME%\AppData\Local\ControlAcceso</c></summary>
        /// <param name="filename">Nombre del archivo con extension</param>
        /// <returns><see cref="object"/> Serializado en un arreglo de bytes</returns>
        static public byte[] ReadBinaryFileOnAppdata(string filename)
        {
            try
            {
                // Retorna un arreglo de bytes leidos del archivo
                return File.ReadAllBytes($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios\\{filename}");
            }
            catch (Exception ex)
            {
                // Cuando ocurra algo inesperado, mandara el error en pantalla y retornara un False
                ApplicationManager.ExceptionHandler(ex);
                return null;
            }
        }

        static public void ExceptionHandler(Exception e)
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                // Se comprueba de que exista la rita donde se escribira el archivo, cuando no la encuentre, se encargara de crearla
                if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios\\Error-Log\\{dateTime.Ticks}-Error.txt"))
                {
                    Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios\\Error-Log");
                }


                MessageBox.Show($"Se ha detectado un error durante la ejecicion del programa, si persiste contacte a su departamento de TI:\n{e.Message}");

                string txt = $"Mensaje:\t{e.Message}\n" +
                    $"Source: \t{e.Source}\n" +
                    $"StackTrace:\n{e.StackTrace}\n" +
                    $"TargetSite:\t{e.TargetSite}" +
                    $"HelpLink:\t{e.HelpLink}\n" +
                    $"HResult:\t{e.HResult}\n";

                if (e.Data.Count > 0)
                    foreach (var key in e.Data)
                    {
                        txt += $"{key.ToString()}\t";
                    }

                File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Inventarios\\Error-Log\\{dateTime.Ticks}-Error.txt", txt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se detectado un error en la ejecucion y se intento registrar el evento, contacte a su departamto de TI\nRaiz: {e.Message} \nCatcher:{ex.Message}");
                throw;
            }
        }

        /// <summary>Lee el archivo Sources.data ubicado en la carpeta del ejecutable de la aplicacion</summary>
        /// <returns>Diccionario con los los valores extraidos del archivo sources.data</returns>
        public static Dictionary<string, string> RetriveFromSourcesFile()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            // MessageBox.Show($@"{Environment.CurrentDirectory}\db.data");
            try
            {
                // Lee el archivo y asigna cada linea de texto en un espacio de un arreglo
                // Cada linea es separada por el caracter (,) el primer valor obtenido es el nombre de la clave para el Dictionary y el 
                // segundo el valor que le corresponde a esa clave
                string[] lines = System.IO.File.ReadAllLines($"{Environment.CurrentDirectory}/sources.data");
                foreach (string s in lines)
                {
                    result.Add(s.Split(',')[0].Trim(), s.Split(',')[1].Trim());
                }
                return result;
            }
            catch (Exception ex)
            {
                ApplicationManager.ExceptionHandler(ex);
                return null;
            }
        }
    }
}

