// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Segments;

/// <inheritdoc cref="IMoveToPathCommand{TPath}" />
public class MoveToPathCommand : PathCommand, IMoveToPathCommand<SKPath>
{
    private readonly FloatMotionProperty _xProperty;
    private readonly FloatMotionProperty _yProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveToPathCommand"/> class.
    /// </summary>
    public MoveToPathCommand()
    {
        _xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0f));
        _yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0f));
    }

    /// <inheritdoc cref="IMoveToPathCommand{TPath}.X" />
    public float X { get => _xProperty.GetMovement(this); set => _xProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IMoveToPathCommand{TPath}.Y" />
    public float Y { get => _yProperty.GetMovement(this); set => _yProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IPathCommand{TPathContext}.Execute(TPathContext, long, Animatable)" />
    public override void Execute(SKPath path, long currentTime, Animatable pathGeometry)
    {
        currentTime = currentTime;
        path.MoveTo(X, Y);
    }
}
