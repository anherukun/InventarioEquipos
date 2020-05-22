using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Classes
{
    class Usuario
    {
        public int Username { get; set; }
        public string NombreAD { get; set; }
        public string Trabajador { get; set; }
        public string Categoria { get; set; }
        public string Correo { get; set; }
        public bool PerfilMigrado { get; set; }
        public bool BuzonMigrado { get; set; }
        public string Contrasena { get; set; }

        public static string GetInsertSQL(Usuario u)
        {
            return $"INSERT INTO USUARIOS (USERNAME, NOMBREAD, TRABAJADOR, CATEGORIA, CORREO, PERFILMIGRADO, BUZONMIGRADO, CONTRASENA)" +
                $"VALUES ({u.Username}, \"{u.NombreAD}\", \"{u.Trabajador}\". \"{u.Categoria}\", \"{u.Correo}\", {u.PerfilMigrado}, {u.BuzonMigrado}, \"{u.Contrasena}\")";
        }

        /// <summary>
        /// Transforma una <see cref="List{T}"/> de elementos de un <see cref="Dictionary{TKey, TValue}"/> en una <see cref="List{T}"/> de objetos de la clase <see cref="Usuario"/>
        /// </summary>
        /// <param name="keyValues"><see cref="List{T}"/> de elementos <see cref="Dictionary{TKey, TValue}"/> resultante a una busqueda de la base de datos</param>
        /// <returns><see cref="List{T}"/> de objetos de la case <see cref="Usuario"/></returns>
        public static List<Usuario> FromDictionaryListToList(List<Dictionary<string, object>> keyValues)
        {
            if (keyValues.Count > 0)
            {
                List<Usuario> ls = new List<Usuario>();
                foreach (Dictionary<string, object> item in keyValues)
                {
                    Usuario u = new Usuario();
                    if (item["USERNAME"].GetType() != typeof(System.DBNull))
                        u.Username = ((int)item["USERNAME"]);

                    if (item["NOMBREAD"].GetType() != typeof(System.DBNull))
                        u.NombreAD = ((string)item["NOMBREAD"]).ToUpper();

                    if (item["TRABAJADOR"].GetType() != typeof(System.DBNull))
                        u.Trabajador = ((string)item["TRABAJADOR"]).ToUpper();

                    if (item["CATEGORIA"].GetType() != typeof(System.DBNull))
                        u.Categoria = ((string)item["CATEGORIA"]).ToUpper();

                    if (item["CORREO"].GetType() != typeof(System.DBNull))
                        u.Correo = ((string)item["CORREO"]);

                    if (item["PERFILMIGRADO"].GetType() != typeof(System.DBNull))
                        u.PerfilMigrado = ((bool)item["PERFILMIGRADO"]);

                    if (item["BUZONMIGRADO"].GetType() != typeof(System.DBNull))
                        u.BuzonMigrado = ((bool)item["BUZONMIGRADO"]);

                    if (item["CONTRASENA"].GetType() != typeof(System.DBNull))
                        u.Contrasena = ((string)item["CONTRASENA"]);

                    ls.Add(u);
                }
                //ls.Add(new Dispositivo()
                //{
                //    Serie = (string)item["SERIE"],
                //    TDispositivo = (string)item["DISPOSITIVO"],
                //    Marca = (string)item["MARCA"],
                //    Modelo = (string)item["MODELO"],
                //    Inventario = (string)item["INVENTARIO"]
                //});

                return ls;
            }
            else return null;
        }

        /// <summary>
        /// Transforma un <see cref="Dictionary{TKey, TValue}"/> a un objeto de clase <see cref="Usuario"/>
        /// </summary>
        /// <param name="keyValues"><see cref="Dictionary{TKey, TValue}"/> resultante a una busqueda de la base de datos</param>
        /// <returns>objeto de la clase <see cref="Usuario"/></returns>
        public static Usuario FromDictionarySingle(Dictionary<string, object> keyValues)
        {
            if (keyValues.Count > 0)
            {
                Usuario u = new Usuario();
                if (keyValues["USERNAME"].GetType() != typeof(System.DBNull))
                    u.Username = ((int)keyValues["USERNAME"]);

                if (keyValues["NOMBREAD"].GetType() != typeof(System.DBNull))
                    u.NombreAD = ((string)keyValues["NOMBREAD"]).ToUpper();

                if (keyValues["TRABAJADOR"].GetType() != typeof(System.DBNull))
                    u.Trabajador = ((string)keyValues["TRABAJADOR"]).ToUpper();

                if (keyValues["CATEGORIA"].GetType() != typeof(System.DBNull))
                    u.Categoria = ((string)keyValues["CATEGORIA"]).ToUpper();

                if (keyValues["CORREO"].GetType() != typeof(System.DBNull))
                    u.Correo = ((string)keyValues["CORREO"]);

                if (keyValues["PERFILMIGRADO"].GetType() != typeof(System.DBNull))
                    u.PerfilMigrado = ((bool)keyValues["PERFILMIGRADO"]);

                if (keyValues["BUZONMIGRADO"].GetType() != typeof(System.DBNull))
                    u.BuzonMigrado = ((bool)keyValues["BUZONMIGRADO"]);

                if (keyValues["CONTRASENA"].GetType() != typeof(System.DBNull))
                    u.Contrasena = ((string)keyValues["CONTRASENA"]);

                return u;
            }
            else return null;
        }
    }
}
