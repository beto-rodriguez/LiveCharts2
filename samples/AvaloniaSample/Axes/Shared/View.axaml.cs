using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.Axes.Shared
{
    public class View : UserControl
    {
        public View()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
