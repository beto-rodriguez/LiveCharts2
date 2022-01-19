using System;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsSample;

public class Form1 : Form
{
    private readonly ListBox listBox1 = new ListBox();
    private Control activeControl = new Panel();
    private readonly PixelLayout layout = new PixelLayout();

    public Form1()
    {
        Title = "Eto.Forms Sample";

        Size = new Size(800, 600);

        var scroll = new Scrollable() { Content = listBox1 };
        listBox1.MouseWheel += (o, e) => { scroll.ScrollPosition -= new Point(0, (int)e.Delta.Height * 48); };

        layout.Add(scroll, Point.Empty);
        layout.Add(activeControl, new Point(scroll.Width, 0));

        Content = layout;

        foreach (var item in ViewModelsSamples.Index.Samples)
        {
            listBox1.Items.Add(item);
        }
        listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        layout.Remove(activeControl);
        activeControl.Dispose();

        var selected = listBox1.SelectedKey;
        activeControl = (Control)Activator.CreateInstance(null, $"EtoFormsSample.{selected.Replace('/', '.')}.View").Unwrap();

        layout.Add(activeControl, listBox1.Width, 0);
    }
}
