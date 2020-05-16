using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows.Input;

namespace SharedCode.Classes
{
    class ConjuntoEquipos
    {
        public string Procesador { get; set; }
        public string Hostname { get; set; }
        public string Departamento { get; set; } = "";
        public string UbicacionFisica { get; set; } = "";
        public int Usuario { get; set; }
        public int Arquitectura { get; set; }

        public static ConjuntoEquipos FromDictionarySingle(Dictionary<string, object> keyValues)
        {
            if (keyValues != null && keyValues.Count > 0)
            {
                ConjuntoEquipos c = new ConjuntoEquipos();

                c.Procesador = (string)keyValues["PROCESADOR"];
                c.Hostname = (string)keyValues["HOSTNAME"];
                c.Departamento = (string)keyValues["DEPTO"];                

                if (keyValues["UBICACION_FISICA"].GetType() != typeof(System.DBNull))
                    c.UbicacionFisica = (string)keyValues["UBICACION_FISICA"];
                
                if (keyValues["USUARIO"].GetType() != typeof(System.DBNull))
                    c.Usuario = (int)keyValues["USUARIO"];

                if (keyValues["ARQUITECURA"].GetType() != typeof(System.DBNull))
                    c.Arquitectura = (int)keyValues["ARQUITECURA"];

                return c;
            }

            return null;
        }

        /// <summary>Costruye una lista de elementos con los datos obtenidos de un diccionario</summary>
        /// <param name="data">Lista de diccionarios con los datos de la Base de Datos</param>
        /// <returns>Lista de ConjuntoEquipos</returns>
        public static List<ConjuntoEquipos> FromDictionaryListToList(List<Dictionary<string, object>> keyValues)
        {
            List<ConjuntoEquipos> ls = new List<ConjuntoEquipos>();

            if (keyValues != null && keyValues.Count > 0)
                foreach (var item in keyValues)
                {
                    ConjuntoEquipos c = new ConjuntoEquipos();

                    c.Procesador = (string)item["PROCESADOR"];
                    c.Hostname = (string)item["HOSTNAME"];
                    c.Departamento = (string)item["DEPTO"];

                    if (item["UBICACION_FISICA"].GetType() != typeof(System.DBNull))
                        c.UbicacionFisica = (string)item["UBICACION_FISICA"];

                    if (item["USUARIO"].GetType() != typeof(System.DBNull))
                        c.Usuario = (int)item["USUARIO"];

                    if (item["ARQUITECTURA"].GetType() != typeof(System.DBNull))
                        c.Arquitectura = (int)item["ARQUITECTURA"];

                    ls.Add(c);
                }

            ApplicationManager.InitGB();
            return ls;
        }
    }
}
