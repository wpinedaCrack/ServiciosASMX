using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosASMX
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string nombre { get; set; }
        public double precio { get; set; }
        public int stock { get; set; }
    }
}