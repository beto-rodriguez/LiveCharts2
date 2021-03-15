using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private double pageWidth;
        private double pageHeight;
        private bool isMenuVisible = true;
        private double menuWidth = 300;
        private double menuLeft = 300;
        private double contentLeft;

        public MainPage()
        {
            InitializeComponent();

            Samples = ViewModelsSamples.Index.Samples;
            DataContext = this;
        }
         public string[] Samples { get; set; }

        public bool IsMenuVisible { get => isMenuVisible; set { isMenuVisible = value; OnPropertyChanged(); OnMenuVisibilityChanged(); } }
        public double PageWidth { get => pageWidth; set { pageWidth = value; OnPropertyChanged(); } }
        public double PageHeight { get => pageHeight; set { pageHeight = value; OnPropertyChanged(); } }
        public double MenuWidth { get => menuWidth; set { menuWidth = value; OnPropertyChanged(); } }
        public double ContentWidth => IsMenuVisible ? ActualWidth - menuWidth : ActualWidth;
        public double MenuLeft { get => menuLeft; set { menuLeft = value; OnPropertyChanged(); } }
        public double ContentLeft { get => contentLeft; set { contentLeft = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void page_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs args)
        {
            PageWidth = ActualWidth;
            PageHeight = ActualHeight;
            ContentLeft = IsMenuVisible ? 300 : 0;
            MenuLeft = IsMenuVisible ? 0 : -menuWidth;
            OnPropertyChanged(nameof(ContentWidth));
        }

        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            IsMenuVisible = !IsMenuVisible;
        }

        private void OnItemPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var ctx = (sender as FrameworkElement).DataContext as string;
            if (ctx == null) throw new Exception("Sample not found");

            var t = Type.GetType($"UnoSample.{ctx.Replace('/', '.')}.View");
            var i = (UserControl) Activator.CreateInstance(t);
            i.Width = content.ActualWidth;
            i.Height = content.ActualHeight;
            content.Content = i;

            if (ActualWidth > 900) return;

            IsMenuVisible = false;
        }

        private void OnMenuVisibilityChanged()
        {
            MenuLeft = IsMenuVisible ? 0 : -menuWidth;
            ContentLeft = IsMenuVisible ? 300 : 0;
            OnPropertyChanged(nameof(ContentWidth));
        }

        private void OnPropertyChanged([CallerMemberName] string porpertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(porpertyName));
        }
    }
}
