// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace MauiSample;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AppShell : Shell
{
    private bool _isLoaded = false;
    private readonly Dictionary<string, string> _routesSamples = new();

    public AppShell()
    {
        InitializeComponent();
        PropertyChanged += AppShell_PropertyChanged;
    }

    private void AppShell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_isLoaded) return;
        _isLoaded = true;

        var samples = ViewModelsSamples.Index.Samples;

        var i = 0;
        for (var i1 = 0; i1 < samples.Length; i1++)
        {
            var item = samples[i1];
            var t = Type.GetType($"MauiSample.{item.Replace('/', '.')}.View");
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
            catch (Exception)
            {
                throw;
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
        var t = Type.GetType($"MauiSample.{item.Replace('/', '.')}.View");
        var i = Activator.CreateInstance(t);
        var c = next.Items[0].Items[0];
        c.Content = i;
    }

    // UnComment the below method to handle Shell Menu item click event
    // And ensure appropriate page definitions are available for it work as expected
    /*
    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//login");
    }
    */
}
