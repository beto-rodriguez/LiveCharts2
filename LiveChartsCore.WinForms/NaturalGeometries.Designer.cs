
using SkiaSharp.Views.Desktop;

namespace LiveChartsCore.WinForms
{
    partial class NaturalGeometries
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.skControl1 = new SKControl();
            this.SuspendLayout();
            // 
            // skControl1
            // 
            this.skControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skControl1.Location = new System.Drawing.Point(0, 0);
            this.skControl1.Name = "skControl1";
            this.skControl1.Size = new System.Drawing.Size(150, 150);
            this.skControl1.TabIndex = 0;
            this.skControl1.Text = "skControl1";
            this.skControl1.PaintSurface += new System.EventHandler<SKPaintSurfaceEventArgs>(this.SkiaElement_PaintSurface);
            // 
            // NaturalGeometries
            // 
            this.Controls.Add(this.skControl1);
            this.Name = "NaturalGeometries";
            this.ResumeLayout(false);

        }

        #endregion

        private SKControl skControl1;
    }
}
