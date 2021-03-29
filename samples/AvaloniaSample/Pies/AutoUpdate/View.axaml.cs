using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using ViewModelsSamples.Pies.AutoUpdate;

namespace AvaloniaSample.Pies.AutoUpdate
{
    public class View : UserControl
    {
        private bool? isStreaming = false;

        public View()
        {
            InitializeComponent();
        }

        private async void ButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var vm = (ViewModel?)DataContext;
            if (vm == null) return;

            isStreaming = isStreaming == null ? true : !isStreaming;

            while (isStreaming.Value)
            {
                vm.RemoveLastSeries();
                vm.AddSeries();
                await Task.Delay(1000);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
