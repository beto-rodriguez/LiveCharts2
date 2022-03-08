using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Test.MotionCanvasDispose;

namespace EtoFormsSample.Test.MotionCanvasDispose;

public class View : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var button = new Button() { Text = "Change content", Height = 40 };

        button.Click += (o, e) => { Content = createLayout(button); };

        Content = createLayout(button);
    }

    private static Control createLayout(Button button)
    {
        var canvas = new MotionCanvas();

        ViewModel.Generate(canvas.CanvasCore);

        return new DynamicLayout(
            new DynamicRow(new DynamicControl()
            {
                Control = button,
                XScale = true
            }),
            canvas
            );
    }
}
