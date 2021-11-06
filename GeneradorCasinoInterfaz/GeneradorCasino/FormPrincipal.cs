using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneradorCasino
{
    public partial class FormPrincipal : Form
    {
        private string seleccioneRuta = "Seleccione un archivo...";
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            btn_exportar.Enabled = false;
            txt_ruta.Text = seleccioneRuta;
            btn_buscar.Focus();
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog Archivo = new OpenFileDialog();
                Archivo.Filter = "Archivo Files | *.xls;*.xlxs;*.xlxm;*.csv;";
                Archivo.Title = "Cargar Archivo";
                if (Archivo.ShowDialog() == DialogResult.OK)
                {
                    txt_ruta.Text = Archivo.FileName;
                    btn_exportar.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Error al cargar el archivo!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex )
            {
                throw ex;
            }
        }

        private void btn_exportar_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(txt_ruta.Text))
                {
                    bool guardoEnTango;
                    CapaNegocios.NegociosCasino negocios = new CapaNegocios.NegociosCasino();
                    guardoEnTango = negocios.GuardarEnTango(txt_ruta.Text);
                    if (guardoEnTango)
                    {
                        MessageBox.Show("El registro se guardó correctamente!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txt_ruta.Text = seleccioneRuta;
                    }
                    else
                    {
                        MessageBox.Show("Error al guardar los registros", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_ruta.Text = seleccioneRuta;
                    }
                }
                else
                {
                    MessageBox.Show("ERROR!! El archivo seleccionado no existe", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogDeErrores(ex);
                throw;
            }
            
        }
        public void LogDeErrores(Exception ex)
        {
            //Application.StartupPath + "\\Errores.log"
            using (StreamWriter LogError = new StreamWriter(Application.StartupPath + "\\Errores\\Errores.log", true))
            {
                LogError.WriteLine(DateTime.Now.ToString("dd-MM-yyy HH:mm") + " - " + ex.Message);
            }
        }

    }
}
