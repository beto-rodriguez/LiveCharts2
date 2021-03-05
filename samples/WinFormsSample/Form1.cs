using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModelsSamples;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var vm = new CartesianViewModel();

            cartesianChart1.Series = vm.Series;
            cartesianChart1.XAxes = vm.XAxes;
            cartesianChart1.YAxes = vm.YAxes;
        }
    }
}
