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

namespace LiveChartsCore.Motion;

/// <summary>
/// The <see cref="MotionProperty{T}"/> object tracks where a property of a <see cref="Animatable"/> is in a time line.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="MotionProperty{T}"/> class.
/// </remarks>
/// <param name="defaultValue">The default value.</param>
public abstract class MotionProperty<T>(T defaultValue) : IMotionProperty
{
    private float _startTime;
    private float _endTime;
    private T _savedValue = defaultValue;

    /// <summary>
    /// Gets the value where the transition began.
    /// </summary>
    public T FromValue { get; private set; } = defaultValue;

    /// <summary>
    /// Gets the value where the transition finished or will finish.
    /// </summary>
    public T ToValue { get; private set; } = defaultValue;

    /// <inheritdoc cref="IMotionProperty.Animation"/>
    public Animation? Animation { get; set; }

    /// <inheritdoc cref="IMotionProperty.IsCompleted"/>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Indicates whether the property transition can complete, this depends
    /// on <see cref="FromValue"/> and <see cref="ToValue"/> being null or any other
    /// state where the transition is not possible.
    /// </summary>
    protected abstract bool CanTransitionate { get; }

    /// <summary>
    /// Moves to the specified value.
    /// </summary>
    /// <param name="value">The value to move to.</param>
    /// <param name="animatable">The <see cref="Animatable"/> instance that is moving.</param>
    public void SetMovement(T value, Animatable animatable)
    {
        FromValue = GetMovement(animatable);
        ToValue = value;

        var animation = GetActualAnimation(animatable);
        if (animation is not null) SetTimeLine(animatable, animation);

        animatable.IsValid = false;
    }

    /// <summary>
    /// Gets the current movement in the <see cref="Animation"/>.
    /// </summary>
    /// <param name="animatable"></param>
    /// <returns></returns>
    public T GetMovement(Animatable animatable)
    {
        var animation = GetActualAnimation(animatable);
        if (animation is null || animation.EasingFunction is null || !CanTransitionate || IsCompleted) return ToValue;

        // at this points we are sure that the animatable has not finished at least with this property.
        animatable.IsValid = false;

        var p = (animatable.CurrentTime - _startTime) / (_endTime - _startTime);

        if (p >= 1)
        {
            // at this point the animation is completed
            p = 1;
            animation._animationCompletedCount++;
            IsCompleted = animation._repeatTimes != int.MaxValue && animation._repeatTimes < animation._animationCompletedCount;
            if (!IsCompleted)
            {
                _startTime = animatable.CurrentTime;
                _endTime = animatable.CurrentTime + animation._duration;
                IsCompleted = false;
            }
        }

        var fp = animation.EasingFunction(p);
        return OnGetMovement(fp);
    }

    /// <summary>
    /// Called to get the movement at a specific progress.
    /// </summary>
    /// <param name="progress">The progress.</param>
    /// <returns></returns>
    protected abstract T OnGetMovement(float progress);

    /// <inheritdoc cref="IMotionProperty.Save"/>
    public void Save() => _savedValue = ToValue;

    /// <inheritdoc cref="IMotionProperty.Restore"/>
    public void Restore(Animatable animatable) => SetMovement(_savedValue, animatable);

    /// <inheritdoc cref="IMotionProperty.CopyFrom(IMotionProperty)"/>
    public void CopyFrom(IMotionProperty source)
    {
        var typedSource = (MotionProperty<T>)source;

        FromValue = typedSource.FromValue;
        ToValue = typedSource.ToValue;
        _startTime = typedSource._startTime;
        _endTime = typedSource._endTime;
        Animation = typedSource.Animation;
        IsCompleted = typedSource.IsCompleted;
    }

    private void SetTimeLine(Animatable animatable, Animation animation)
    {
        if (float.IsNaN(animatable.CurrentTime))
        {
            // this means that the animatable is not being animated yet,
            _startTime = 0;
            _endTime = animation.Duration;
        }
        else
        {
            _startTime = animatable.CurrentTime;
            _endTime = animatable.CurrentTime + animation._duration;
        }

        animation._animationCompletedCount = 0;
        IsCompleted = false;
    }

    private Animation? GetActualAnimation(Animatable animatable) => Animation ??= animatable.DefaultAnimation;
}
