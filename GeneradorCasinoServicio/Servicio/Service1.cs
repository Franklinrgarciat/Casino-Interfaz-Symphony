using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace Servicio
{
    partial class Service1 : ServiceBase
    {
        System.Timers.Timer timer;
        int Tiempo = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MINUTOS").ToString());
        public Service1()
        {
            InitializeComponent();
            timer = new System.Timers.Timer(Tiempo * 10000);

            timer.Elapsed += new ElapsedEventHandler(Ejecutar);
        }

        private void Ejecutar(object sender, ElapsedEventArgs e)
        {
            //if (9 <= Convert.ToInt32(DateTime.Now.ToString("HH")) && Convert.ToInt32(DateTime.Now.ToString("HH")) <= 10)
            //{
            timer.Stop();
            try
            {
                string ruta = ConfigurationManager.AppSettings.Get("RUTA_DE_ARCHIVOS");
                string rutaCompPasados = ConfigurationManager.AppSettings.Get("RUTA_DE_ARCHIVOS_PASADOS");
                foreach (var item in Directory.GetFiles(ruta))
                {
                    CapaNegocios.NegociosCasino negocios = new CapaNegocios.NegociosCasino();
                    negocios.GuardarEnTango(item);
                    int lastIndex = item.LastIndexOf(@"\");
                    string nombreArchivo = item.Substring(lastIndex);
                    File.Move(item, rutaCompPasados + nombreArchivo);
                }
            }
            catch (Exception ex)
            {
                LogDeErrores(ex);
                throw ex;
            }
            timer.Start();
            //}

        }

        protected override void OnStart(string[] args)
        {
            // TODO: agregar código aquí para iniciar el servicio.
            System.Diagnostics.Debugger.Launch();
            timer.Start();
        }

        protected override void OnStop()
        {
            // TODO: agregar código aquí para realizar cualquier anulación necesaria para detener el servicio.
        }
        public void LogDeErrores(Exception ex)
        {
            using (StreamWriter LogError = new StreamWriter(Application.StartupPath + "\\Errores.log", true))
            {
                LogError.WriteLine(DateTime.Now.ToString("dd-MM-yyy HH:mm") + " - " + ex.Message);
            }
        }
    }
}
