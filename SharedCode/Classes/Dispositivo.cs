﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Classes
{
    class Dispositivo
    {
        public string Serie { get; set; }
        public string TDispositivo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Inventario { get; set; }

        /// <summary>
        /// Transforma una <see cref="List{T}"/> de elementos de un <see cref="Dictionary{TKey, TValue}"/> en una <see cref="List{T}"/> de objetos de la clase <see cref="Dispositivo"/>
        /// </summary>
        /// <param name="keyValues"><see cref="List{T}"/> de elementos <see cref="Dictionary{TKey, TValue}"/> resultante a una busqueda de la base de datos</param>
        /// <returns><see cref="List{T}"/> de objetos de la case <see cref="Dispositivo"/></returns>
        public static List<Dispositivo> FromDictionaryListToList(List<Dictionary<string, object>> keyValues)
        {
            if (keyValues.Count > 0)
            {
                List<Dispositivo> ls = new List<Dispositivo>();
                foreach (Dictionary<string, object> item in keyValues)
                {
                    Dispositivo d = new Dispositivo();
                    if (item["SERIE"].GetType() != typeof(System.DBNull))
                        d.Serie = ((string)item["SERIE"]).ToUpper();
                    
                    if (item["DISPOSITIVO"].GetType() != typeof(System.DBNull))
                        d.TDispositivo = ((string)item["DISPOSITIVO"]).ToUpper();
                    
                    if (item["MARCA"].GetType() != typeof(System.DBNull))
                        d.Marca = ((string)item["MARCA"]).ToUpper();
                    
                    if (item["MODELO"].GetType() != typeof(System.DBNull))
                        d.Modelo = ((string)item["MODELO"]).ToUpper();
                    
                    if (item["INVENTARIO"].GetType() != typeof(System.DBNull))
                        d.Inventario = ((string)item["INVENTARIO"]).ToUpper();

                    ls.Add(d);
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
        /// Transforma un <see cref="Dictionary{TKey, TValue}"/> a un objeto de clase <see cref="Dispositivo"/>
        /// </summary>
        /// <param name="keyValues"><see cref="Dictionary{TKey, TValue}"/> resultante a una busqueda de la base de datos</param>
        /// <returns>objeto de la clase <see cref="Dispositivo"/></returns>
        public static Dispositivo FromDictionarySingle(Dictionary<string, object> keyValues)
        {
            if (keyValues.Count > 0)
            {
                Dispositivo d = new Dispositivo()
                {
                    Serie = (string)keyValues["SERIE"],
                    TDispositivo = (string)keyValues["DISPOSITIVO"],
                    Marca = (string)keyValues["MARCA"],
                    Modelo = (string)keyValues["MODELO"],
                    Inventario = (string)keyValues["INVENTARIO"]
                };

                return d;
            }
            else return null;
        }
    }
}
