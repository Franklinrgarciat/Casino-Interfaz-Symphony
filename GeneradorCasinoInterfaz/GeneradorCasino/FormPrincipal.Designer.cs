
namespace GeneradorCasino
{
    partial class FormPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipal));
            this.txt_ruta = new System.Windows.Forms.TextBox();
            this.gb_importador = new System.Windows.Forms.GroupBox();
            this.btn_buscar = new System.Windows.Forms.Button();
            this.btn_exportar = new System.Windows.Forms.Button();
            this.gb_importador.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_ruta
            // 
            this.txt_ruta.Enabled = false;
            this.txt_ruta.Location = new System.Drawing.Point(33, 43);
            this.txt_ruta.Name = "txt_ruta";
            this.txt_ruta.Size = new System.Drawing.Size(261, 20);
            this.txt_ruta.TabIndex = 0;
            // 
            // gb_importador
            // 
            this.gb_importador.Controls.Add(this.txt_ruta);
            this.gb_importador.Controls.Add(this.btn_buscar);
            this.gb_importador.Location = new System.Drawing.Point(23, 23);
            this.gb_importador.Name = "gb_importador";
            this.gb_importador.Size = new System.Drawing.Size(445, 100);
            this.gb_importador.TabIndex = 3;
            this.gb_importador.TabStop = false;
            this.gb_importador.Text = "Importa el archivo para continuar";
            // 
            // btn_buscar
            // 
            this.btn_buscar.BackColor = System.Drawing.Color.White;
            this.btn_buscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_buscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_buscar.Image = global::GeneradorCasino.Properties.Resources.Busca;
            this.btn_buscar.Location = new System.Drawing.Point(343, 35);
            this.btn_buscar.Name = "btn_buscar";
            this.btn_buscar.Size = new System.Drawing.Size(58, 35);
            this.btn_buscar.TabIndex = 1;
            this.btn_buscar.UseVisualStyleBackColor = false;
            this.btn_buscar.Click += new System.EventHandler(this.btn_buscar_Click);
            // 
            // btn_exportar
            // 
            this.btn_exportar.BackColor = System.Drawing.Color.White;
            this.btn_exportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_exportar.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btn_exportar.FlatAppearance.BorderSize = 2;
            this.btn_exportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_exportar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_exportar.Image = global::GeneradorCasino.Properties.Resources.iconfinder_floppy_285657;
            this.btn_exportar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_exportar.Location = new System.Drawing.Point(175, 147);
            this.btn_exportar.Name = "btn_exportar";
            this.btn_exportar.Size = new System.Drawing.Size(152, 29);
            this.btn_exportar.TabIndex = 4;
            this.btn_exportar.Text = "Exportar a Tango";
            this.btn_exportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_exportar.UseVisualStyleBackColor = false;
            this.btn_exportar.Click += new System.EventHandler(this.btn_exportar_Click);
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(498, 197);
            this.Controls.Add(this.btn_exportar);
            this.Controls.Add(this.gb_importador);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importador Casino";
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this.gb_importador.ResumeLayout(false);
            this.gb_importador.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txt_ruta;
        private System.Windows.Forms.Button btn_buscar;
        private System.Windows.Forms.GroupBox gb_importador;
        private System.Windows.Forms.Button btn_exportar;
    }
}

