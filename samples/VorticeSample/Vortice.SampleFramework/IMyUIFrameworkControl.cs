// just an interface to represent a control in our direct 2d framework

using Vortice.Direct2D1;

namespace Vortice;

public interface IMyUIFrameworkControl
{
    IMyUIFrameworkControl[] Children { get; set; }
    void DrawFrame(ID2D1HwndRenderTarget renderTarget);
    void Measure();
}
