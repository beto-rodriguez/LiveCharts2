
namespace LiveChartsCore.SkiaSharpView.WinForms
{
    partial class MotionCanvas
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
            this.skControl2 = new SkiaSharp.Views.Desktop.SKControl();
            this.SuspendLayout();
            // 
            // skControl2
            // 
            this.skControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skControl2.Location = new System.Drawing.Point(0, 0);
            this.skControl2.Name = "skControl2";
            this.skControl2.Size = new System.Drawing.Size(150, 150);
            this.skControl2.TabIndex = 1;
            this.skControl2.Text = "skControl2";
            this.skControl2.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs>(this.SkControl_PaintSurface);
            // 
            // MotionCanvas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.skControl2);
            this.Name = "MotionCanvas";
            this.ResumeLayout(false);
        }

        #endregion

        private SkiaSharp.Views.Desktop.SKControl skControl2;
    }
}
