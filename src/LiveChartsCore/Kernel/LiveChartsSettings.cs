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

// Ignore Spelling: Tooltip

using System;
using System.Collections.Generic;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Providers;
using LiveChartsCore.Measure;
using LiveChartsCore.Themes;

namespace LiveChartsCore.Kernel;

/// <summary>
/// LiveCharts global settings
/// </summary>
public class LiveChartsSettings
{
    private object? _currentProvider;
    private readonly Dictionary<Type, object> _mappers = new();
    private object _theme = new();

    /// <summary>
    /// Gets the theme identifier.
    /// </summary>
    /// <value>
    /// The theme identifier.
    /// </value>
    public object CurrentThemeId { get; private set; } = new();

    /// <summary>
    /// Gets or sets the default easing function.
    /// </summary>
    /// <value>
    /// The default easing function.
    /// </value>
    public Func<float, float> EasingFunction { get; set; } = EasingFunctions.ExponentialOut;

    /// <summary>
    /// Gets or sets the default animations speed.
    /// </summary>
    /// <value>
    /// The default animations speed.
    /// </value>
    public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(800);

    /// <summary>
    /// Gets or sets the default zoom speed.
    /// </summary>
    /// <value>
    /// The default zoom speed.
    /// </value>
    public double ZoomSpeed { get; set; } = 0.2;

    /// <summary>
    /// Gets or sets the default zoom mode.
    /// </summary>
    /// <value>
    /// The default zoom mode.
    /// </value>
    public ZoomAndPanMode ZoomMode { get; set; } = ZoomAndPanMode.None;

    /// <summary>
    /// Gets or sets the default legend position.
    /// </summary>
    /// <value>
    /// The default legend position.
    /// </value>
    public LegendPosition LegendPosition { get; set; } = LegendPosition.Hidden;

    /// <summary>
    /// Gets or sets the default legend background paint.
    /// </summary>
    public object? LegendBackgroundPaint { get; set; }

    /// <summary>
    /// Gets or sets the default legend text paint.
    /// </summary>
    public object? LegendTextPaint { get; set; }

    /// <summary>
    /// Gets or sets the default legend text size.
    /// </summary>
    public double? LegendTextSize { get; set; }

    /// <summary>
    /// Gets or sets the default tooltip position.
    /// </summary>
    /// <value>
    /// The default tooltip position.
    /// </value>
    public TooltipPosition TooltipPosition { get; set; } = TooltipPosition.Auto;

    /// <summary>
    /// Gets or sets the default tooltip background paint.
    /// </summary>
    public object? TooltipBackgroundPaint { get; set; }

    /// <summary>
    /// Gets or sets the default tooltip text paint.
    /// </summary>
    public object? TooltipTextPaint { get; set; }

    /// <summary>
    /// Gets or sets the default tooltip text size.
    /// </summary>
    public double? TooltipTextSize { get; set; }

    /// <summary>
    /// Gets or sets the default max with for labels inside tooltips and legends.
    /// </summary>
    public double MaxTooltipsAndLegendsLabelsWidth { get; set; } = 170;

    /// <summary>
    /// Gets or sets the default tooltip finding strategy.
    /// </summary>
    /// <value>
    /// The default tooltip finding strategy.
    /// </value>
    public TooltipFindingStrategy TooltipFindingStrategy { get; set; } = TooltipFindingStrategy.Automatic;

    /// <summary>
    /// Gets or sets the default polar initial rotation.
    /// </summary>
    /// <value>
    /// The default animations speed.
    /// </value>
    public double PolarInitialRotation { get; set; } = -90;

    /// <summary>
    /// Gets or sets the default update throttling timeout
    /// </summary>
    /// <value>
    /// The default update throttling timeout
    /// </value>
    public TimeSpan UpdateThrottlingTimeout { get; set; } = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Gets or sets a value indicating whether the text is right to left.
    /// </summary>
    public bool IsRightToLeft { get; set; }

    /// <summary>
    /// Adds or replaces a mapping for a given type, the mapper defines how a type is mapped to a <see cref="Coordinate"/>
    /// in the chart.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="mapper">The mapper.</param>
    /// <returns></returns>
    public LiveChartsSettings HasMap<TModel>(Func<TModel, int, Coordinate> mapper)
    {
        var t = typeof(TModel);
        _mappers[t] = mapper;
        return this;
    }

    /// <summary>
    /// Indicates that the library should render tooltips in a right to left mode, you also need to load
    /// a right to left font.
    /// </summary>
    /// <returns></returns>
    public LiveChartsSettings UseRightToLeftSettings()
    {
        IsRightToLeft = true;
        return this;
    }

    internal Func<TModel, int, Coordinate> GetMap<TModel>()
    {
        return !_mappers.TryGetValue(typeof(TModel), out var mapper)
            ? throw new NotImplementedException(
                $"A mapper for type {typeof(TModel)} is not implemented yet, consider using " +
                $"{nameof(LiveCharts)}.{nameof(LiveCharts.Configure)}() " +
                $"method to call {nameof(HasMap)}() with the type you are trying to plot.")
            : (Func<TModel, int, Coordinate>)mapper;
    }

    internal LiveChartsSettings HasProvider<TDrawingContext>(ChartEngine<TDrawingContext> factory)
        where TDrawingContext : DrawingContext
    {
        _currentProvider = factory;
        return this;
    }

    internal ChartEngine<TDrawingContext> GetProvider<TDrawingContext>()
        where TDrawingContext : DrawingContext
    {
        return _currentProvider is null
            ? throw new NotImplementedException(
                $"There is no a {nameof(ChartEngine<TDrawingContext>)} registered.")
            : (ChartEngine<TDrawingContext>)_currentProvider;
    }

    /// <summary>
    /// Sets the default animations speed.
    /// </summary>
    /// <param name="animationsSpeed">The animations speed.</param>
    /// <returns>The current settings</returns>
    public LiveChartsSettings WithAnimationsSpeed(TimeSpan animationsSpeed)
    {
        AnimationsSpeed = animationsSpeed;
        return this;
    }

    /// <summary>
    /// Sets the default the default easing function.
    /// </summary>
    /// <param name="easingFunction">The easing function.</param>
    /// <returns>The current settings</returns>
    public LiveChartsSettings WithEasingFunction(Func<float, float> easingFunction)
    {
        EasingFunction = easingFunction;
        return this;
    }

    /// <summary>
    /// Sets the default the default zoom speed.
    /// </summary>
    /// <param name="speed">The speed.</param>
    /// <returns>The current settings</returns>
    public LiveChartsSettings WithZoomSpeed(double speed)
    {
        ZoomSpeed = speed;
        return this;
    }

    /// <summary>
    /// Sets the default the default zoom mode.
    /// </summary>
    /// <param name="zoomMode">The zoom mode.</param>
    /// <returns>The current settings</returns>
    public LiveChartsSettings WithZoomMode(ZoomAndPanMode zoomMode)
    {
        ZoomMode = zoomMode;
        return this;
    }

    /// <summary>
    /// Sets the default the default update throttling timeout
    /// </summary>
    /// <param name="timeout">The update throttling timeout.</param>
    /// <returns>The current settings</returns>
    public LiveChartsSettings WithUpdateThrottlingTimeout(TimeSpan timeout)
    {
        UpdateThrottlingTimeout = timeout;
        return this;
    }

    /// <summary>
    /// Sets the default legend background paint.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="paint">The paint.</param>
    /// <returns>The current settings.</returns>
    public LiveChartsSettings WithLegendBackgroundPaint<TDrawingContext>(IPaint<TDrawingContext> paint)
        where TDrawingContext : DrawingContext
    {
        LegendBackgroundPaint = paint;
        return this;
    }

    /// <summary>
    /// Sets the default legend text paint.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="paint">The paint.</param>
    /// <returns>The current settings.</returns>
    public LiveChartsSettings WithLegendTextPaint<TDrawingContext>(IPaint<TDrawingContext> paint)
        where TDrawingContext : DrawingContext
    {
        LegendTextPaint = paint;
        return this;
    }

    /// <summary>
    /// Sets the default legend text size.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>The current settings.</returns>
    public LiveChartsSettings WithLegendTextSize<TDrawingContext>(double? size)
    {
        LegendTextSize = size;
        return this;
    }

    /// <summary>
    /// Sets the default tooltip background paint.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="paint">The paint.</param>
    /// <returns>The current settings.</returns>
    public LiveChartsSettings WithTooltipBackgroundPaint<TDrawingContext>(IPaint<TDrawingContext> paint)
        where TDrawingContext : DrawingContext
    {
        TooltipBackgroundPaint = paint;
        return this;
    }

    /// <summary>
    /// Sets the default tooltip text paint.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="paint">The paint.</param>
    /// <returns>The current settings.</returns>
    public LiveChartsSettings WithTooltipTextPaint<TDrawingContext>(IPaint<TDrawingContext> paint)
        where TDrawingContext : DrawingContext
    {
        TooltipTextPaint = paint;
        return this;
    }

    /// <summary>
    /// Sets the default tooltip text size.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>The current settings.</returns>
    public LiveChartsSettings WithTooltipTextSize<TDrawingContext>(double? size)
    {
        TooltipTextSize = size;
        return this;
    }

    /// <summary>
    /// Removes a map from the settings.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <returns></returns>
    public LiveChartsSettings RemoveMap<TModel>()
    {
        _ = _mappers.Remove(typeof(TModel));
        return this;
    }

    /// <summary>
    /// Adds the default styles.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public LiveChartsSettings HasTheme<TDrawingContext>(Action<Theme<TDrawingContext>> builder)
        where TDrawingContext : DrawingContext
    {
        CurrentThemeId = new object();
        Theme<TDrawingContext> t;
        _theme = t = new Theme<TDrawingContext>();
        builder(t);

        return this;
    }

    /// <summary>
    /// Gets the styles builder.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <returns></returns>
    /// <exception cref="Exception">$"The type {nameof(TDrawingContext)} is not registered.</exception>
    public Theme<TDrawingContext> GetTheme<TDrawingContext>()
        where TDrawingContext : DrawingContext
    {
        return (Theme<TDrawingContext>?)_theme ?? throw new Exception("A theme is required.");
    }

    /// <summary>
    /// Enables LiveCharts to be able to plot short, int, long, float, double, decimal, short?, int?, long?, float?, double?, decimal?,
    /// <see cref="WeightedPoint"/>, <see cref="ObservableValue"/>, <see cref="ObservablePoint"/>, <see cref="DateTimePoint"/> and
    /// <see cref="FinancialPoint"/>.
    /// </summary>
    /// <returns></returns>
    public LiveChartsSettings AddDefaultMappers()
    {
        return
            HasMap<short>((model, index) => new(index, model))
            .HasMap<int>((model, index) => new(index, model))
            .HasMap<long>((model, index) => new(index, model))
            .HasMap<float>((model, index) => new(index, model))
            .HasMap<double>((model, index) => new(index, model))
            .HasMap<decimal>((model, index) => new(index, (double)model))
            .HasMap<short?>((model, index) => new(index, model!.Value))
            .HasMap<int?>((model, index) => new(index, model!.Value))
            .HasMap<long?>((model, index) => new(index, model!.Value))
            .HasMap<float?>((model, index) => new(index, model!.Value))
            .HasMap<double?>((model, index) => new(index, model!.Value))
            .HasMap<decimal?>((model, index) => new(index, (double)model!.Value));
    }
}
