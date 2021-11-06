using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public class ArchivoFormato
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Letra { get; set; }
        public string PtoVenta { get; set; }
        public string NroFactura { get; set; }
        public string RazonSocial { get; set; }
        public string CuitDni { get; set; }
        public decimal Base { get; set; }
        public decimal Iva1 { get; set; }
        public decimal Iva2 { get; set; }
        public decimal Iva3 { get; set; }
        public decimal Percep_iva { get; set; }
        public decimal Percep_iibb { get; set; }
        public decimal Exento { get; set; }
        public decimal Total { get; set; }
        public string CaeCaea { get; set; }
        public DateTime CaeVencimiento { get; set; }
        public string Rvc { get; set; }
    }
}
