using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UnoSample.Test.Dispose;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        // the event is not attached using xaml?
        button.AddHandler(PointerPressedEvent, new PointerEventHandler(Button_PointerPressed2), true);
    }

    private void Button_PointerPressed2(object sender, PointerRoutedEventArgs e)
    {
        content.Content = new UserControl1();
    }
}
