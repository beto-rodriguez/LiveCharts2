namespace LiveChartsCore.WinForms
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
            this.naturalGeometries1 = new LiveChartsCore.WinForms.NaturalGeometries();
            this.SuspendLayout();
            // 
            // naturalGeometries1
            // 
            this.naturalGeometries1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.naturalGeometries1.Location = new System.Drawing.Point(0, 0);
            this.naturalGeometries1.Name = "naturalGeometries1";
            this.naturalGeometries1.Size = new System.Drawing.Size(800, 450);
            this.naturalGeometries1.TabIndex = 0;
            // 
            // CartesianChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.naturalGeometries1);
            this.Name = "CartesianChart";
            this.Size = new System.Drawing.Size(800, 450);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.Resize += new System.EventHandler(this.CartesianChart_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private NaturalGeometries naturalGeometries1;
    }
}
