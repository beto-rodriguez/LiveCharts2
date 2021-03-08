using System.Timers;
using System.Windows;
using ViewModelsSamples;

namespace WPFSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CartesianViewModel viewModel;

       public MainWindow()
        {
            InitializeComponent();

            var c = DataContext as CartesianViewModel;
            if (c == null) return;

            viewModel = c;

            var t = new Timer();
            t.Interval = 1000;
            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                viewModel?.Randomize();
            });
        }
    }
}
