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

using LiveChartsCore.Motion;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a drawable object, an object that can be represented in the user interface.
/// </summary>
public interface IDrawnElement
{
    /// <summary>
    /// Gets the parent shape, if any, the <see cref="X"/> and <see cref="Y"/>
    /// coordinates will be relative to the parent.
    /// </summary>
    IDrawnElement? Parent { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate, if the parent is not null the x coordinate will be relative to the parent.
    /// </summary>
    float X { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate, if the parent is not null the y coordinate will be relative to the parent.
    /// </summary>
    float Y { get; set; }

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    float Opacity { get; set; }

    /// <summary>
    /// Gets or sets the transform origin.
    /// </summary>
    LvcPoint TransformOrigin { get; set; }

    /// <summary>
    /// Gets or sets the translate transform.
    /// </summary>
    LvcPoint TranslateTransform { get; set; }

    /// <summary>
    /// Gets or sets the rotation transform in degrees.
    /// </summary>
    float RotateTransform { get; set; }

    /// <summary>
    /// Gets or sets the scale transform.
    /// </summary>
    LvcPoint ScaleTransform { get; set; }

    /// <summary>
    /// Gets or sets the skew transform.
    /// </summary>
    LvcPoint SkewTransform { get; set; }

    /// <summary>
    /// Gets or sets the drop shadow.
    /// </summary>
    LvcDropShadow? DropShadow { get; set; }

    /// <summary>
    /// Gets or sets the draw effect, this property is used to apply effects like gradients and...
    /// ToDo only gradients for now.
    /// </summary>
    DrawEffect? DrawEffect { get; set; }

    /// <summary>
    /// Gets or sets the clipping bounds.
    /// </summary>
    LvcRectangle ClippingBounds { get; set; }

    /// <summary>
    /// Gets a value indicating whether the instance has transform.
    /// </summary>
    bool HasTransform { get; }

    /// <summary>
    /// Gets a value indicating whether the instance has translation.
    /// </summary>
    bool HasTranslate { get; }

    /// <summary>
    /// Gets a value indicating whether the instance has translation.
    /// </summary>
    bool HasScale { get; }

    /// <summary>
    /// Gets a value indicating whether the instance has skew.
    /// </summary>
    bool HasSkew { get; }

    /// <summary>
    /// Gets a value indicating whether the instance has rotation.
    /// </summary>
    bool HasRotation { get; }

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    Paint? Stroke { get; set; }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    Paint? Fill { get; set; }

    /// <summary>
    /// Gets or sets the paint, this property is different from <see cref="Fill"/> and <see cref="Stroke"/>
    /// because it does not set a <see cref="PaintStyle"/>, it is normally used to draw labels.
    /// </summary>
    Paint? Paint { get; set; }

    /// <summary>
    /// Gets or sets the Stroke thickness.
    /// </summary>
    double StrokeThickness { get; set; }

    /// <summary>
    /// Gets a value indicating whether the instance is valid.
    /// </summary>
    bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the instance should be removed from the canvas when all the animations are completed.
    /// </summary>
    bool RemoveOnCompleted { get; set; }

    /// <summary>
    /// Sets the transition for the specified properties.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="properties">The properties, null to select all properties.</param>
    void SetTransition(Animation? animation, params PropertyDefinition[]? properties);

    /// <summary>
    /// Removes the transition for the specified properties.
    /// </summary>
    /// <param name="properties">The properties to remove, null to select all properties.</param>
    void RemoveTransition(params PropertyDefinition[]? properties);

    /// <summary>
    /// Completes the transition for the specified properties.
    /// </summary>
    /// <param name="properties">The properties to complete, null to select all properties.</param>
    void CompleteTransition(params PropertyDefinition[]? properties);

    /// <summary>
    /// Measures the instance.
    /// </summary>
    /// <returns>The size.</returns>
    LvcSize Measure();

    /// <summary>
    /// Disposes the paints holded by the instance.
    /// </summary>
    void DisposePaints();
}

/// <summary>
/// Defines a drawable object, an object that can be represented in the user interface.
/// </summary>
/// <typeparam name="TDrawingContext"></typeparam>
public interface IDrawnElement<TDrawingContext> : IDrawnElement
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Draws the instance in the user interface with for the specified context.
    /// </summary>
    /// <param name="context"></param>
    void Draw(TDrawingContext context);
}
