using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using CapaEntidades;

namespace CapaDatos
{
    public class DatosCasino
    {
        public string Conexion = ConfigurationManager.AppSettings.Get("Conexion").ToString();
        public bool ValidarComprobante(string tipo, string n_comp)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());
            SqlCommand cm = new SqlCommand();
            bool Existe = false;
            try
            {
                using (cn)
                {
                    cn.Open();
                    cm.Connection = cn;
                    cm.CommandText = "SELECT * FROM GVA12 WHERE T_COMP='" + tipo + "'AND N_COMP='" + n_comp + "'";
                    cm.ExecuteNonQuery();
                    SqlDataReader dr = cm.ExecuteReader();
                    if (dr.Read())
                    {
                        Existe = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Existe;
        }
        public int TraerTalonario(string tipo, string tComprob, string ptoVenta)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());
            SqlCommand cm = new SqlCommand();
            int Talonario = 0;
            try
            {
                using (cn)
                {
                    cn.Open();
                    cm.Connection = cn;
                    cm.CommandText = "SELECT TALONARIO FROM GVA43 WHERE TIPO='" + tipo + "'AND COMPROB='" + tComprob + "' AND SUCURSAL=" + ptoVenta + "";
                    cm.ExecuteNonQuery();
                    SqlDataReader dr = cm.ExecuteReader();
                    if (dr.Read())
                    {
                        Talonario = Convert.ToInt32(dr["TALONARIO"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Talonario;
        }
        public string TraerDatos(string sTabla, string sCampoAtraer, bool bTextoCampoAtraer, string sCampoABuscar, bool bTexto, string sValorAComparar)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());

            string sSQL;
            SqlCommand cm = new SqlCommand();
            SqlDataReader dr;
            string sValor;


            try
            {
                cn.Open();
                if (bTexto == false)
                {
                    sSQL = "SELECT " + sCampoAtraer + " FROM " + sTabla + " WHERE " + sCampoABuscar + "=" + sValorAComparar;
                }
                else
                {
                    sSQL = "SELECT " + sCampoAtraer + " FROM " + sTabla + " WHERE " + sCampoABuscar + "='" + sValorAComparar + "'";
                }
                cm.Connection = cn;
                cm.CommandText = sSQL;
                dr = cm.ExecuteReader();



                if (dr.Read())
                {
                    sValor = dr[0].ToString();
                }
                else if (bTextoCampoAtraer == true)
                {
                    sValor = "";
                }
                else
                {
                    sValor = "";
                }
            }
            catch
            {
                if (bTextoCampoAtraer == true)
                    sValor = "";
                else
                    sValor = "";
            }
            if (cn.State == ConnectionState.Open)
                cn.Close();
            return sValor;
        }

        public void GuardarComprobante(CapaEntidades.CabeceraGVA12 comprobante)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr = null;

            try
            {
                cn.Open();
                tr = cn.BeginTransaction();
                cm = InsertarCabecera(comprobante);
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();
                foreach (var item in comprobante.Detalle)
                {
                    cm = InsertarDetalle(item);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }
                
                foreach (var item in comprobante.Cuotas)
                {
                    cm = InsertarCuotas(item);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }
                
                foreach (var item in comprobante.Impuesto)
                {
                    cm = InsertarImpuestos(item);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }
                cm = InsertarAsientoComprobante();
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();

                tr.Commit();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public SqlCommand InsertarCabecera(CapaEntidades.CabeceraGVA12 cabecera)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @" 
                        DECLARE @NCOMP_IN_V INT = (SELECT ISNULL(MAX(NCOMP_IN_V), 0) FROM GVA12) + 1;
                        INSERT INTO GVA12
                        (
                        FILLER,	
                        AFEC_STK,	
                        CANC_COMP,	
                        CANT_HOJAS,	
                        CAT_IVA,
                        CENT_STK,	
                        CENT_COB,	
                        CITI_OPERA,	
                        CITI_TIPO,	
                        COD_CAJA,
                        COD_CLIENT,	
                        COD_SUCURS,	
                        COD_TRANSP,	
                        COD_VENDED,	
                        COND_VTA,	
                        CONTABILIZ,	
                        CONTFISCAL,	
                        COTIZ,	
                        DESC_PANT,	
                        ESTADO,	
                        ESTADO_STK,	
                        EXPORTADO,	
                        FECHA_ANU,	
                        FECHA_EMIS,	
                        ID_CIERRE,	
                        IMPORTE,	
                        IMPORTE_BO,	
                        IMPORTE_EX,
                        IMPORTE_FL,
                        IMPORTE_GR,
                        IMPORTE_IN,	
                        IMPORTE_IV,	
                        IMP_TICK_N,	
                        IMP_TICK_P,	
                        LEYENDA,	
                        LOTE,	
                        MON_CTE,	
                        MOTI_ANU,	
                        NRO_DE_LIS,	
                        NRO_SUCURS,	
                        N_COMP,
                        ORIGEN,
                        PORC_BONIF,	
                        PORC_PRO,	
                        PORC_REC,	
                        PORC_TICK,	
                        PROPINA,	
                        PROPINA_EX,	
                        PTO_VTA,
                        REC_PANT,	
                        TALONARIO,	
                        TCOMP_IN_V,	
                        TICKET,	
                        TIPO_ASIEN,	
                        TIPO_EXPOR,	
                        TIPO_VEND,	
                        T_COMP,	
                        T_FORM,	
                        UNIDADES,	
                        LOTE_ANU,	
                        PORC_INT,	
                        PORC_FLE,	
                        ESTADO_UNI,	
                        ID_CFISCAL,	
                        NUMERO_Z,	
                        HORA_COMP,	
                        SENIA,	
                        ID_TURNO,	
                        ID_TURNOX,	
                        HORA_ANU,	
                        CCONTROL,	
                        ID_A_RENTA,	
                        COD_CLASIF,	
                        AFEC_CIERR,	
                        CAICAE,	
                        CAICAE_VTO,	
                        DOC_ELECTR,	
                        SERV_DESDE,	
                        SERV_HASTA,	
                        CANT_IMP,	
                        CANT_MAIL,	
                        ULT_IMP,	
                        ULT_MAIL,	
                        MORA_SOBRE,	
                        ESTADO_ANT,	
                        T_DOC_DTE,	
                        DTE_ANU,	
                        FOLIO_ANU,	
                        REBAJA_DEB,	
                        SUCURS_SII,	
                        EXENTA,
                        MOTIVO_DTE,
                        IMPOR_EXT,	
                        CERRADO,	
                        IMP_BO_EXT,	
                        IMP_EX_EXT,	
                        IMP_FL_EXT,	
                        IMP_GR_EXT,	
                        IMP_IN_EXT,	
                        IMP_IV_EXT,
                        IM_TIC_N_E,	
                        IM_TIC_P_E,	
                        UNIDAD_EXT,	
                        PROPIN_EXT,	
                        PRO_EX_EXT,	
                        REC_PAN_EX,	
                        DES_PAN_EX,	
                        T_DTO_COMP,	
                        RECARGO_PV,	
                        NCOMP_IN_V,	
                        ID_ASIENTO_MODELO_GV,	
                        GENERA_ASIENTO,	
                        FECHA_INGRESO,	
                        HORA_INGRESO,
                        USUARIO_INGRESO,
                        TERMINAL_INGRESO,
                        FECHA_ULTIMA_MODIFICACION,	
                        HORA_ULTIMA_MODIFICACION,
                        USUA_ULTIMA_MODIFICACION,
                        TERM_ULTIMA_MODIFICACION,
                        ID_PUESTO_CAJA,
                        NCOMP_IN_ORIGEN,
                        OBS_COMERC,
                        OBSERVAC,
                        LEYENDA_1,
                        LEYENDA_2,	
                        LEYENDA_3,
                        LEYENDA_4,
                        LEYENDA_5,
                        IMP_CIGARRILLOS,	
                        POR_CIGARRILLOS,	
                        ID_MOTIVO_NOTA_CREDITO,
                        FECHA_DESCARGA_PDF,	
                        HORA_DESCARGA_PDF,
                        USUARIO_DESCARGA_PDF,
                        ID_DIRECCION_ENTREGA,
                        ID_HISTORIAL_RENDICION,
                        IMPUTACION_MODIFICADA,
                        PUBLICADO_WEB_CLIENTES,
                        RG_3572_TIPO_OPERACION_HABITUAL_VENTAS,
                        RG_3685_TIPO_OPERACION_VENTAS,
                        DESCRIPCION_FACTURA,
                        ID_NEXO_COBRANZAS_PAGO,
                        TIPO_TRANSACCION_VENTA,
                        TIPO_TRANSACCION_COMPRA,
                        COMPROBANTE_CREDITO
                        )
                        VALUES
                        (
                        @FILLER,	
                        @AFEC_STK,	
                        @CANC_COMP,	
                        @CANT_HOJAS,	
                        @CAT_IVA,	
                        @CENT_STK,	
                        @CENT_COB,	
                        @CITI_OPERA,	
                        @CITI_TIPO,	
                        @COD_CAJA,
                        @COD_CLIENT,	
                        @COD_SUCURS,	
                        @COD_TRANSP,	
                        @COD_VENDED,	
                        @COND_VTA,	
                        @CONTABILIZ,	
                        @CONTFISCAL,	
                        @COTIZ,	
                        @DESC_PANT,	
                        @ESTADO,	
                        @ESTADO_STK,	
                        @EXPORTADO,	
                        @FECHA_ANU,	
                        @FECHA_EMIS,	
                        @ID_CIERRE,	
                        @IMPORTE,	
                        @IMPORTE_BO,	
                        @IMPORTE_EX,
                        @IMPORTE_FL,	
                        @IMPORTE_GR,	
                        @IMPORTE_IN,	
                        @IMPORTE_IV,	
                        @IMP_TICK_N,	
                        @IMP_TICK_P,	
                        @LEYENDA,	
                        @LOTE,	
                        @MON_CTE,	
                        @MOTI_ANU,	
                        @NRO_DE_LIS,	
                        @NRO_SUCURS,	
                        @N_COMP,
                        @ORIGEN,	
                        @PORC_BONIF,	
                        @PORC_PRO,	
                        @PORC_REC,	
                        @PORC_TICK,	
                        @PROPINA,	
                        @PROPINA_EX,	
                        @PTO_VTA,
                        @REC_PANT,	
                        @TALONARIO,	
                        @TCOMP_IN_V,	
                        @TICKET,	
                        @TIPO_ASIEN,	
                        @TIPO_EXPOR,	
                        @TIPO_VEND,	
                        @T_COMP,	
                        @T_FORM,	
                        @UNIDADES,	
                        @LOTE_ANU,	
                        @PORC_INT,	
                        @PORC_FLE,	
                        @ESTADO_UNI,	
                        @ID_CFISCAL,	
                        @NUMERO_Z,	
                        @HORA_COMP,	
                        @SENIA,	
                        @ID_TURNO,	
                        @ID_TURNOX,
                        @HORA_ANU,	
                        @CCONTROL,	
                        @ID_A_RENTA,	
                        @COD_CLASIF,	
                        @AFEC_CIERR,	
                        @CAICAE,	
                        @CAICAE_VTO,	
                        @DOC_ELECTR,	
                        @SERV_DESDE,	
                        @SERV_HASTA,	
                        @CANT_IMP,	
                        @CANT_MAIL,	
                        @ULT_IMP,	
                        @ULT_MAIL,	
                        @MORA_SOBRE,	
                        @ESTADO_ANT,	
                        @T_DOC_DTE,	
                        @DTE_ANU,	
                        @FOLIO_ANU,	
                        @REBAJA_DEB,	
                        @SUCURS_SII,	
                        @EXENTA,
                        @MOTIVO_DTE,
                        @IMPOR_EXT,	
                        @CERRADO,	
                        @IMP_BO_EXT,	
                        @IMP_EX_EXT,
                        @IMP_FL_EXT,	
                        @IMP_GR_EXT,	
                        @IMP_IN_EXT,	
                        @IMP_IV_EXT,
                        @IM_TIC_N_E,	
                        @IM_TIC_P_E,	
                        @UNIDAD_EXT,	
                        @PROPIN_EXT,	
                        @PRO_EX_EXT,	
                        @REC_PAN_EX,	
                        @DES_PAN_EX,	
                        @T_DTO_COMP,	
                        @RECARGO_PV,
                        @NCOMP_IN_V,	
                        @ID_ASIENTO_MODELO_GV,	
                        @GENERA_ASIENTO,	
                        @FECHA_INGRESO,	
                        @HORA_INGRESO,
                        @USUARIO_INGRESO,
                        @TERMINAL_INGRESO,
                        @FECHA_ULTIMA_MODIFICACION,	
                        @HORA_ULTIMA_MODIFICACION,
                        @USUA_ULTIMA_MODIFICACION,
                        @TERM_ULTIMA_MODIFICACION,
                        @ID_PUESTO_CAJA,
                        @NCOMP_IN_ORIGEN,
                        @OBS_COMERC,
                        @OBSERVAC,
                        @LEYENDA_1,
                        @LEYENDA_2,
                        @LEYENDA_3,
                        @LEYENDA_4,
                        @LEYENDA_5,
                        @IMP_CIGARRILLOS,	
                        @POR_CIGARRILLOS,	
                        @ID_MOTIVO_NOTA_CREDITO,
                        @FECHA_DESCARGA_PDF,	
                        @HORA_DESCARGA_PDF,
                        @USUARIO_DESCARGA_PDF,
                        @ID_DIRECCION_ENTREGA,
                        @ID_HISTORIAL_RENDICION,
                        @IMPUTACION_MODIFICADA,
                        @PUBLICADO_WEB_CLIENTES,
                        @RG_3572_TIPO_OPERACION_HABITUAL_VENTAS,
                        @RG_3685_TIPO_OPERACION_VENTAS,
                        @DESCRIPCION_FACTURA,
                        @ID_NEXO_COBRANZAS_PAGO,
                        @TIPO_TRANSACCION_VENTA,
                        @TIPO_TRANSACCION_COMPRA,
                        @COMPROBANTE_CREDITO
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = cabecera.FILLER;
                cm.Parameters.Add("@AFEC_STK", SqlDbType.Bit).Value = cabecera.AFEC_STK;
                cm.Parameters.Add("@CANC_COMP", SqlDbType.VarChar).Value = cabecera.CANC_COMP;
                cm.Parameters.Add("@CANT_HOJAS", SqlDbType.Int).Value = cabecera.CANT_HOJAS;
                cm.Parameters.Add("@CAT_IVA", SqlDbType.VarChar).Value = cabecera.CAT_IVA;
                cm.Parameters.Add("@CENT_STK", SqlDbType.VarChar).Value = cabecera.CENT_STK;
                cm.Parameters.Add("@CENT_COB", SqlDbType.VarChar).Value = cabecera.CENT_COB;
                cm.Parameters.Add("@CITI_OPERA", SqlDbType.VarChar).Value = cabecera.CITI_OPERA;
                cm.Parameters.Add("@CITI_TIPO", SqlDbType.VarChar).Value = cabecera.CITI_TIPO;
                cm.Parameters.Add("@COD_CAJA", SqlDbType.Int).Value = cabecera.COD_CAJA;
                cm.Parameters.Add("@COD_CLIENT", SqlDbType.VarChar).Value = cabecera.COD_CLIENT;
                cm.Parameters.Add("@COD_SUCURS", SqlDbType.VarChar).Value = cabecera.COD_SUCURS;
                cm.Parameters.Add("@COD_TRANSP", SqlDbType.VarChar).Value = cabecera.COD_TRANSP;
                cm.Parameters.Add("@COD_VENDED", SqlDbType.VarChar).Value = cabecera.COD_VENDED;
                cm.Parameters.Add("@COND_VTA", SqlDbType.Int).Value = cabecera.COND_VTA;
                cm.Parameters.Add("@CONTABILIZ", SqlDbType.Bit).Value = cabecera.CONTABILIZ;
                cm.Parameters.Add("@CONTFISCAL", SqlDbType.Bit).Value = cabecera.CONTFISCAL;
                cm.Parameters.Add("@COTIZ", SqlDbType.Decimal).Value = cabecera.COTIZ;
                cm.Parameters.Add("@DESC_PANT", SqlDbType.Decimal).Value = cabecera.DESC_PANT;
                cm.Parameters.Add("@ESTADO", SqlDbType.VarChar).Value = cabecera.ESTADO;
                cm.Parameters.Add("@ESTADO_STK", SqlDbType.VarChar).Value = cabecera.ESTADO_STK;
                cm.Parameters.Add("@EXPORTADO", SqlDbType.Bit).Value = cabecera.EXPORTADO;
                cm.Parameters.Add("@FECHA_ANU", SqlDbType.DateTime).Value = cabecera.FECHA_ANU;
                cm.Parameters.Add("@FECHA_EMIS", SqlDbType.DateTime).Value = cabecera.FECHA_EMIS;
                cm.Parameters.Add("@ID_CIERRE", SqlDbType.Float).Value = cabecera.ID_CIERRE;
                cm.Parameters.Add("@IMPORTE", SqlDbType.Decimal).Value = cabecera.IMPORTE;
                cm.Parameters.Add("@IMPORTE_BO", SqlDbType.Decimal).Value = cabecera.IMPORTE_BO;
                cm.Parameters.Add("@IMPORTE_EX", SqlDbType.Decimal).Value = cabecera.IMPORTE_EX;
                cm.Parameters.Add("@IMPORTE_FL", SqlDbType.Decimal).Value = cabecera.IMPORTE_FL;
                cm.Parameters.Add("@IMPORTE_GR", SqlDbType.Decimal).Value = cabecera.IMPORTE_GR;
                cm.Parameters.Add("@IMPORTE_IN", SqlDbType.Decimal).Value = cabecera.IMPORTE_IN;
                cm.Parameters.Add("@IMPORTE_IV", SqlDbType.Decimal).Value = cabecera.IMPORTE_IV;
                cm.Parameters.Add("@IMP_TICK_N", SqlDbType.Decimal).Value = cabecera.IMP_TICK_N;
                cm.Parameters.Add("@IMP_TICK_P", SqlDbType.Decimal).Value = cabecera.IMP_TICK_P;
                cm.Parameters.Add("@LEYENDA", SqlDbType.VarChar).Value = cabecera.LEYENDA;
                cm.Parameters.Add("@LOTE", SqlDbType.Float).Value = cabecera.LOTE;
                cm.Parameters.Add("@MON_CTE", SqlDbType.Bit).Value = cabecera.MON_CTE;
                cm.Parameters.Add("@MOTI_ANU", SqlDbType.VarChar).Value = cabecera.MOTI_ANU;
                cm.Parameters.Add("@NRO_DE_LIS", SqlDbType.Int).Value = cabecera.NRO_DE_LIS;
                cm.Parameters.Add("@NRO_SUCURS", SqlDbType.Int).Value = cabecera.NRO_SUCURS;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = cabecera.N_COMP;
                cm.Parameters.Add("@ORIGEN", SqlDbType.VarChar).Value = cabecera.ORIGEN;
                cm.Parameters.Add("@PORC_BONIF", SqlDbType.Decimal).Value = cabecera.PORC_BONIF;
                cm.Parameters.Add("@PORC_PRO", SqlDbType.Decimal).Value = cabecera.PORC_PRO;
                cm.Parameters.Add("@PORC_REC", SqlDbType.Decimal).Value = cabecera.PORC_REC;
                cm.Parameters.Add("@PORC_TICK", SqlDbType.Decimal).Value = cabecera.PORC_TICK;
                cm.Parameters.Add("@PROPINA", SqlDbType.Decimal).Value = cabecera.PROPINA;
                cm.Parameters.Add("@PROPINA_EX", SqlDbType.Decimal).Value = cabecera.PROPINA_EX;
                cm.Parameters.Add("@PTO_VTA", SqlDbType.Bit).Value = cabecera.PTO_VTA;
                cm.Parameters.Add("@REC_PANT", SqlDbType.Decimal).Value = cabecera.REC_PANT;
                cm.Parameters.Add("@TALONARIO", SqlDbType.Int).Value = cabecera.TALONARIO;
                cm.Parameters.Add("@TCOMP_IN_V", SqlDbType.VarChar).Value = cabecera.TCOMP_IN_V;
                cm.Parameters.Add("@TICKET", SqlDbType.VarChar).Value = cabecera.TICKET;
                cm.Parameters.Add("@TIPO_ASIEN", SqlDbType.VarChar).Value = cabecera.TIPO_ASIEN;
                cm.Parameters.Add("@TIPO_EXPOR", SqlDbType.VarChar).Value = cabecera.TIPO_EXPOR;
                cm.Parameters.Add("@TIPO_VEND", SqlDbType.VarChar).Value = cabecera.TIPO_VEND;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = cabecera.T_COMP;
                cm.Parameters.Add("@T_FORM", SqlDbType.VarChar).Value = cabecera.T_FORM;
                cm.Parameters.Add("@UNIDADES", SqlDbType.Decimal).Value = cabecera.UNIDADES;
                cm.Parameters.Add("@LOTE_ANU", SqlDbType.Float).Value = cabecera.LOTE_ANU;
                cm.Parameters.Add("@PORC_INT", SqlDbType.Decimal).Value = cabecera.PORC_INT;
                cm.Parameters.Add("@PORC_FLE", SqlDbType.Decimal).Value = cabecera.PORC_FLE;
                cm.Parameters.Add("@ESTADO_UNI", SqlDbType.VarChar).Value = cabecera.ESTADO_UNI;
                cm.Parameters.Add("@ID_CFISCAL", SqlDbType.VarChar).Value = cabecera.ID_CFISCAL;
                cm.Parameters.Add("@NUMERO_Z", SqlDbType.Float).Value = cabecera.NUMERO_Z;
                cm.Parameters.Add("@HORA_COMP", SqlDbType.VarChar).Value = cabecera.HORA_COMP;
                cm.Parameters.Add("@SENIA", SqlDbType.Bit).Value = cabecera.SENIA;
                cm.Parameters.Add("@ID_TURNO", SqlDbType.Float).Value = cabecera.ID_TURNO;
                cm.Parameters.Add("@ID_TURNOX", SqlDbType.Float).Value = cabecera.ID_TURNOX;
                cm.Parameters.Add("@HORA_ANU", SqlDbType.VarChar).Value = cabecera.HORA_ANU;
                cm.Parameters.Add("@CCONTROL", SqlDbType.VarChar).Value = cabecera.CCONTROL;
                cm.Parameters.Add("@ID_A_RENTA", SqlDbType.Float).Value = cabecera.ID_A_RENTA;
                cm.Parameters.Add("@COD_CLASIF", SqlDbType.VarChar).Value = cabecera.COD_CLASIF;
                cm.Parameters.Add("@AFEC_CIERR", SqlDbType.VarChar).Value = cabecera.AFEC_CIERR;
                cm.Parameters.Add("@CAICAE", SqlDbType.VarChar).Value = cabecera.CAICAE;
                cm.Parameters.Add("@CAICAE_VTO", SqlDbType.DateTime).Value = cabecera.CAICAE_VTO;
                cm.Parameters.Add("@DOC_ELECTR", SqlDbType.Bit).Value = cabecera.DOC_ELECTR;
                cm.Parameters.Add("@SERV_DESDE", SqlDbType.DateTime).Value = cabecera.SERV_DESDE;
                cm.Parameters.Add("@SERV_HASTA", SqlDbType.DateTime).Value = cabecera.SERV_HASTA;
                cm.Parameters.Add("@CANT_IMP", SqlDbType.Int).Value = cabecera.CANT_IMP;
                cm.Parameters.Add("@CANT_MAIL", SqlDbType.Int).Value = cabecera.CANT_MAIL;
                cm.Parameters.Add("@ULT_IMP", SqlDbType.DateTime).Value = cabecera.ULT_IMP;
                cm.Parameters.Add("@ULT_MAIL", SqlDbType.DateTime).Value = cabecera.ULT_MAIL;
                cm.Parameters.Add("@MORA_SOBRE", SqlDbType.VarChar).Value = cabecera.MORA_SOBRE;
                cm.Parameters.Add("@ESTADO_ANT", SqlDbType.VarChar).Value = cabecera.ESTADO_ANT;
                cm.Parameters.Add("@T_DOC_DTE", SqlDbType.VarChar).Value = cabecera.T_DOC_DTE;
                cm.Parameters.Add("@DTE_ANU", SqlDbType.VarChar).Value = cabecera.DTE_ANU;
                cm.Parameters.Add("@FOLIO_ANU", SqlDbType.VarChar).Value = cabecera.FOLIO_ANU;
                cm.Parameters.Add("@REBAJA_DEB", SqlDbType.Bit).Value = cabecera.REBAJA_DEB;
                cm.Parameters.Add("@SUCURS_SII", SqlDbType.Float).Value = cabecera.SUCURS_SII;
                cm.Parameters.Add("@EXENTA", SqlDbType.Bit).Value = cabecera.EXENTA;
                cm.Parameters.Add("@MOTIVO_DTE", SqlDbType.Int).Value = cabecera.MOTIVO_DTE;
                cm.Parameters.Add("@IMPOR_EXT", SqlDbType.Decimal).Value = cabecera.IMPOR_EXT;
                cm.Parameters.Add("@CERRADO", SqlDbType.Bit).Value = cabecera.CERRADO;
                cm.Parameters.Add("@IMP_BO_EXT", SqlDbType.Decimal).Value = cabecera.IMP_BO_EXT;
                cm.Parameters.Add("@IMP_EX_EXT", SqlDbType.Decimal).Value = cabecera.IMP_EX_EXT;
                cm.Parameters.Add("@IMP_FL_EXT", SqlDbType.Decimal).Value = cabecera.IMP_FL_EXT;
                cm.Parameters.Add("@IMP_GR_EXT", SqlDbType.Decimal).Value = cabecera.IMP_GR_EXT;
                cm.Parameters.Add("@IMP_IN_EXT", SqlDbType.Decimal).Value = cabecera.IMP_IN_EXT;
                cm.Parameters.Add("@IMP_IV_EXT", SqlDbType.Decimal).Value = cabecera.IMP_IV_EXT;
                cm.Parameters.Add("@IM_TIC_N_E", SqlDbType.Decimal).Value = cabecera.IM_TIC_N_E;
                cm.Parameters.Add("@IM_TIC_P_E", SqlDbType.Decimal).Value = cabecera.IM_TIC_P_E;
                cm.Parameters.Add("@UNIDAD_EXT", SqlDbType.Decimal).Value = cabecera.UNIDAD_EXT;
                cm.Parameters.Add("@PROPIN_EXT", SqlDbType.Decimal).Value = cabecera.PROPIN_EXT;
                cm.Parameters.Add("@PRO_EX_EXT", SqlDbType.Decimal).Value = cabecera.PRO_EX_EXT;
                cm.Parameters.Add("@REC_PAN_EX", SqlDbType.Decimal).Value = cabecera.REC_PAN_EX;
                cm.Parameters.Add("@DES_PAN_EX", SqlDbType.Decimal).Value = cabecera.DES_PAN_EX;
                cm.Parameters.Add("@T_DTO_COMP", SqlDbType.VarChar).Value = cabecera.T_DTO_COMP;
                cm.Parameters.Add("@RECARGO_PV", SqlDbType.VarChar).Value = cabecera.RECARGO_PV;
                cm.Parameters.Add("@ID_ASIENTO_MODELO_GV", SqlDbType.Int).Value = cabecera.ID_ASIENTO_MODELO_GV;
                cm.Parameters.Add("@GENERA_ASIENTO", SqlDbType.Char).Value = cabecera.GENERA_ASIENTO;
                cm.Parameters.Add("@FECHA_INGRESO", SqlDbType.DateTime).Value = cabecera.FECHA_INGRESO;
                cm.Parameters.Add("@HORA_INGRESO", SqlDbType.VarChar).Value = cabecera.HORA_INGRESO;
                cm.Parameters.Add("@USUARIO_INGRESO", SqlDbType.VarChar).Value = cabecera.USUARIO_INGRESO;
                cm.Parameters.Add("@TERMINAL_INGRESO", SqlDbType.VarChar).Value = cabecera.TERMINAL_INGRESO;
                cm.Parameters.Add("@FECHA_ULTIMA_MODIFICACION", SqlDbType.DateTime).Value = cabecera.FECHA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@HORA_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = cabecera.HORA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@USUA_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = cabecera.USUA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@TERM_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = cabecera.TERM_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@ID_PUESTO_CAJA", SqlDbType.Int).Value = cabecera.ID_PUESTO_CAJA;
                cm.Parameters.Add("@NCOMP_IN_ORIGEN", SqlDbType.Float).Value = cabecera.NCOMP_IN_ORIGEN;
                cm.Parameters.Add("@OBS_COMERC", SqlDbType.Text).Value = cabecera.OBS_COMERC;
                cm.Parameters.Add("@OBSERVAC", SqlDbType.Text).Value = cabecera.OBSERVAC;
                cm.Parameters.Add("@LEYENDA_1", SqlDbType.VarChar).Value = cabecera.LEYENDA_1;
                cm.Parameters.Add("@LEYENDA_2", SqlDbType.VarChar).Value = cabecera.LEYENDA_2;
                cm.Parameters.Add("@LEYENDA_3", SqlDbType.VarChar).Value = cabecera.LEYENDA_3;
                cm.Parameters.Add("@LEYENDA_4", SqlDbType.VarChar).Value = cabecera.LEYENDA_4;
                cm.Parameters.Add("@LEYENDA_5", SqlDbType.VarChar).Value = cabecera.LEYENDA_5;
                cm.Parameters.Add("@IMP_CIGARRILLOS", SqlDbType.Decimal).Value = cabecera.IMP_CIGARRILLOS;
                cm.Parameters.Add("@POR_CIGARRILLOS", SqlDbType.Decimal).Value = cabecera.POR_CIGARRILLOS;
                cm.Parameters.Add("@ID_MOTIVO_NOTA_CREDITO", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@FECHA_DESCARGA_PDF", SqlDbType.DateTime).Value = cabecera.FECHA_DESCARGA_PDF;
                cm.Parameters.Add("@HORA_DESCARGA_PDF", SqlDbType.DateTime).Value = cabecera.HORA_DESCARGA_PDF;
                cm.Parameters.Add("@USUARIO_DESCARGA_PDF", SqlDbType.VarChar).Value = cabecera.USUARIO_DESCARGA_PDF;
                cm.Parameters.Add("@ID_DIRECCION_ENTREGA", SqlDbType.Int).Value = cabecera.ID_DIRECCION_ENTREGA;
                cm.Parameters.Add("@ID_HISTORIAL_RENDICION", SqlDbType.Int).Value = cabecera.ID_HISTORIAL_RENDICION;
                cm.Parameters.Add("@IMPUTACION_MODIFICADA", SqlDbType.VarChar).Value = cabecera.IMPUTACION_MODIFICADA;
                cm.Parameters.Add("@PUBLICADO_WEB_CLIENTES", SqlDbType.VarChar).Value = cabecera.PUBLICADO_WEB_CLIENTES;
                cm.Parameters.Add("@RG_3572_TIPO_OPERACION_HABITUAL_VENTAS", SqlDbType.VarChar).Value = cabecera.RG_3572_TIPO_OPERACION_HABITUAL_VENTAS;
                cm.Parameters.Add("@RG_3685_TIPO_OPERACION_VENTAS", SqlDbType.VarChar).Value = cabecera.RG_3685_TIPO_OPERACION_VENTAS;
                cm.Parameters.Add("@DESCRIPCION_FACTURA", SqlDbType.VarChar).Value = cabecera.DESCRIPCION_FACTURA;
                cm.Parameters.Add("@ID_NEXO_COBRANZAS_PAGO", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@TIPO_TRANSACCION_VENTA", SqlDbType.Int).Value = cabecera.TIPO_TRANSACCION_VENTA;
                cm.Parameters.Add("@TIPO_TRANSACCION_COMPRA", SqlDbType.VarChar).Value = cabecera.TIPO_TRANSACCION_COMPRA;
                cm.Parameters.Add("@COMPROBANTE_CREDITO", SqlDbType.Char).Value = cabecera.COMPROBANTE_CREDITO;

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public SqlCommand InsertarDetalle(CapaEntidades.DetalleGVA53 detalle)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @" INSERT INTO GVA53
                        (
                        FILLER,
                        CANC_CRE,
                        CANTIDAD,
                        CAN_EQUI_V,
                        CENT_STK,
                        COD_ARTICU,
                        COD_DEPOSI,
                        FALTAN_REM,
                        FECHA_MOV,
                        IMP_NETO_P,
                        IMP_RE_PAN,
                        N_COMP,
                        N_PARTIDA,
                        N_RENGL_V,
                        PORC_DTO,
                        PORC_IVA,
                        PPP_EX,
                        PPP_LO,
                        PRECIO_NET,
                        PRECIO_PAN,
                        PREC_ULC_E,
                        PREC_ULC_L,
                        PROMOCION,
                        T_COMP,
                        TCOMP_IN_V,
                        COD_CLASIF,
                        IM_NET_P_E,
                        IM_RE_PA_E,
                        PREC_NET_E,
                        PREC_PAN_E,
                        PR_ULC_E_E,
                        PR_ULC_L_E,
                        PRECSINDTO,
                        IMPORTE_EXENTO,
                        IMPORTE_GRAVADO,	
                        CANTIDAD_2,
                        FALTAN_REM_2,
                        ID_MEDIDA_VENTAS,
                        ID_MEDIDA_STOCK_2,
                        ID_MEDIDA_STOCK,
                        UNIDAD_MEDIDA_SELECCIONADA,
                        RENGL_PADR,
                        COD_ARTICU_KIT,
                        INSUMO_KIT_SEPARADO,
                        PRECIO_FECHA,
                        PRECIO_LISTA,
                        PRECIO_BONIF,
                        PORC_DTO_PARAM,
                        FECHA_MODIFICACION_PRECIO,
                        USUARIO_MODIFICACION_PRECIO,
                        TERMINAL_MODIFICACION_PRECIO,
                        ITEM_ESPECTACULO
                        )
                        VALUES
                        (
                        @FILLER,
                        @CANC_CRE,
                        @CANTIDAD,
                        @CAN_EQUI_V,
                        @CENT_STK,
                        @COD_ARTICU,
                        @COD_DEPOSI,
                        @FALTAN_REM,
                        @FECHA_MOV,
                        @IMP_NETO_P,
                        @IMP_RE_PAN,
                        @N_COMP,
                        @N_PARTIDA,
                        @N_RENGL_V,
                        @PORC_DTO,
                        @PORC_IVA,
                        @PPP_EX,
                        @PPP_LO,
                        @PRECIO_NET,
                        @PRECIO_PAN,
                        @PREC_ULC_E,
                        @PREC_ULC_L,
                        @PROMOCION,
                        @T_COMP,
                        @TCOMP_IN_V,
                        @COD_CLASIF,
                        @IM_NET_P_E,
                        @IM_RE_PA_E,
                        @PREC_NET_E,
                        @PREC_PAN_E,
                        @PR_ULC_E_E,
                        @PR_ULC_L_E,
                        @PRECSINDTO,
                        @IMPORTE_EXENTO,
                        @IMPORTE_GRAVADO,
                        @CANTIDAD_2,	
                        @FALTAN_REM_2,
                        @ID_MEDIDA_VENTAS,
                        @ID_MEDIDA_STOCK_2,
                        @ID_MEDIDA_STOCK,
                        @UNIDAD_MEDIDA_SELECCIONADA,
                        @RENGL_PADR,
                        @COD_ARTICU_KIT,
                        @INSUMO_KIT_SEPARADO,
                        @PRECIO_FECHA,
                        @PRECIO_LISTA,
                        @PRECIO_BONIF,
                        @PORC_DTO_PARAM,
                        @FECHA_MODIFICACION_PRECIO,
                        @USUARIO_MODIFICACION_PRECIO,
                        @TERMINAL_MODIFICACION_PRECIO,
                        @ITEM_ESPECTACULO
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = detalle.FILLER;
                cm.Parameters.Add("@CANC_CRE", SqlDbType.Decimal).Value = detalle.CANC_CRE;
                cm.Parameters.Add("@CANTIDAD", SqlDbType.Decimal).Value = detalle.CANTIDAD;
                cm.Parameters.Add("@CAN_EQUI_V", SqlDbType.Decimal).Value = detalle.CAN_EQUI_V;
                cm.Parameters.Add("@CENT_STK", SqlDbType.VarChar).Value = detalle.CENT_STK;
                cm.Parameters.Add("@COD_ARTICU", SqlDbType.VarChar).Value = detalle.COD_ARTICU;
                cm.Parameters.Add("@COD_DEPOSI", SqlDbType.VarChar).Value = detalle.COD_DEPOSI;
                cm.Parameters.Add("@FALTAN_REM", SqlDbType.Decimal).Value = detalle.FALTAN_REM;
                cm.Parameters.Add("@FECHA_MOV", SqlDbType.DateTime).Value = detalle.FECHA_MOV;
                cm.Parameters.Add("@IMP_NETO_P", SqlDbType.Decimal).Value = detalle.IMP_NETO_P;
                cm.Parameters.Add("@IMP_RE_PAN", SqlDbType.Decimal).Value = detalle.IMP_RE_PAN;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = detalle.N_COMP;
                cm.Parameters.Add("@N_PARTIDA", SqlDbType.VarChar).Value = detalle.N_PARTIDA;
                cm.Parameters.Add("@N_RENGL_V", SqlDbType.Int).Value = detalle.N_RENGL_V;
                cm.Parameters.Add("@PORC_DTO", SqlDbType.Decimal).Value = detalle.PORC_DTO;
                cm.Parameters.Add("@PORC_IVA", SqlDbType.Decimal).Value = detalle.PORC_IVA;
                cm.Parameters.Add("@PPP_EX", SqlDbType.Decimal).Value = detalle.PPP_EX;
                cm.Parameters.Add("@PPP_LO", SqlDbType.Decimal).Value = detalle.PPP_LO;
                cm.Parameters.Add("@PRECIO_NET", SqlDbType.Decimal).Value = detalle.PRECIO_NET;
                cm.Parameters.Add("@PRECIO_PAN", SqlDbType.Decimal).Value = detalle.PRECIO_PAN;
                cm.Parameters.Add("@PREC_ULC_E", SqlDbType.Decimal).Value = detalle.PREC_ULC_E;
                cm.Parameters.Add("@PREC_ULC_L", SqlDbType.Decimal).Value = detalle.PREC_ULC_L;
                cm.Parameters.Add("@PROMOCION", SqlDbType.Bit).Value = detalle.PROMOCION;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = detalle.T_COMP;
                cm.Parameters.Add("@TCOMP_IN_V", SqlDbType.VarChar).Value = detalle.TCOMP_IN_V;
                cm.Parameters.Add("@COD_CLASIF", SqlDbType.VarChar).Value = detalle.COD_CLASIF;
                cm.Parameters.Add("@IM_NET_P_E", SqlDbType.Decimal).Value = detalle.IM_NET_P_E;
                cm.Parameters.Add("@IM_RE_PA_E", SqlDbType.Decimal).Value = detalle.IM_RE_PA_E;
                cm.Parameters.Add("@PREC_NET_E", SqlDbType.Decimal).Value = detalle.PREC_NET_E;
                cm.Parameters.Add("@PREC_PAN_E", SqlDbType.Decimal).Value = detalle.PREC_PAN_E;
                cm.Parameters.Add("@PR_ULC_E_E", SqlDbType.Decimal).Value = detalle.PR_ULC_E_E;
                cm.Parameters.Add("@PR_ULC_L_E", SqlDbType.Decimal).Value = detalle.PR_ULC_L_E;
                cm.Parameters.Add("@PRECSINDTO", SqlDbType.Decimal).Value = detalle.PRECSINDTO;
                cm.Parameters.Add("@IMPORTE_EXENTO", SqlDbType.Decimal).Value = detalle.IMPORTE_EXENTO;
                cm.Parameters.Add("@IMPORTE_GRAVADO", SqlDbType.Decimal).Value = detalle.IMPORTE_GRAVADO;
                cm.Parameters.Add("@CANTIDAD_2", SqlDbType.Decimal).Value = detalle.CANTIDAD_2;
                cm.Parameters.Add("@FALTAN_REM_2", SqlDbType.Decimal).Value = detalle.FALTAN_REM_2;
                cm.Parameters.Add("@ID_MEDIDA_VENTAS", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@ID_MEDIDA_STOCK_2", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@ID_MEDIDA_STOCK", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@UNIDAD_MEDIDA_SELECCIONADA", SqlDbType.Char).Value = detalle.UNIDAD_MEDIDA_SELECCIONADA;
                cm.Parameters.Add("@RENGL_PADR", SqlDbType.Int).Value = detalle.RENGL_PADR;
                cm.Parameters.Add("@COD_ARTICU_KIT", SqlDbType.VarChar).Value = detalle.COD_ARTICU_KIT;
                cm.Parameters.Add("@INSUMO_KIT_SEPARADO", SqlDbType.Bit).Value = detalle.INSUMO_KIT_SEPARADO;
                cm.Parameters.Add("@PRECIO_FECHA", SqlDbType.DateTime).Value = detalle.PRECIO_FECHA;
                cm.Parameters.Add("@PRECIO_LISTA", SqlDbType.Decimal).Value = detalle.PRECIO_LISTA;
                cm.Parameters.Add("@PRECIO_BONIF", SqlDbType.Decimal).Value = detalle.PRECIO_BONIF;
                cm.Parameters.Add("@PORC_DTO_PARAM", SqlDbType.Decimal).Value = detalle.PORC_DTO_PARAM;
                cm.Parameters.Add("@FECHA_MODIFICACION_PRECIO", SqlDbType.DateTime).Value = detalle.FECHA_MODIFICACION_PRECIO;
                cm.Parameters.Add("@USUARIO_MODIFICACION_PRECIO", SqlDbType.VarChar).Value = DBNull.Value;
                cm.Parameters.Add("@TERMINAL_MODIFICACION_PRECIO", SqlDbType.VarChar).Value = DBNull.Value;
                cm.Parameters.Add("@ITEM_ESPECTACULO", SqlDbType.VarChar).Value = detalle.ITEM_ESPECTACULO;

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public SqlCommand InsertarCuotas(CapaEntidades.CuotasGVA46 cuota)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @" INSERT INTO GVA46
                        (
                        FILLER,
                        ESTADO_VTO,
                        FECHA_VTO,
                        IMPORTE_VT,
                        N_COMP,
                        T_COMP,
                        ESTADO_UNI,
                        IMP_VT_UNI,
                        IMP_VT_EXT,
                        IM_VT_UN_E,
                        ALTERNATIVA_1,
                        IMPORTE_TOTAL_1,
                        ALTERNATIVA_2,
                        IMPORTE_TOTAL_2,
                        AJUSTA_COBRO_FECHA_ALTERNATIVA,
                        UNIDADES_TOTAL_1,
                        UNIDADES_TOTAL_2
                        )   
                        VALUES
                        (
                        @FILLER,
                        @ESTADO_VTO,
                        @FECHA_VTO,
                        @IMPORTE_VT,
                        @N_COMP,
                        @T_COMP,
                        @ESTADO_UNI,
                        @IMP_VT_UNI,
                        @IMP_VT_EXT,
                        @IM_VT_UN_E,
                        @ALTERNATIVA_1,
                        @IMPORTE_TOTAL_1,
                        @ALTERNATIVA_2,
                        @IMPORTE_TOTAL_2,
                        @AJUSTA_COBRO_FECHA_ALTERNATIVA,
                        @UNIDADES_TOTAL_1,
                        @UNIDADES_TOTAL_2
                        )
                    ";

                    cm.Parameters.Clear();
                    cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = cuota.FILLER;
                    cm.Parameters.Add("@ESTADO_VTO", SqlDbType.VarChar).Value = cuota.ESTADO_VTO;
                    cm.Parameters.Add("@FECHA_VTO", SqlDbType.DateTime).Value = cuota.FECHA_VTO;
                    cm.Parameters.Add("@IMPORTE_VT", SqlDbType.Decimal).Value = cuota.IMPORTE_VT;
                    cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = cuota.N_COMP;
                    cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = cuota.T_COMP;
                    cm.Parameters.Add("@ESTADO_UNI", SqlDbType.VarChar).Value = cuota.ESTADO_UNI;
                    cm.Parameters.Add("@IMP_VT_UNI", SqlDbType.Decimal).Value = cuota.IMP_VT_UNI;
                    cm.Parameters.Add("@IMP_VT_EXT", SqlDbType.Decimal).Value = cuota.IMP_VT_EXT;
                    cm.Parameters.Add("@IM_VT_UN_E", SqlDbType.Decimal).Value = cuota.IM_VT_UN_E;
                    cm.Parameters.Add("@ALTERNATIVA_1", SqlDbType.DateTime).Value = cuota.ALTERNATIVA_1;
                    cm.Parameters.Add("@IMPORTE_TOTAL_1", SqlDbType.Decimal).Value = cuota.IMPORTE_TOTAL_1;
                    cm.Parameters.Add("@ALTERNATIVA_2", SqlDbType.DateTime).Value = cuota.ALTERNATIVA_2;
                    cm.Parameters.Add("@IMPORTE_TOTAL_2", SqlDbType.Decimal).Value = cuota.IMPORTE_TOTAL_2;
                    cm.Parameters.Add("@AJUSTA_COBRO_FECHA_ALTERNATIVA", SqlDbType.VarChar).Value = cuota.AJUSTA_COBRO_FECHA_ALTERNATIVA;
                    cm.Parameters.Add("@UNIDADES_TOTAL_1", SqlDbType.Decimal).Value = cuota.UNIDADES_TOTAL_1;
                    cm.Parameters.Add("@UNIDADES_TOTAL_2", SqlDbType.Decimal).Value = cuota.UNIDADES_TOTAL_2;


                    return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public SqlCommand InsertarImpuestos(CapaEntidades.ImpuestosGVA42 impuesto)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @"  INSERT INTO GVA42
                        (
                        FILLER,
                        COD_ALICUO,
                        IMPORTE,
                        N_COMP,
                        NETO_GRAV,
                        PERCEP,
                        PORCENTAJE,
                        T_COMP,
                        COD_IMPUES,
                        COD_SII,
                        IMP_EXT,
                        NE_GRAV_EX,
                        PERCEP_EXT
                        )
                        VALUES
                        (
                        @FILLER,
                        @COD_ALICUO,
                        @IMPORTE,
                        @N_COMP,
                        @NETO_GRAV,
                        @PERCEP,
                        @PORCENTAJE,
                        @T_COMP,
                        @COD_IMPUES,
                        @COD_SII,
                        @IMP_EXT,
                        @NE_GRAV_EX,
                        @PERCEP_EXT
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = impuesto.FILLER;
                cm.Parameters.Add("@COD_ALICUO", SqlDbType.Int).Value = impuesto.COD_ALICUO;
                cm.Parameters.Add("@IMPORTE", SqlDbType.Decimal).Value = impuesto.IMPORTE;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = impuesto.N_COMP;
                cm.Parameters.Add("@NETO_GRAV", SqlDbType.Decimal).Value = impuesto.NETO_GRAV;
                cm.Parameters.Add("@PERCEP", SqlDbType.Decimal).Value = impuesto.PERCEP;
                cm.Parameters.Add("@PORCENTAJE", SqlDbType.Decimal).Value = impuesto.PORCENTAJE;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = impuesto.T_COMP;
                cm.Parameters.Add("@COD_IMPUES", SqlDbType.VarChar).Value = impuesto.COD_IMPUES;
                cm.Parameters.Add("@COD_SII", SqlDbType.VarChar).Value = impuesto.COD_SII;
                cm.Parameters.Add("@IMP_EXT", SqlDbType.Decimal).Value = impuesto.IMP_EXT;
                cm.Parameters.Add("@NE_GRAV_EX", SqlDbType.Decimal).Value = impuesto.NE_GRAV_EX;
                cm.Parameters.Add("@PERCEP_EXT", SqlDbType.Decimal).Value = impuesto.PERCEP_EXT;

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SqlCommand InsertarAsientoComprobante()
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.CommandText = "insert into  asiento_comprobante_gv " +
                    " ( id_asiento_comprobante_gv,ncomp_in_v, " +
                    " asiento_anulacion,contabilizado,usuario_contabilizacion,fecha_contabilizacion,transferido_cn) " +
                    " select (SELECT MAX(id_asiento_comprobante_gv) from " +
                    " ASIENTO_COMPROBANTE_GV) + ROW_NUMBER() over (order by  id_gva12),NCOMP_IN_V ,'N','N',NULL,NULL,'N' " +
                    " from gva12 " +
                    " where t_comp<>'REC' and ncomp_in_v not in (select ncomp_in_v from asiento_comprobante_gv) AND ESTADO<>'ANU'";
                return cm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Reset_ID()
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());
                SqlCommand cm = new SqlCommand();
                cn.Open();
                cm.CommandText = "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA12')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA12'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA42')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA42'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA53')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA53'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA46')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA46'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'ASIENTO_COMPROBANTE_GV')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'ASIENTO_COMPROBANTE_GV'";

                cm.Connection = cn;
                cm.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public void Reset_ID()
        //{
        //    try
        //    {
        //        SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());
        //        SqlCommand cm = new SqlCommand();
        //        cn.Open();
        //        cm.CommandText = @"DECLARE @TABLA VARCHAR(MAX), @SQL VARCHAR(MAX)
        //                        DECLARE RS CURSOR LOCAL FOR
        //                        SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME LIKE 'ID[_]%'
        //                        OPEN RS
        //                        FETCH NEXT FROM RS INTO @TABLA
        //                        WHILE @@FETCH_STATUS = 0
        //                        BEGIN
        //                        SET @SQL = ' IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = ''SEQUENCE_' + @TABLA + ''')EXECUTE P_UPDATESEQUENCEBYTABLENAME ' + @TABLA + ''
        //                        EXEC(@SQL)
        //                        FETCH NEXT FROM RS INTO @TABLA
        //                        END
        //                        CLOSE RS
        //                        DEALLOCATE RS
        //        ";

        //        cm.Connection = cn;

        //        cm.ExecuteNonQuery();
        //        cn.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
    }
}
