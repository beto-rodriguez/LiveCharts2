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
    private readonly Dictionary<Type, object> _seriesStyleBuilder = new();

    /// <summary>
    /// Gets or sets the default easing function.
    /// </summary>
    /// <value>
    /// The default easing function.
    /// </value>
    public Func<float, float> DefaultEasingFunction { get; set; } = EasingFunctions.QuadraticOut;

    /// <summary>
    /// Gets or sets the default animations speed.
    /// </summary>
    /// <value>
    /// The default animations speed.
    /// </value>
    public TimeSpan DefaultAnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

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

    internal LiveChartsSettings HasProvider<TDrawingContext>(ChartProvider<TDrawingContext> factory)
        where TDrawingContext : DrawingContext
    {
        _currentProvider = factory;
        return this;
    }

    internal ChartProvider<TDrawingContext> GetProvider<TDrawingContext>()
        where TDrawingContext : DrawingContext
    {
        return _currentProvider is null
            ? throw new NotImplementedException(
                $"There is no a {nameof(ChartProvider<TDrawingContext>)} registered.")
            : (ChartProvider<TDrawingContext>)_currentProvider;
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
        if (!_seriesStyleBuilder.TryGetValue(typeof(TDrawingContext), out var stylesBuilder))
        {
            stylesBuilder = new Theme<TDrawingContext>();
            _seriesStyleBuilder[typeof(TDrawingContext)] = stylesBuilder;
        }

        ThemeId = new object();
        var sb = (Theme<TDrawingContext>)stylesBuilder;
        builder(sb);

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
        return !_seriesStyleBuilder.TryGetValue(typeof(TDrawingContext), out var stylesBuilder)
            ? throw new Exception($"The type {nameof(TDrawingContext)} is not registered.")
            : (Theme<TDrawingContext>)stylesBuilder;
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
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<int>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<long>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<float>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<double>((model, point) =>
            {
                point.PrimaryValue = model;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<decimal>((model, point) =>
            {
                point.PrimaryValue = unchecked((double)model);
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<short?>((model, point) =>
            {
                if (model is null)
                {
                    point.IsNull = true;
                    return;
                }
                point.IsNull = false;
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<int?>((model, point) =>
            {
                if (model is null)
                {
                    point.IsNull = true;
                    return;
                }
                point.IsNull = false;
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<long?>((model, point) =>
            {
                if (model is null)
                {
                    point.IsNull = true;
                    return;
                }
                point.IsNull = false;
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<float?>((model, point) =>
            {
                if (model is null)
                {
                    point.IsNull = true;
                    return;
                }
                point.IsNull = false;
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<double?>((model, point) =>
            {
                if (model is null)
                {
                    point.IsNull = true;
                    return;
                }
                point.IsNull = false;
                point.PrimaryValue = model.Value;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<decimal?>((model, point) =>
            {
                if (model is null)
                {
                    point.IsNull = true;
                    return;
                }
                point.IsNull = false;
                point.PrimaryValue = unchecked((double)model.Value);
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<WeightedPoint>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(WeightedPoint)} can not be null, instead set to null to any of its properties.");

                if (model.Weight is null || model.X is null || model.Y is null)
                {
                    point.IsNull = true;
                    return;
                }

                point.IsNull = false;
                point.PrimaryValue = model.Y.Value;
                point.SecondaryValue = model.X.Value;
                point.TertiaryValue = model.Weight.Value;
            })
            .HasMap<ObservableValue>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(ObservableValue)} can not be null, instead set to null to any of its properties.");

                if (model.Value is null)
                {
                    point.IsNull = true;
                    return;
                }

                point.IsNull = false;
                point.PrimaryValue = model.Value.Value;
                point.SecondaryValue = point.Context.Index;
            })
            .HasMap<ObservablePoint>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(ObservablePoint)} can not be null, instead set to null to any of its properties.");

                if (model.X is null || model.Y is null)
                {
                    point.IsNull = true;
                    return;
                }

                point.IsNull = false;
                point.PrimaryValue = model.Y.Value;
                point.SecondaryValue = model.X.Value;
            })
            .HasMap<ObservablePolarPoint>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(ObservablePolarPoint)} can not be null, instead set to null to any of its properties.");

                if (model.Angle is null || model.Radius is null)
                {
                    point.IsNull = true;
                    return;
                }

                point.IsNull = false;
                point.PrimaryValue = model.Radius.Value;
                point.SecondaryValue = model.Angle.Value;
            })
            .HasMap<DateTimePoint>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(DateTimePoint)} can not be null, instead set to null the " +
                        $"{nameof(DateTimePoint.Value)} property.");

                if (model.Value is null)
                {
                    point.IsNull = true;
                    return;
                }

                point.IsNull = false;
                point.PrimaryValue = model.Value.Value;
                point.SecondaryValue = model.DateTime.Ticks;
            })
            .HasMap<TimeSpanPoint>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(TimeSpanPoint)} can not be null, instead set to null the " +
                        $"{nameof(TimeSpanPoint.Value)} property.");

                if (model.Value is null)
                {
                    point.IsNull = true;
                    return;
                }

                point.IsNull = false;
                point.PrimaryValue = model.Value.Value;
                point.SecondaryValue = model.TimeSpan.Ticks;
            })
            .HasMap<FinancialPoint>((model, point) =>
            {
                if (model is null)
                    throw new Exception(
                        $"A {nameof(FinancialPoint)} can not be null");

                point.PrimaryValue = model.High.Value;
                point.SecondaryValue = model.Date.Ticks;
                point.TertiaryValue = model.Open.Value;
                point.QuaternaryValue = model.Close.Value;
                point.QuinaryValue = model.Low.Value;
            });
    }
}
