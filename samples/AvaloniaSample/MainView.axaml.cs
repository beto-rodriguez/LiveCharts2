using Avalonia.Controls;

namespace AvaloniaSample;
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }

}
