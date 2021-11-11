using Application = Microsoft.Maui.Controls.Application;

namespace MauiSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
