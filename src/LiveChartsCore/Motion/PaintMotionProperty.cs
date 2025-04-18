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

using LiveChartsCore.Painting;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the float motion property class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PaintMotionProperty"/> class.
/// </remarks>
/// <param name="defaultValue">The default value.</param>
public class PaintMotionProperty(Paint defaultValue = null!)
    : MotionProperty<Paint>(defaultValue)
{
    internal static Paint? s_activePaint;
    private bool _isFromDefault;
    private bool _isToDefault;

    /// <inheritdoc cref="MotionProperty{T}.CanTransitionate"/>
    protected override bool CanTransitionate =>
        (FromValue ?? s_activePaint) is not null &&
        (ToValue ?? s_activePaint) is not null;

    // how do paints transitionate?

    //    - each property in the paint is interpolated according to the animation timeline.

    //    - there is a special case, when the paint is null, the library will use the s_activePaint
    //      as the default paint, this is to prevent initializing a new paint for every geometry
    //      that is drawn, this is useful because normally series contain a lot of geometries that
    //      are drawn with the same paint, so we can just use the same paint for all of them.
    //      The s_activePaint is changing as the library is drawing, finally is set to null when the
    //      frame is completed.

    //    - GetFromOrDefault and GetToOrDefault are used to evaluate if the paint is null,
    //      when it is, we clone the s_activePaint and set it to the FromValue or ToValue,
    //      a clone only knows how to replicate the same skia paint, but it does not clone any reference
    //      inside the paint instance, this should help to prevent memory leaks, but also when the
    //      animation is completed, we set the FromValue and ToValue to null to release those clones from memory.

    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
    protected override Paint OnGetMovement(float progress) =>
        GetFromOrDefault().Transitionate(progress, GetToOrDefault());

    /// <inheritdoc cref="MotionProperty{T}.OnCompleted" />
    protected override void OnCompleted()
    {
        if (_isFromDefault)
        {
            FromValue = null!;
            _isFromDefault = false;
        }

        if (_isToDefault)
        {
            ToValue = null!;
            _isToDefault = false;
        }
    }

    private Paint GetFromOrDefault()
    {
        if (FromValue is not null)
        {
            FromValue.ResolveActiveColor(s_activePaint);
            return FromValue;
        }

        _isFromDefault = true;
        FromValue = s_activePaint!.CloneTask(); // ! canot be null here because of the check in CanTransitionate

        return FromValue;
    }

    private Paint GetToOrDefault()
    {
        if (ToValue is not null)
        {
            ToValue.ResolveActiveColor(s_activePaint);
            return ToValue;
        }

        _isToDefault = true;
        ToValue = s_activePaint!.CloneTask(); // ! canot be null here because of the check in CanTransitionate

        return ToValue;
    }
}
