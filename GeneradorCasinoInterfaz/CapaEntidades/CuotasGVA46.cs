using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public class CuotasGVA46
    {

        public string FILLER { get; set; }

        public string ESTADO_VTO { get; set; }
        public DateTime FECHA_VTO { get; set; }

        public double IMPORTE_VT { get; set; }

        public string N_COMP { get; set; }
        public string T_COMP { get; set; }

        public string ESTADO_UNI { get; set; }

        public double IMP_VT_UNI { get; set; }

        public double IMP_VT_EXT { get; set; }
        public double IM_VT_UN_E { get; set; }

        public DateTime ALTERNATIVA_1 { get; set; }

        public double IMPORTE_TOTAL_1 { get; set; }

        public DateTime ALTERNATIVA_2 { get; set; }
        public double IMPORTE_TOTAL_2 { get; set; }

        public string AJUSTA_COBRO_FECHA_ALTERNATIVA { get; set; }

        public double UNIDADES_TOTAL_1 { get; set; }

        public double UNIDADES_TOTAL_2 { get; set; }
    }
}
