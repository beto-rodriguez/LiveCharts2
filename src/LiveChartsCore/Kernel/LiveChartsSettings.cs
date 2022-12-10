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
    /// Gets or sets the default easing function.
    /// </summary>
    /// <value>
    /// The default easing function.
    /// </value>
    public Func<float, float> DefaultEasingFunction { get; set; } = EasingFunctions.ExponentialOut;

    /// <summary>
    /// Gets or sets the default animations speed.
    /// </summary>
    /// <value>
    /// The default animations speed.
    /// </value>
    public TimeSpan DefaultAnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(800);

    /// <summary>
    /// Gets or sets the default zoom speed.
    /// </summary>
    /// <value>
    /// The default zoom speed.
    /// </value>
    public double DefaultZoomSpeed { get; set; } = 0.2;

    /// <summary>
    /// Gets or sets the default zoom mode.
    /// </summary>
    /// <value>
    /// The default zoom mode.
    /// </value>
    public ZoomAndPanMode DefaultZoomMode { get; set; } = ZoomAndPanMode.None;

    /// <summary>
    /// Gets or sets the default legend position.
    /// </summary>
    /// <value>
    /// The default legend position.
    /// </value>
    public LegendPosition DefaultLegendPosition { get; set; } = LegendPosition.Hidden;

    /// <summary>
    /// Gets or sets the default legend orientation.
    /// </summary>
    /// <value>
    /// The default legend orientation.
    /// </value>
    public LegendOrientation DefaultLegendOrientation { get; set; } = LegendOrientation.Auto;

    /// <summary>
    /// Gets or sets the default tooltip position.
    /// </summary>
    /// <value>
    /// The default tooltip position.
    /// </value>
    public TooltipPosition DefaultTooltipPosition { get; set; } = TooltipPosition.Top;

    /// <summary>
    /// Gets or sets the default tooltip finding strategy.
    /// </summary>
    /// <value>
    /// The default tooltip finding strategy.
    /// </value>
    public TooltipFindingStrategy DefaultTooltipFindingStrategy { get; set; } = TooltipFindingStrategy.Automatic;

    /// <summary>
    /// Gets or sets the default polar initial rotation.
    /// </summary>
    /// <value>
    /// The default animations speed.
    /// </value>
    public double PolarInitialRotation { get; set; } = -90;

    /// <summary>
    /// Gets the theme identifier.
    /// </summary>
    /// <value>
    /// The theme identifier.
    /// </value>
    public object ThemeId { get; private set; } = new();

    /// <summary>
    /// Gets or sets the default update throttling timeout
    /// </summary>
    /// <value>
    /// The default update throttling timeout
    /// </value>
    public TimeSpan DefaultUpdateThrottlingTimeout { get; set; } = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Adds or replaces a mapping for a given type, the mapper defines how a type is mapped to a<see cref="ChartPoint"/> instance,
    /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="mapper">The mapper.</param>
    /// <returns></returns>
    public LiveChartsSettings HasMap<TModel>(Action<TModel, ChartPoint> mapper)
    {
        var t = typeof(TModel);
        _mappers[t] = mapper;
        return this;
    }

    internal Action<TModel, ChartPoint> GetMap<TModel>()
    {
        return !_mappers.TryGetValue(typeof(TModel), out var mapper)
            ? throw new NotImplementedException(
                $"A mapper for type {typeof(TModel)} is not implemented yet, consider using " +
                $"{nameof(LiveCharts)}.{nameof(LiveCharts.Configure)}() " +
                $"method to call {nameof(HasMap)}() with the type you are trying to plot.")
            : (Action<TModel, ChartPoint>)mapper;
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
    /// <returns>the current settings</returns>
    public LiveChartsSettings WithDefaultAnimationsSpeed(TimeSpan animationsSpeed)
    {
        DefaultAnimationsSpeed = animationsSpeed;
        return this;
    }

    /// <summary>
    /// Withes the default easing function.
    /// </summary>
    /// <param name="easingFunction">The easing function.</param>
    /// <returns>the current settings</returns>
    public LiveChartsSettings WithDefaultEasingFunction(Func<float, float> easingFunction)
    {
        DefaultEasingFunction = easingFunction;
        return this;
    }

    /// <summary>
    /// Withes the default zoom speed.
    /// </summary>
    /// <param name="speed">The speed.</param>
    /// <returns>the current settings</returns>
    public LiveChartsSettings WithDefaultZoomSpeed(double speed)
    {
        DefaultZoomSpeed = speed;
        return this;
    }

    /// <summary>
    /// Withes the default zoom mode.
    /// </summary>
    /// <param name="zoomMode">The zoom mode.</param>
    /// <returns>the current settings</returns>
    public LiveChartsSettings WithDefaultZoomMode(ZoomAndPanMode zoomMode)
    {
        DefaultZoomMode = zoomMode;
        return this;
    }

    /// <summary>
    /// Withes the default update throttling timeout
    /// </summary>
    /// <param name="timeout">The update throttling timeout.</param>
    /// <returns>the current settings</returns>
    public LiveChartsSettings WithDefaultUpdateThrottlingTimeout(TimeSpan timeout)
    {
        DefaultUpdateThrottlingTimeout = timeout;
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
        ThemeId = new object();
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
            HasMap<short>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<int>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<long>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<float>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<double>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<decimal>((model, point) =>
            {
                point.PrimaryValue = (double)model;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<short?>((model, point) =>
            {
                if (model is null) throw new Exception("Unexpected exception");
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<int?>((model, point) =>
            {
                if (model is null) throw new Exception("Unexpected exception");
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<long?>((model, point) =>
            {
                if (model is null) throw new Exception("Unexpected exception");
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<float?>((model, point) =>
            {
                if (model is null) throw new Exception("Unexpected exception");
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<double?>((model, point) =>
            {
                if (model is null) throw new Exception("Unexpected exception");
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            })
            .HasMap<decimal?>((model, point) =>
            {
                if (model is null) throw new Exception("Unexpected exception");
                point.PrimaryValue = (double)model.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            });
    }
}
