using System;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsSample;

public class Form1 : Form
{
    private readonly ListBox _listbox = new();
    private readonly DynamicLayout _layout = new();

    public Form1()
    {
        Title = $"Eto.Forms Sample ({Eto.Platform.Instance} / {System.Runtime.InteropServices.RuntimeInformation.OSDescription})";

        Size = new Size(800, 600);

        foreach (var item in ViewModelsSamples.Index.Samples)
        {
            _listbox.Items.Add(item);
        }

        _ = _layout.AddRow(_listbox, new Panel());
        Content = _layout;

        _listbox.SelectedIndexChanged += listBox1_SelectedIndexChanged;
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selected = (sender as ListBox).SelectedKey;
        var chart = (Control)Activator.CreateInstance(null, $"EtoFormsSample.{selected.Replace('/', '.')}.View").Unwrap();

        var layout = new DynamicLayout();
        _ = layout.AddRow(_listbox, chart);
        Content = layout;
    }
}
