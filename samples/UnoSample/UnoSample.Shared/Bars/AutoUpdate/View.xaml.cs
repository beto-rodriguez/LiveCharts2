﻿using System.Threading.Tasks;
using ViewModelsSamples.Bars.AutoUpdate;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UnoSample.Bars.AutoUpdate
{
    public sealed partial class View : UserControl
    {
        private bool? _isStreaming = false;

        public View()
        {
            InitializeComponent();
        }

        private async void Button_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var vm = (ViewModel)DataContext;
            _isStreaming = _isStreaming is null ? true : !_isStreaming;

            while (_isStreaming.Value)
            {
                vm.RemoveFirstItem();
                vm.AddRandomItem();
                await Task.Delay(1000);
            }
        }
    }
}
