using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace XamarinSample;

public partial class AppShell : Shell
{
    private bool _isLoaded = false;
    private readonly Dictionary<string, string> _routesSamples = new();

    public AppShell()
    {
        InitializeComponent();
        SizeChanged += AppShell_SizeChanged;
    }

    private void AppShell_SizeChanged(object sender, EventArgs e)
    {
        if (_isLoaded) return;
        _isLoaded = true;

        var samples = ViewModelsSamples.Index.Samples;

        var i = 0;
        foreach (var item in samples)
        {
            var t = Type.GetType($"XamarinSample.{item.Replace('/', '.')}.View");
            //var i = Activator.CreateInstance(t);
            Routing.RegisterRoute(item, t);

            var shell_section = new ShellSection { Title = item };

            ShellContent content;

            try
            {
                content = new ShellContent()
                {
                    Content = i == 0 ? Activator.CreateInstance(t) : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            shell_section.Items.Add(content);

            Items.Add(shell_section);
            _routesSamples.Add("//" + content.Route, item);
            i++;

            //if (i > 4) break;
        }

        Navigating += AppShell_Navigating;
    }

    private void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
    {
        var shell = (AppShell)sender;
        var r = shell.Items.Select(x => x.CurrentItem.CurrentItem.Route).ToArray();
        var next = Items.FirstOrDefault(x => "//" + x.CurrentItem.CurrentItem.Route == e.Target.Location.OriginalString);

        var item = _routesSamples[e.Target.Location.OriginalString];
        var t = Type.GetType($"XamarinSample.{item.Replace('/', '.')}.View");

        object i;

        try
        {
            i = Activator.CreateInstance(t);
        }
        catch (Exception ex)
        {
            var a = 1;
            throw;
        }

        var c = next.Items[0].Items[0];
        c.Content = i;
    }

    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//LoginPage");
    }
}
