﻿using ServiciosASMX;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Servicios
{
    public class EnlaceSqlServer
    {
        private static SqlConnection conexion = null;

        public static SqlConnection Conexion
        {
            get { return EnlaceSqlServer.conexion; }
        }

        public static bool ConectarSqlServer()
        {

            bool estado = false;

            try
            {

                if (conexion == null)
                {
                    conexion = new SqlConnection();
                    conexion.ConnectionString = "Data Source=" + DatosEnlace.IpBaseDatos +
                        "; Initial Catalog=" + DatosEnlace.nombreBaseDatos +
                        "; User ID=" + DatosEnlace.usuarioBaseDatos +
                        "; Password=" + DatosEnlace.passworldBaseDatos +
                        "; MultipleActiveResultSets=True";
                    System.Threading.Thread.Sleep(750);
                }

                if (conexion.State == System.Data.ConnectionState.Closed)
                {
                    conexion.Open();
                }

                if (conexion.State == System.Data.ConnectionState.Broken)
                {
                    conexion.Close();
                    conexion.Open();
                }

                if (conexion.State == System.Data.ConnectionState.Connecting)
                {
                    while (conexion.State == System.Data.ConnectionState.Connecting)
                        System.Threading.Thread.Sleep(500);
                }

                estado = true;

            }
            catch (Exception e)
            {
                estado = false;
                Funciones.Logs("ENLACESQLSERVER", "Problemas al abrir la conexion; Captura error: " + e.Message);
                Funciones.Logs("ENLACESQLSERVER_DEBUG", e.StackTrace);
            }

            return estado;

        }
    }
}