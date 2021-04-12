using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace XamarinSample
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        private bool isLoaded = false;
        private Dictionary<string, string> routesSamples = new Dictionary<string, string>();

        public AppShell()
        {
            InitializeComponent();
            SizeChanged += AppShell_SizeChanged;
        }

        private void AppShell_SizeChanged(object sender, EventArgs e)
        {
            if (isLoaded) return;
            isLoaded = true;

            var samples = ViewModelsSamples.Index.Samples;

            var i = 0;
            foreach (var item in samples)
            {
                var t = Type.GetType($"XamarinSample.{item.Replace('/', '.')}.View");
                //var i = Activator.CreateInstance(t);
                Routing.RegisterRoute(item, t);

                var shell_section = new ShellSection { Title = item };

                var content = new ShellContent() 
                { 
                    Content = i == 0 ? Activator.CreateInstance(t) : null
                };
                shell_section.Items.Add(content);

                Items.Add(shell_section);
                routesSamples.Add("//" + content.Route, item);
                i++;
            }

            Navigating += AppShell_Navigating;
        }

        private void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            var shell = (AppShell)sender;
            var r = shell.Items.Select(x => x.CurrentItem.CurrentItem.Route).ToArray();
            var next = Items.FirstOrDefault(x => "//" + x.CurrentItem.CurrentItem.Route == e.Target.Location.OriginalString);

            var item = routesSamples[e.Target.Location.OriginalString];
            var t = Type.GetType($"XamarinSample.{item.Replace('/', '.')}.View");
            var i = Activator.CreateInstance(t);
            var c = next.Items[0].Items[0];
            c.Content = i;
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
