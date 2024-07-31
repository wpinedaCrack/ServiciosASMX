using Newtonsoft.Json;
using Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace ServiciosASMX
{
    /// <summary>
    /// Descripción breve de ServiciosASMX
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiciosASMX : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hola a todos";
        }

        [WebMethod(Description = "saludar")]
        public string Saludar(string nombre)
        {
            return "Hola Mi perrito " + nombre;
        }

        [WebMethod(Description = "Guardar Archivo de Log")]
        public string GuardarLog(string mensaje)
        {
            Funciones.Logs("LogServicio", mensaje);

            return "OK";
        }


        [WebMethod(Description = "Retornar Vector")]
        public string[] ObtenerFrutas()
        {
            string[] frutas = new string[3];

            frutas[0] = "Manzana";
            frutas[1] = "Pera";
            frutas[2] = "Mora";

            return frutas;
        }


        [WebMethod(Description = "Guardar Vector")]
        public string GuardarFrutas(string[] frutas)
        {
            foreach (string fruta in frutas)
            {
                Funciones.Logs("Frutas", fruta);
            }
            return "OK Guardar Vector";
        }

        [WebMethod]
        public List<Equipos> ObtenerEquipos()
        {
            List<Equipos> equipos = new List<Equipos>();

            equipos.Add(new Equipos { nombre = "Hila", pais = "Italia" });
            equipos.Add(new Equipos { nombre = "AJAX", pais = "Holanda" });

            return equipos;
        }

        [WebMethod]
        public string GuardarEquipos(Equipos[] equipos)
        {
            foreach (Equipos equipo in equipos)
            {
                Funciones.Logs("Equipos", equipo.nombre + " => " + equipo.pais);
            }
            return "Proceso Realizado con exito..";
        }

        [WebMethod]
        public string GuardarXML(string xml)
        {
            XmlDocument data_xml = new XmlDocument();
            data_xml.LoadXml(xml);

            XmlNode documento = data_xml.SelectSingleNode("documento");//primera etiqueta del XML

            string deporte = documento["deporte"].InnerText;///extrar el valor de la segunda etiqueta

            Funciones.Logs("XML", "Deporte: " + deporte + "; Equipos: ");

            XmlNodeList node_equipos = data_xml.GetElementsByTagName("equipos");

            XmlNodeList equipos = ((XmlElement)node_equipos[0]).GetElementsByTagName("equipo");

            foreach (XmlElement equipo in equipos)
            {
                string nombre = equipo.GetElementsByTagName("nombre")[0].InnerText;
                string pais = equipo.GetElementsByTagName("pais")[0].InnerText;

                Funciones.Logs("XML", nombre + " - " + pais);
            }
            return "Proceso Realizado con exito..";
        }

        [WebMethod]
        public string RetornarJson()
        {
            dynamic json = new Dictionary<string, dynamic>();
            json.Add("deporte", "futbol");
            List<Dictionary<string, string>> equipos = new List<Dictionary<string, string>>();

            Dictionary<string, string> equipo1 = new Dictionary<string, string>();
            equipo1.Add("nombre", "Manchester");
            equipo1.Add("pais", "Inglaterra");

            equipos.Add(equipo1);

            Dictionary<string, string> equipo2 = new Dictionary<string, string>();
            equipo2.Add("nombre", "Valencia");
            equipo2.Add("pais", "España");

            equipos.Add(equipo2);

            json.Add("Equipos", equipos);

            return JsonConvert.SerializeObject(json);
        }


        [WebMethod]
        public string GuardarJson(string Json)
        {
            var data_json = JsonConvert.DeserializeObject<DataJson>(Json);

            Funciones.Logs("JSON", "DEPORTES : " + data_json.deporte + " EQUIPOS : ");

            foreach (var equipo in data_json.Equipos)
            {
                Funciones.Logs("JSON", equipo.nombre + " - " + equipo.pais);
            }

            return "Proceso Realizado con exito..";
        }

        [WebMethod]
        public string ObtenerProductos()
        {

            List<Dictionary<string, string>> json = new List<Dictionary<string, string>>();

            if (!EnlaceSqlServer.ConectarSqlServer())
            {
                return "";
            }

            try
            {

                SqlCommand com = new SqlCommand("SELECT * FROM Productos", EnlaceSqlServer.Conexion);
                com.CommandType = CommandType.Text;
                com.CommandTimeout = DatosEnlace.timeOutSqlServer;


                SqlDataReader record = com.ExecuteReader();

                if (record.HasRows)
                {
                    Dictionary<string, string> row;

                    while (record.Read())
                    {
                        row = new Dictionary<string, string>();

                        for (int f = 0; f < record.FieldCount; f++)
                        {
                            row.Add(record.GetName(f), record.GetValue(f).ToString());
                        }
                        json.Add(row);
                    }
                }

                record.Close();
                record.Dispose();
                record = null;
                com.Dispose();
            }
            catch (Exception e)
            {
                Funciones.Logs("ObtenerProductos", e.Message);
                Funciones.Logs("ObtenerProductos_DEBUG", e.StackTrace);
            }

            return JsonConvert.SerializeObject(json);
        }


        [WebMethod]
        public Producto ObtenerIdProducto(string IdProducto)
        {
            Producto producto = new Producto();

            producto.IdProducto = 0;
            producto.nombre = "";
            producto.precio = 0;
            producto.stock = 0;

            if (!EnlaceSqlServer.ConectarSqlServer())
            {
                return producto;
            }

            try
            {
                SqlCommand com = new SqlCommand("SELECT * FROM Productos where IdProducto = " + IdProducto, EnlaceSqlServer.Conexion);
                com.CommandType = CommandType.Text;
                com.CommandTimeout = DatosEnlace.timeOutSqlServer;

                SqlDataReader record = com.ExecuteReader();

                if (record.HasRows && record.Read())
                {
                    producto.IdProducto = int.Parse(record.GetValue(0).ToString());
                    producto.nombre = record.GetValue(1).ToString();
                    producto.precio = double.Parse(record.GetValue(2).ToString());
                    producto.stock = int.Parse(record.GetValue(3).ToString());
                }

                record.Close();
                record.Dispose();
                record = null;
                com.Dispose();
            }
            catch (Exception e)
            {
                Funciones.Logs("ObtenerIdProducto", e.Message);
                Funciones.Logs("ObtenerIdProducto_DEBUG", e.StackTrace);
            }

            return producto;
        }

        [WebMethod]
        public string ActualizarProducto(Producto producto)
        {
            string resultado = string.Empty;

            if (!EnlaceSqlServer.ConectarSqlServer())
            {
                return "Error Conexión";
            }

            try
            {
                SqlCommand com = new SqlCommand("UPDATE Productos set nombre = @Nombre," +
                     " precio = @Precio, stock = @Stock" +
                     " WHERE IdProducto = @IdProducto", EnlaceSqlServer.Conexion);

                com.Parameters.AddWithValue("@Nombre", producto.nombre);
                com.Parameters.AddWithValue("@Stock", producto.stock);
                com.Parameters.AddWithValue("@Precio", producto.precio);
                com.Parameters.AddWithValue("@IdProducto", producto.IdProducto);

                int cantidad = com.ExecuteNonQuery();

                if (cantidad > 0)
                {
                    resultado = "Producto Actualizado con Exito.";
                }
                else
                {
                    resultado = "Error al Actualizar el Producto.";
                }

                com.Dispose();
            }
            catch (Exception ex)
            {
                Funciones.Logs("ActualizarProducto", ex.Message);
                Funciones.Logs("ActualizarProducto_DEBUG", ex.StackTrace);
            }

            return resultado;
        }

        [WebMethod]
        public int GuardarProducto(Producto producto)
        {
            int idProducto = 0;
            if (!EnlaceSqlServer.ConectarSqlServer())
            {
                return 0;
            }

            try
            {
                SqlCommand com = new SqlCommand("INSERT INTO Productos ( nombre, precio, stock ) " +
                    " VALUES ( @Nombre, @Precio, @Stock ) " +
                    " ; SELECT CAST(scope_identity() AS int) ", EnlaceSqlServer.Conexion);

                com.Parameters.AddWithValue("@Nombre", producto.nombre);
                com.Parameters.AddWithValue("@Precio", producto.precio);
                com.Parameters.AddWithValue("@Stock", producto.stock);

                idProducto = (int)com.ExecuteScalar();

                com.Dispose();
            }
            catch (Exception ex)
            {
                Funciones.Logs("GuardarProducto", ex.Message);
                Funciones.Logs("GuardarProducto_DEBUG", ex.StackTrace);
            }

            return idProducto;
        }

        [WebMethod]
        public string EliminarProducto(string IdProducto)
        {
            string resultado = string.Empty;

            if (!EnlaceSqlServer.ConectarSqlServer())
            {
                return "Error Conexión";
            }

            try
            {
                SqlCommand com = new SqlCommand("DELETE Productos where IdProducto = " + IdProducto, EnlaceSqlServer.Conexion);

                int cantidad = com.ExecuteNonQuery();

                if (cantidad > 0)
                {
                    resultado = "Producto Borrado con Exito.";
                }
                else
                {
                    resultado = "Error al Borrar el Producto.";
                }

                com.Dispose();
            }
            catch (Exception e)
            {
                Funciones.Logs("EliminarProducto", e.Message);
                Funciones.Logs("EliminarProducto_DEBUG", e.StackTrace);
            }

            return resultado;
        }
        public AuthUser Users;

        [WebMethod]
        [SoapHeader ("Users")]
        public string ObtenerFecha()
        {
            if(Users!=null && Users.IsValid())
            {
                return DateTime.Now.Year.ToString()+"-"+ DateTime.Now.Month.ToString() + "-"+DateTime.Now.Day.ToString();
            }
            else
            {
                return "Credenciales Incorrectas";
            }            
        }


    }
}
