using System;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsSample;

public class Form1 : Form
{
    private readonly Scrollable _scroll = new Scrollable();
    private readonly DynamicLayout _layout = new DynamicLayout();

    public Form1()
    {
        Title = $"Eto.Forms Sample ({Eto.Platform.Instance} / {System.Runtime.InteropServices.RuntimeInformation.OSDescription})";

        Size = new Size(800, 600);

        var listbox = new ListBox();
        foreach (var item in ViewModelsSamples.Index.Samples)
        {
            listbox.Items.Add(item);
        }

        _scroll = new Scrollable() { Content = listbox };
        listbox.MouseWheel += (o, e) => { _scroll.ScrollPosition -= new Point(0, (int)e.Delta.Height * 48); };

        _layout.AddRow(_scroll, new Panel());
        Content = _layout;

        listbox.SelectedIndexChanged += listBox1_SelectedIndexChanged;
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selected = (sender as ListBox).SelectedKey;
        var chart = (Control)Activator.CreateInstance(null, $"EtoFormsSample.{selected.Replace('/', '.')}.View").Unwrap();

        var layout = new DynamicLayout();
        layout.AddRow(_scroll, chart);
        Content = layout;
    }
}
