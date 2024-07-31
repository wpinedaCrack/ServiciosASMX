using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ServiciosASMX
{
    public class DatosEnlace
    {
        public static string IpBaseDatos = ConfigurationManager.AppSettings["ipBaseDatos"];
        public static string nombreBaseDatos = ConfigurationManager.AppSettings["nombreBaseDatos"];
        public static string usuarioBaseDatos = ConfigurationManager.AppSettings["usuarioBaseDatos"];
        public static string passworldBaseDatos = ConfigurationManager.AppSettings["passworldBaseDatos"];
        public static int timeOutSqlServer = int.Parse(ConfigurationManager.AppSettings["timeOutSqlServer"]);
    }
}