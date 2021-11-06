using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace CapaNegocios
{
    public class NegociosCasino
    {
        public List<CapaEntidades.ArchivoFormato> LeerDatosDelArchivo(string ruta)
        {
            try
            {
                List<CapaEntidades.ArchivoFormato> ListaDeDatos = new List<CapaEntidades.ArchivoFormato>();
                using (var reader = new StreamReader(ruta))
                {
                    bool SaltarPrimeraFila = true;
                    while (!reader.EndOfStream)
                    {
                        var linea = reader.ReadLine();
                        var dato = linea.Split(';');
                        if (SaltarPrimeraFila)
                        {
                            SaltarPrimeraFila = false;
                        }
                        else
                        {
                            if (!ListaDeDatos.Any(x => x.Tipo == dato[1].ToString() && x.Letra == dato[2].ToString() && x.PtoVenta == dato[3].ToString() && x.NroFactura == dato[4].ToString()))
                            {
                                CapaEntidades.ArchivoFormato archivo = new CapaEntidades.ArchivoFormato();
                                archivo.Fecha = Convert.ToDateTime(dato[0].ToString()).Date;
                                archivo.Tipo = dato[1].ToString();
                                archivo.Letra = dato[2].ToString();
                                archivo.PtoVenta = dato[3].ToString();
                                archivo.NroFactura = dato[4].ToString();
                                archivo.RazonSocial = dato[5].ToString();
                                archivo.CuitDni = dato[6].ToString();
                                archivo.Base = Convert.ToDecimal(dato[7].ToString());
                                archivo.Iva1 = Convert.ToDecimal(dato[8].ToString());
                                archivo.Iva2 = Convert.ToDecimal(dato[9].ToString());
                                archivo.Iva3 = Convert.ToDecimal(dato[10].ToString());
                                archivo.Percep_iva = Convert.ToDecimal(dato[11].ToString());
                                archivo.Percep_iibb = Convert.ToDecimal(dato[12].ToString());
                                archivo.Exento = Convert.ToDecimal(dato[13].ToString());
                                archivo.Total = Convert.ToDecimal(dato[14].ToString());
                                archivo.CaeCaea = dato[15].ToString();
                                archivo.CaeVencimiento = Convert.ToDateTime(dato[16].ToString());
                                archivo.Rvc = dato[17].ToString();

                                ListaDeDatos.Add(archivo);
                            }
                        }
                    }
                }
                return ListaDeDatos;
            }
            catch (Exception)
            {
                MessageBox.Show("Por favor CIERRE el archivo antes de importar", MessageBoxIcon.Error.ToString(), MessageBoxButtons.OK);
                throw;
            }
        }
        public bool GuardarEnTango(string ruta)
        {
            try
            {
                CapaDatos.DatosCasino dbCasino = new CapaDatos.DatosCasino();
                List<CapaEntidades.ArchivoFormato> archivoFactura = new List<CapaEntidades.ArchivoFormato>();
                List<CapaEntidades.CabeceraGVA12> listaDeFacturasCargadas = new List<CapaEntidades.CabeceraGVA12>();
                List<string> CompNoPasados = new List<string>();
                List<string> ComprobExistentes = new List<string>();
                archivoFactura = LeerDatosDelArchivo(ruta);

                foreach (var factura in archivoFactura)
                {
                    CapaEntidades.CabeceraGVA12 cabeceraGVA12 = new CapaEntidades.CabeceraGVA12();
                    if (factura.Tipo == "FC")
                    {
                        cabeceraGVA12.T_COMP = "FAC";
                        cabeceraGVA12.TCOMP_IN_V = "FC";
                    }
                    else
                    {
                        cabeceraGVA12.T_COMP = "N/C";
                        cabeceraGVA12.TCOMP_IN_V = "CC";
                    }
                    cabeceraGVA12.N_COMP = factura.Letra + factura.PtoVenta.PadLeft(5, '0') + factura.NroFactura.PadLeft(8, '0');
                    if (dbCasino.ValidarComprobante(cabeceraGVA12.T_COMP, cabeceraGVA12.N_COMP))
                    {
                        CompNoPasados.Add($"NO SE AGREGO: el Número de Comprobante: {cabeceraGVA12.N_COMP} de tipo: {cabeceraGVA12.T_COMP} , porque ya se encuentra registrado en tango");
                    }
                    else
                    {
                        string cod_cliente = "";
                        if (factura.CuitDni == "" || factura.CuitDni == Convert.ToString(0))
                        {
                            cod_cliente = ConfigurationManager.AppSettings.Get("CodClienteConsumidorFinalBar").ToString();
                        }
                        else
                        {
                            cod_cliente = dbCasino.TraerDatos("GVA14", "COD_CLIENT", true, "REPLACE(CUIT,'-','')", true, factura.CuitDni.Replace("-", ""));
                        }
                        string idCategoriaIva = dbCasino.TraerDatos("GVA14", "ID_CATEGORIA_IVA", true, "COD_CLIENT", true, cod_cliente);
                        
                        string comprob = cabeceraGVA12.T_COMP;//---------------------------------------------------------------------------------------------------------------------------------------

                        if (comprob == "N/C")
                        {
                            comprob = "CRE";
                        }
                        int Talonario = Convert.ToInt32(dbCasino.TraerTalonario(factura.Letra, comprob, factura.PtoVenta.PadLeft(5, '0')));
                        if (Talonario != 0)
                        {
                            cabeceraGVA12.TALONARIO = Talonario;
                            cabeceraGVA12.FILLER = "";
                            cabeceraGVA12.AFEC_STK = true;
                            cabeceraGVA12.CANC_COMP = "";
                            cabeceraGVA12.CANT_HOJAS = 1;
                            string codCategoriaIva = dbCasino.TraerDatos("CATEGORIA_IVA", "COD_CATEGORIA_IVA", true, "ID_CATEGORIA_IVA", true, idCategoriaIva);
                            cabeceraGVA12.CAT_IVA = codCategoriaIva;// traer dato
                            cabeceraGVA12.CENT_STK = "N";
                            cabeceraGVA12.CENT_COB = "N";
                            cabeceraGVA12.CITI_OPERA = "O";
                            cabeceraGVA12.CITI_TIPO = "B";
                            cabeceraGVA12.COD_CAJA = 0;
                            cabeceraGVA12.COD_CLIENT = cod_cliente;
                            if (cabeceraGVA12.COD_CLIENT != "")
                            {
                                cabeceraGVA12.COD_SUCURS = "";
                                cabeceraGVA12.COD_TRANSP = dbCasino.TraerDatos("GVA14", "COD_TRANSP", true, "COD_CLIENT", true, cabeceraGVA12.COD_CLIENT);
                                cabeceraGVA12.COD_VENDED = ConfigurationManager.AppSettings.Get("CodVendedor").ToString();
                                cabeceraGVA12.COND_VTA = 1;
                                cabeceraGVA12.CONTABILIZ = true;
                                cabeceraGVA12.CONTFISCAL = false;
                                cabeceraGVA12.COTIZ = 1;
                                cabeceraGVA12.DESC_PANT = 0;
                                if (cabeceraGVA12.T_COMP.Equals("N/C"))
                                {
                                    cabeceraGVA12.ESTADO = "CTA";
                                }
                                else
                                {
                                    cabeceraGVA12.ESTADO = "PEN";
                                }
                                cabeceraGVA12.ESTADO_STK = "";
                                cabeceraGVA12.EXPORTADO = false;
                                cabeceraGVA12.FECHA_ANU = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.FECHA_EMIS = factura.Fecha;
                                cabeceraGVA12.ID_CIERRE = 0;
                                cabeceraGVA12.IMPORTE = Math.Abs(Convert.ToDouble(factura.Total));
                                cabeceraGVA12.IMPORTE_BO = 0;
                                cabeceraGVA12.IMPORTE_EX = 0;
                                cabeceraGVA12.IMPORTE_FL = 0;
                                cabeceraGVA12.IMPORTE_GR = Math.Abs(Convert.ToDouble(factura.Base));
                                cabeceraGVA12.IMPORTE_IN = 0;
                                cabeceraGVA12.IMPORTE_IV = Math.Abs(Convert.ToDouble(factura.Iva1));
                                cabeceraGVA12.IMP_TICK_N = 0;
                                cabeceraGVA12.IMP_TICK_P = 0;
                                cabeceraGVA12.LEYENDA = "";
                                cabeceraGVA12.LOTE = 0;
                                cabeceraGVA12.MON_CTE = true;
                                cabeceraGVA12.MOTI_ANU = "";
                                cabeceraGVA12.NRO_DE_LIS = 1;
                                cabeceraGVA12.NRO_SUCURS = 0;
                                cabeceraGVA12.ORIGEN = "";
                                cabeceraGVA12.PORC_BONIF = 0;
                                cabeceraGVA12.PORC_PRO = 0;
                                cabeceraGVA12.PORC_REC = 0;
                                cabeceraGVA12.PORC_TICK = 0;
                                cabeceraGVA12.PROPINA = 0;
                                cabeceraGVA12.PROPINA_EX = 0;
                                cabeceraGVA12.PTO_VTA = false;
                                cabeceraGVA12.REC_PANT = 0;
                                cabeceraGVA12.TICKET = "N";
                                cabeceraGVA12.TIPO_ASIEN = ConfigurationManager.AppSettings.Get("TipoAsiento").ToString();
                                cabeceraGVA12.TIPO_EXPOR = "";
                                cabeceraGVA12.TIPO_VEND = "V";
                                cabeceraGVA12.T_FORM = "";
                                cabeceraGVA12.UNIDADES = cabeceraGVA12.IMPORTE;
                                cabeceraGVA12.LOTE_ANU = 0;
                                cabeceraGVA12.PORC_INT = 0;
                                cabeceraGVA12.PORC_FLE = 0;
                                cabeceraGVA12.ESTADO_UNI = cabeceraGVA12.ESTADO;
                                cabeceraGVA12.ID_CFISCAL = "";
                                cabeceraGVA12.NUMERO_Z = 0;
                                cabeceraGVA12.HORA_COMP = "0";
                                cabeceraGVA12.SENIA = false;
                                cabeceraGVA12.ID_TURNO = 0;
                                cabeceraGVA12.ID_TURNOX = 0;
                                cabeceraGVA12.HORA_ANU = "0";
                                cabeceraGVA12.CCONTROL = "";
                                cabeceraGVA12.ID_A_RENTA = 0;
                                cabeceraGVA12.COD_CLASIF = "";
                                cabeceraGVA12.AFEC_CIERR = "";
                                cabeceraGVA12.CAICAE = factura.CaeCaea;
                                cabeceraGVA12.CAICAE_VTO = factura.CaeVencimiento;
                                cabeceraGVA12.DOC_ELECTR = false;
                                cabeceraGVA12.SERV_DESDE = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.SERV_HASTA = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.CANT_IMP = 0;
                                cabeceraGVA12.CANT_MAIL = 0;
                                cabeceraGVA12.ULT_IMP = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.ULT_MAIL = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.MORA_SOBRE = "N";
                                cabeceraGVA12.ESTADO_ANT = "";
                                cabeceraGVA12.T_DOC_DTE = "";
                                cabeceraGVA12.DTE_ANU = "";
                                cabeceraGVA12.FOLIO_ANU = "";
                                cabeceraGVA12.REBAJA_DEB = false;
                                cabeceraGVA12.SUCURS_SII = 0;
                                cabeceraGVA12.EXENTA = false;
                                cabeceraGVA12.MOTIVO_DTE = 0;
                                cabeceraGVA12.IMPOR_EXT = 0;
                                cabeceraGVA12.CERRADO = false;
                                cabeceraGVA12.IMP_BO_EXT = 0;
                                cabeceraGVA12.IMP_EX_EXT = 0;
                                cabeceraGVA12.IMP_FL_EXT = 0;
                                cabeceraGVA12.IMP_GR_EXT = 0;
                                cabeceraGVA12.IMP_IN_EXT = 0;
                                cabeceraGVA12.IMP_IV_EXT = 0;
                                cabeceraGVA12.IM_TIC_N_E = 0;
                                cabeceraGVA12.IM_TIC_P_E = 0;
                                cabeceraGVA12.UNIDAD_EXT = 0;
                                cabeceraGVA12.PROPIN_EXT = 0;
                                cabeceraGVA12.PRO_EX_EXT = 0;
                                cabeceraGVA12.REC_PAN_EX = 0;
                                cabeceraGVA12.DES_PAN_EX = 0;
                                cabeceraGVA12.T_DTO_COMP = "";
                                cabeceraGVA12.RECARGO_PV = 0;
                                cabeceraGVA12.NCOMP_IN_V = 0;// guardar en el insert
                                if (factura.Rvc.Equals("8161") && cabeceraGVA12.T_COMP.Equals("FAC"))
                                {
                                    cabeceraGVA12.ID_ASIENTO_MODELO_GV = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ID_ASIENTO_MODELO_GV_1_FAC").ToString());
                                }
                                else if (factura.Rvc.Equals("8161") && cabeceraGVA12.T_COMP.Equals("N/C"))
                                {
                                    cabeceraGVA12.ID_ASIENTO_MODELO_GV = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ID_ASIENTO_MODELO_GV_2_NC").ToString());
                                }
                                else if (!factura.Rvc.Equals("8161") && cabeceraGVA12.T_COMP.Equals("FAC"))
                                {
                                    cabeceraGVA12.ID_ASIENTO_MODELO_GV = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ID_ASIENTO_MODELO_GV_3_FAC").ToString());
                                }
                                else
                                {
                                    cabeceraGVA12.ID_ASIENTO_MODELO_GV = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ID_ASIENTO_MODELO_GV_4_NC").ToString());
                                }
                                cabeceraGVA12.GENERA_ASIENTO = "S";
                                cabeceraGVA12.FECHA_INGRESO = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.HORA_INGRESO = "";
                                cabeceraGVA12.USUARIO_INGRESO = "";
                                cabeceraGVA12.TERMINAL_INGRESO = "";
                                cabeceraGVA12.FECHA_ULTIMA_MODIFICACION = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.HORA_ULTIMA_MODIFICACION = "";
                                cabeceraGVA12.USUA_ULTIMA_MODIFICACION = "";
                                cabeceraGVA12.TERM_ULTIMA_MODIFICACION = "";
                                cabeceraGVA12.ID_PUESTO_CAJA = 0;
                                cabeceraGVA12.NCOMP_IN_ORIGEN = 0;
                                cabeceraGVA12.OBS_COMERC = "";
                                cabeceraGVA12.OBSERVAC = "";
                                cabeceraGVA12.LEYENDA_1 = "Interfaz Simphony Tango";
                                cabeceraGVA12.LEYENDA_2 = "";
                                cabeceraGVA12.LEYENDA_3 = "";
                                cabeceraGVA12.LEYENDA_4 = "";
                                cabeceraGVA12.LEYENDA_5 = "";
                                cabeceraGVA12.IMP_CIGARRILLOS = 0;
                                cabeceraGVA12.POR_CIGARRILLOS = 0;
                                cabeceraGVA12.ID_MOTIVO_NOTA_CREDITO = null;
                                cabeceraGVA12.FECHA_DESCARGA_PDF = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.HORA_DESCARGA_PDF = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cabeceraGVA12.USUARIO_DESCARGA_PDF = "";
                                cabeceraGVA12.ID_DIRECCION_ENTREGA = Convert.ToInt32(dbCasino.TraerDatos("DIRECCION_ENTREGA", "ID_DIRECCION_ENTREGA", true, "COD_CLIENTE", true, cabeceraGVA12.COD_CLIENT + "'  AND HABILITADO='S"));
                                cabeceraGVA12.ID_HISTORIAL_RENDICION = 0;
                                cabeceraGVA12.IMPUTACION_MODIFICADA = "";
                                cabeceraGVA12.PUBLICADO_WEB_CLIENTES = "";
                                cabeceraGVA12.RG_3572_TIPO_OPERACION_HABITUAL_VENTAS = ConfigurationManager.AppSettings.Get("RG_3572_TIPO_OPERACION_HABITUAL_VENTAS");
                                cabeceraGVA12.RG_3685_TIPO_OPERACION_VENTAS = "0";
                                cabeceraGVA12.DESCRIPCION_FACTURA = "";
                                cabeceraGVA12.ID_NEXO_COBRANZAS_PAGO = 0;
                                cabeceraGVA12.TIPO_TRANSACCION_VENTA = 0;
                                cabeceraGVA12.TIPO_TRANSACCION_COMPRA = 0;
                                cabeceraGVA12.COMPROBANTE_CREDITO = "N";

                                CapaEntidades.DetalleGVA53 detalleGVA53 = new CapaEntidades.DetalleGVA53();
                                detalleGVA53.FILLER = "";
                                detalleGVA53.CANC_CRE = 0;
                                detalleGVA53.CANTIDAD = 1;
                                detalleGVA53.CAN_EQUI_V = 1;
                                detalleGVA53.CENT_STK = "N";
                                if (factura.Rvc.Equals("8161"))
                                {
                                    detalleGVA53.COD_ARTICU = ConfigurationManager.AppSettings.Get("CodArticulo_9");
                                }
                                else
                                {
                                    detalleGVA53.COD_ARTICU = ConfigurationManager.AppSettings.Get("CodArticulo");
                                }
                                detalleGVA53.COD_DEPOSI = "1";
                                detalleGVA53.FALTAN_REM = 0;
                                detalleGVA53.FECHA_MOV = factura.Fecha;
                                detalleGVA53.IMP_NETO_P = cabeceraGVA12.IMPORTE;
                                detalleGVA53.IMP_RE_PAN = 0;
                                detalleGVA53.N_COMP = cabeceraGVA12.N_COMP;
                                detalleGVA53.N_PARTIDA = "0";
                                detalleGVA53.N_RENGL_V = 1;
                                detalleGVA53.PORC_DTO = 0;
                                detalleGVA53.PORC_IVA = 0;
                                detalleGVA53.PPP_EX = 0;
                                detalleGVA53.PPP_LO = 0;
                                detalleGVA53.PRECIO_NET = cabeceraGVA12.IMPORTE;
                                detalleGVA53.PRECIO_PAN = cabeceraGVA12.IMPORTE;
                                detalleGVA53.PREC_ULC_E = 0;
                                detalleGVA53.PREC_ULC_L = 0;
                                detalleGVA53.PROMOCION = false;
                                detalleGVA53.T_COMP = cabeceraGVA12.T_COMP;
                                detalleGVA53.TCOMP_IN_V = cabeceraGVA12.TCOMP_IN_V;
                                detalleGVA53.COD_CLASIF = "2";
                                detalleGVA53.IM_NET_P_E = 0;
                                detalleGVA53.IM_RE_PA_E = 0;
                                detalleGVA53.PREC_NET_E = 0;
                                detalleGVA53.PREC_PAN_E = 0;
                                detalleGVA53.PR_ULC_E_E = 0;
                                detalleGVA53.PR_ULC_L_E = 0;
                                detalleGVA53.PRECSINDTO = 0;
                                detalleGVA53.IMPORTE_EXENTO = 0;
                                detalleGVA53.IMPORTE_GRAVADO = cabeceraGVA12.IMPORTE;
                                detalleGVA53.CANTIDAD_2 = 0;
                                detalleGVA53.FALTAN_REM_2 = 0;
                                detalleGVA53.ID_MEDIDA_VENTAS = null;
                                detalleGVA53.ID_MEDIDA_STOCK_2 = null;
                                detalleGVA53.ID_MEDIDA_STOCK = null;
                                detalleGVA53.UNIDAD_MEDIDA_SELECCIONADA = "P";
                                detalleGVA53.RENGL_PADR = 0;
                                detalleGVA53.COD_ARTICU_KIT = "";
                                detalleGVA53.INSUMO_KIT_SEPARADO = false;
                                detalleGVA53.PRECIO_FECHA = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                detalleGVA53.PRECIO_LISTA = 0;
                                detalleGVA53.PRECIO_BONIF = 0;
                                detalleGVA53.PORC_DTO_PARAM = 0;
                                detalleGVA53.FECHA_MODIFICACION_PRECIO = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                detalleGVA53.USUARIO_MODIFICACION_PRECIO = null;
                                detalleGVA53.TERMINAL_MODIFICACION_PRECIO = null;
                                detalleGVA53.ITEM_ESPECTACULO = "";

                                cabeceraGVA12.Detalle.Add(detalleGVA53);

                                CapaEntidades.CuotasGVA46 cuotas = new CapaEntidades.CuotasGVA46();
                                cuotas.FILLER = "";
                                cuotas.ESTADO_VTO = cabeceraGVA12.ESTADO;
                                cuotas.FECHA_VTO = cabeceraGVA12.CAICAE_VTO;
                                cuotas.IMPORTE_VT = cabeceraGVA12.IMPORTE;
                                cuotas.N_COMP = detalleGVA53.N_COMP;
                                cuotas.T_COMP = detalleGVA53.T_COMP;
                                cuotas.ESTADO_UNI = cabeceraGVA12.ESTADO;
                                cuotas.IMP_VT_UNI = 0;
                                cuotas.IMP_VT_EXT = 0;
                                cuotas.IM_VT_UN_E = 0;
                                cuotas.ALTERNATIVA_1 = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cuotas.IMPORTE_TOTAL_1 = 0;
                                cuotas.ALTERNATIVA_2 = DateTime.ParseExact("18000101", "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                                cuotas.IMPORTE_TOTAL_2 = 0;
                                cuotas.AJUSTA_COBRO_FECHA_ALTERNATIVA = "N";
                                cuotas.UNIDADES_TOTAL_1 = 0;
                                cuotas.UNIDADES_TOTAL_2 = 0;

                                if (cuotas.T_COMP != "N/C")
                                {
                                    cabeceraGVA12.Cuotas.Add(cuotas);
                                }

                                CapaEntidades.ImpuestosGVA42 impuestos1 = new CapaEntidades.ImpuestosGVA42();
                                impuestos1.FILLER = "";
                                impuestos1.COD_ALICUO = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CodAlicuo_iva1"));
                                impuestos1.IMPORTE = Math.Abs(Convert.ToDouble(factura.Iva1));
                                impuestos1.N_COMP = cuotas.N_COMP;
                                impuestos1.NETO_GRAV = cabeceraGVA12.IMPORTE;
                                impuestos1.PERCEP = 0;
                                impuestos1.PORCENTAJE = Convert.ToDouble(dbCasino.TraerDatos("GVA41", "PORCENTAJE", true, "COD_ALICUO", true, impuestos1.COD_ALICUO.ToString()));
                                impuestos1.T_COMP = cuotas.T_COMP;
                                impuestos1.COD_IMPUES = "";
                                impuestos1.COD_SII = "";
                                impuestos1.IMP_EXT = 0;
                                impuestos1.NE_GRAV_EX = 0;
                                impuestos1.PERCEP_EXT = 0;

                                if (impuestos1.IMPORTE != 0)
                                {
                                    cabeceraGVA12.Impuesto.Add(impuestos1);
                                }

                                CapaEntidades.ImpuestosGVA42 impuestos2 = new CapaEntidades.ImpuestosGVA42();
                                impuestos2.FILLER = "";
                                impuestos2.COD_ALICUO = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CodAlicuo_iva2"));
                                impuestos2.IMPORTE = Math.Abs(Convert.ToDouble(factura.Iva2));
                                impuestos2.N_COMP = impuestos1.N_COMP;
                                impuestos2.NETO_GRAV = cabeceraGVA12.IMPORTE;
                                impuestos2.PERCEP = Convert.ToDouble(factura.Percep_iva);
                                impuestos2.PORCENTAJE = Convert.ToDouble(dbCasino.TraerDatos("GVA41", "PORCENTAJE", true, "COD_ALICUO", true, impuestos2.COD_ALICUO.ToString()));
                                impuestos2.T_COMP = impuestos1.T_COMP;
                                impuestos2.COD_IMPUES = "";
                                impuestos2.COD_SII = "";
                                impuestos2.IMP_EXT = 0;
                                impuestos2.NE_GRAV_EX = 0;
                                impuestos2.PERCEP_EXT = 0;

                                if (impuestos2.IMPORTE != 0)
                                {
                                    cabeceraGVA12.Impuesto.Add(impuestos2);
                                }

                                CapaEntidades.ImpuestosGVA42 impuestos3 = new CapaEntidades.ImpuestosGVA42();
                                impuestos3.FILLER = "";
                                impuestos3.COD_ALICUO = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CodAlicuo_iva3"));
                                impuestos3.IMPORTE = Math.Abs(Convert.ToDouble(factura.Iva3));
                                impuestos3.N_COMP = impuestos2.N_COMP;
                                impuestos3.NETO_GRAV = cabeceraGVA12.IMPORTE;
                                impuestos3.PERCEP = 0;
                                impuestos3.PORCENTAJE = Convert.ToDouble(dbCasino.TraerDatos("GVA41", "PORCENTAJE", true, "COD_ALICUO", true, impuestos3.COD_ALICUO.ToString()));
                                impuestos3.T_COMP = impuestos2.T_COMP;
                                impuestos3.COD_IMPUES = "";
                                impuestos3.COD_SII = "";
                                impuestos3.IMP_EXT = 0;
                                impuestos3.NE_GRAV_EX = 0;
                                impuestos3.PERCEP_EXT = 0;

                                if (impuestos3.IMPORTE != 0)
                                {
                                    cabeceraGVA12.Impuesto.Add(impuestos3);
                                }

                                CapaEntidades.ImpuestosGVA42 impuestosPercepIva = new CapaEntidades.ImpuestosGVA42();
                                impuestosPercepIva.FILLER = "";
                                impuestosPercepIva.COD_ALICUO = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CodAlicuoPercepIva"));
                                impuestosPercepIva.IMPORTE = Math.Abs(Convert.ToDouble(factura.Percep_iva));
                                impuestosPercepIva.N_COMP = impuestos3.N_COMP;
                                impuestosPercepIva.NETO_GRAV = cabeceraGVA12.IMPORTE;
                                impuestosPercepIva.PERCEP = 0;
                                impuestosPercepIva.PORCENTAJE = Convert.ToDouble(dbCasino.TraerDatos("GVA41", "PORCENTAJE", true, "COD_ALICUO", true, impuestosPercepIva.COD_ALICUO.ToString()));
                                impuestosPercepIva.T_COMP = impuestos3.T_COMP;
                                impuestosPercepIva.COD_IMPUES = "";
                                impuestosPercepIva.COD_SII = "";
                                impuestosPercepIva.IMP_EXT = 0;
                                impuestosPercepIva.NE_GRAV_EX = 0;
                                impuestosPercepIva.PERCEP_EXT = 0;

                                if (impuestosPercepIva.IMPORTE != 0)
                                {
                                    cabeceraGVA12.Impuesto.Add(impuestosPercepIva);
                                }

                                CapaEntidades.ImpuestosGVA42 impuestosPercepIIBB = new CapaEntidades.ImpuestosGVA42();
                                impuestosPercepIIBB.FILLER = "";
                                impuestosPercepIIBB.COD_ALICUO = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CodAlicuoPercepIIBB"));
                                impuestosPercepIIBB.IMPORTE = Math.Abs(Convert.ToDouble(factura.Percep_iibb));
                                impuestosPercepIIBB.N_COMP = impuestosPercepIva.N_COMP;
                                impuestosPercepIIBB.NETO_GRAV = cabeceraGVA12.IMPORTE;
                                impuestosPercepIIBB.PERCEP = 0;
                                impuestosPercepIIBB.PORCENTAJE = Convert.ToDouble(dbCasino.TraerDatos("GVA41", "PORCENTAJE", true, "COD_ALICUO", true, impuestosPercepIIBB.COD_ALICUO.ToString()));
                                impuestosPercepIIBB.T_COMP = impuestosPercepIva.T_COMP;
                                impuestosPercepIIBB.COD_IMPUES = "";
                                impuestosPercepIIBB.COD_SII = "";
                                impuestosPercepIIBB.IMP_EXT = 0;
                                impuestosPercepIIBB.NE_GRAV_EX = 0;
                                impuestosPercepIIBB.PERCEP_EXT = 0;

                                if (impuestosPercepIIBB.IMPORTE != 0)
                                {
                                    cabeceraGVA12.Impuesto.Add(impuestosPercepIIBB);
                                }
                                listaDeFacturasCargadas.Add(cabeceraGVA12);
                            }
                            else
                            {
                                CompNoPasados.Add("No se agrego el número de comprobante: " + cabeceraGVA12.N_COMP + ", el CUIT: " + factura.CuitDni + "no existe en Tango.");
                            }
                        }
                        else
                        {
                            CompNoPasados.Add($"No se agrego el registro: {cabeceraGVA12.N_COMP} de tipo: {cabeceraGVA12.T_COMP} , porque el talonario no existe en tango");
                        }

                    }
                }
                foreach (var item in CompNoPasados)
                {
                    using (StreamWriter LogError = new StreamWriter(Application.StartupPath + "\\Errores.log", true))
                    {
                        LogError.WriteLine(DateTime.Now.ToString("dd-MM-yyy HH:mm") + " - " + item);
                    }
                }
                bool guardo = true;
                if (listaDeFacturasCargadas.Count > 0)
                {
                    foreach (var datos in listaDeFacturasCargadas)
                    {
                        dbCasino.GuardarComprobante(datos);
                    }
                    dbCasino.Reset_ID();
                }
                else
                {
                    guardo = false;
                }
                return guardo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
