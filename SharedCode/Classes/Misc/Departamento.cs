using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Classes.Misc
{
    class Departamento
    {
        public int Clave { get; set; }
        public string Nombre { get; set; }

        public static Departamento FromDictionarySingle(Dictionary<string, object> keyValues)
        {
            if (keyValues.Count > 0)
            {
                Departamento d = new Departamento()
                {
                    Clave = (int)keyValues["CLAVE"],
                    Nombre = (string)keyValues["NOMBRE"]
                };

                return d;
            }

            return null;
        }

        /// <summary>Costruye una lista de elementos con los datos obtenidos de un diccionario</summary>
        /// <param name="data">Lista de diccionarios con los datos de la Base de Datos</param>
        /// <returns>Lista de Departamentos</returns>
        public static List<Departamento> FromDictionaryListToList(List<Dictionary<string, object>> keyValues)
        {
            List<Departamento> ls = new List<Departamento>();

            foreach (var item in keyValues)
            {
                Departamento d = new Departamento();
                d.Clave = (int)item["CLAVE"];
                d.Nombre = (string)item["NOMBRE"];

                ls.Add(d);
            }

            ApplicationManager.InitGB();
            return ls;
        }
    }
}
