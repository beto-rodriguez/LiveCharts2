using System;
using System.Windows.Forms;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        private UserControl activeControl = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var item in ViewModelsSamples.Index.Samples)
            {
                listBox1.Items.Add(item);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeControl != null)
            {
                Controls.Remove(activeControl);
                activeControl.Dispose();
            }

            var selected = listBox1.SelectedItem.ToString();
            activeControl = (UserControl) Activator.CreateInstance(null, $"WinFormsSample.{selected.Replace('/', '.')}.View").Unwrap();

            var padding = 8;
            activeControl.Location = new System.Drawing.Point(listBox1.Width + padding, padding);
            activeControl.Width = Width - listBox1.Width;
            activeControl.Height = Height;
            activeControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(activeControl);
        }
    }
}
