
namespace LiveChartsCore.SkiaSharpView.WinForms
{
    partial class CartesianChart
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
            this.motionCanvas = new LiveChartsCore.SkiaSharpView.WinForms.MotionCanvas();
            this.SuspendLayout();
            // 
            // motionCanvas
            // 
            this.motionCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.motionCanvas.FramesPerSecond = 90D;
            this.motionCanvas.Location = new System.Drawing.Point(0, 0);
            this.motionCanvas.Name = "motionCanvas";
            this.motionCanvas.Size = new System.Drawing.Size(150, 150);
            this.motionCanvas.TabIndex = 0;
            this.motionCanvas.Load += new System.EventHandler(this.OnLoaded);
            this.motionCanvas.Resize += new System.EventHandler(this.OnResized);
            // 
            // CartesianChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.motionCanvas);
            this.Name = "CartesianChart";
            this.ResumeLayout(false);

        }

        #endregion

        private MotionCanvas motionCanvas;
    }
}
