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

using System;
using System.Collections.Generic;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <inheritdoc cref="Animatable" />
public abstract class Animatable
{
    private readonly FloatMotionProperty _xProperty;
    private readonly FloatMotionProperty _yProperty;
    private readonly FloatMotionProperty _rotationProperty;
    private readonly PointMotionProperty _transformOriginProperty;
    private readonly PointMotionProperty _scaleProperty;
    private readonly PointMotionProperty _skewProperty;
    private readonly PointMotionProperty _translateProperty;
    private readonly FloatMotionProperty _opacityProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Animatable"/> class.
    /// </summary>
    protected Animatable(bool hasGeometryTransform = false)
    {
        HasTransform = hasGeometryTransform;
        _xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
        _yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
        _transformOriginProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(TransformOrigin), new LvcPoint(0.5f, 0.5f)));
        _translateProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(TranslateTransform), new LvcPoint(0, 0)));
        _rotationProperty = RegisterMotionProperty(
            new FloatMotionProperty(nameof(RotateTransform), 0));
        _scaleProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(ScaleTransform), new LvcPoint(1, 1)));
        _skewProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(SkewTransform), new LvcPoint(1, 1)));
        _opacityProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Opacity), 1));
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is valid, the instance is valid when all the
    /// motion properties in the object finished their animations.
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Gets or sets the current time, this property is used by the motion engine to calculate the progress of the animations.
    /// </summary>
    public long CurrentTime { get; set; } = long.MinValue;

    /// <summary>
    /// Gets or sets a value indicating whether this instance should be removed from the canvas when all the animations are completed.
    /// </summary>
    public bool RemoveOnCompleted { get; set; }

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    public float Opacity
    {
        get => _opacityProperty.GetMovement(this);
        set => _opacityProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.X"/>
    public float X
    {
        get => Parent is null
            ? _xProperty.GetMovement(this)
            : _xProperty.GetMovement(this) + Parent.X;
        set => _xProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.Y"/>
    public float Y
    {
        get => Parent is null
            ? _yProperty.GetMovement(this)
            : _yProperty.GetMovement(this) + Parent.Y;
        set => _yProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.TransformOrigin"/>
    public LvcPoint TransformOrigin
    {
        get => _transformOriginProperty.GetMovement(this);
        set => _transformOriginProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.TranslateTransform"/>
    public LvcPoint TranslateTransform
    {
        get => _translateProperty.GetMovement(this);
        set
        {
            _translateProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.RotateTransform"/>
    public float RotateTransform
    {
        get => _rotationProperty.GetMovement(this);
        set
        {
            _rotationProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.ScaleTransform"/>
    public LvcPoint ScaleTransform
    {
        get => _scaleProperty.GetMovement(this);
        set
        {
            _scaleProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.SkewTransform"/>
    public LvcPoint SkewTransform
    {
        get => _skewProperty.GetMovement(this);
        set
        {
            _skewProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.HasTransform"/>
    public bool HasTransform { get; protected set; }

    /// <inheritdoc cref="IDrawable.HasTranslate"/>
    public bool HasTranslate
    {
        get
        {
            var t = TranslateTransform;
            return t.X != 0 || t.Y != 0;
        }
    }

    /// <inheritdoc cref="IDrawable.HasScale"/>
    public bool HasScale
    {
        get
        {
            var s = ScaleTransform;
            return s.X != 1 || s.Y != 1;
        }
    }

    /// <inheritdoc cref="IDrawable.HasSkew"/>
    public bool HasSkew
    {
        get
        {
            var s = SkewTransform;
            return s.X != 1 || s.Y != 1;
        }
    }

    /// <inheritdoc cref="IDrawable.HasSkew"/>
    public bool HasRotation => Math.Abs(RotateTransform) > 0;

    /// <summary>
    /// Gets or sets the stroke paint.
    /// </summary>
    public Paint? Stroke { get; set; }

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public Paint? Fill { get; set; }

    /// <summary>
    /// Gets or sets the parent shape, if any the X and Y properties will be relative to the parent.
    /// </summary>
    public CoreGeometry? Parent { get; set; }

    /// <summary>
    /// Gets the motion properties.
    /// </summary>
    public Dictionary<string, IMotionProperty> MotionProperties { get; } = [];

    /// <summary>
    /// Sets the transition for the specified properties.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="propertyName">The property name, null to select all properties.</param>
    public void SetTransition(Animation? animation, params string[]? propertyName)
    {
        var a = animation?.Duration == 0 ? null : animation;
        if (propertyName is null || propertyName.Length == 0) propertyName = [.. MotionProperties.Keys];

        foreach (var name in propertyName)
            MotionProperties[name].Animation = a;
    }

    /// <summary>
    /// Removes the transition for the specified properties.
    /// </summary>
    /// <param name="propertyName">The properties to remove, null to select all properties.</param>
    public void RemoveTransition(params string[]? propertyName)
    {
        if (propertyName is null || propertyName.Length == 0) propertyName = [.. MotionProperties.Keys];

        foreach (var name in propertyName)
        {
            MotionProperties[name].Animation = null;
        }
    }

    /// <summary>
    /// Completes the transition for the specified properties.
    /// </summary>
    /// <param name="propertyName">The property name, null to select all properties.</param>
    public virtual void CompleteTransition(params string[]? propertyName)
    {
        if (propertyName is null || propertyName.Length == 0) propertyName = [.. MotionProperties.Keys];

        foreach (var property in propertyName)
        {
            if (!MotionProperties.TryGetValue(property, out var transitionProperty))
                throw new Exception(
                    $"The property {property} is not a transition property of this instance.");

            if (transitionProperty.Animation is null) continue;
            transitionProperty.IsCompleted = true;
        }
    }

    /// <summary>
    /// Registers a motion property.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="motionProperty">The transition.</param>
    /// <returns></returns>
    protected T RegisterMotionProperty<T>(T motionProperty)
        where T : IMotionProperty
    {
        MotionProperties[motionProperty.PropertyName] = motionProperty;
        return motionProperty;
    }
}
