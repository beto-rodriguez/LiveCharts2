using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using ViewModelsSamples.Lines.AutoUpdate;

namespace AvaloniaSample.Lines.AutoUpdate
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
            if (vm is null) return;

            isStreaming = isStreaming is null ? true : !isStreaming;

            while (isStreaming.Value)
            {
                vm.RemoveFirstItem();
                vm.AddItem();
                await Task.Delay(100);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
