// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Kernel;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// LiveCharts global settings
    /// </summary>
    public class LiveChartsSettings
    {
        private object? currentFactory;
        private readonly Dictionary<Type, object> mappers = new();
        private readonly Dictionary<Type, object> seriesStyleBuilder = new();

        /// <summary>
        /// Adds or replaces a mapping for a given type, the mapper defines how a type is mapped to a <see cref="ChartPoint"/> instance, 
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="predicate">The mapper</param>
        /// <returns></returns>
        public LiveChartsSettings HasMap<TModel>(Action<TModel, ChartPoint> mapper)
        {
            var t = typeof(TModel);
            mappers[t] = mapper;
            return this;
        }

        /// <summary>
        /// Gets the current mapping for a given type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The current mapper</returns>
        internal Action<TModel, ChartPoint> GetMap<TModel>()
        {
            if (!mappers.TryGetValue(typeof(TModel), out var mapper))
                throw new NotImplementedException(
                    $"A mapper for type {typeof(TModel)} is not implemented yet, consider using " +
                    $"{nameof(LiveCharts)}.{nameof(LiveCharts.Configure)}() " +
                    $"method to call {nameof(HasMap)}() with the type you are trying to plot.");

            return (Action<TModel, ChartPoint>)mapper;
        }

        internal LiveChartsSettings HasDataFactory<TDrawingContext>(IDataFactoryProvider<TDrawingContext> factory)
            where TDrawingContext: DrawingContext
        {
            currentFactory = factory;
            return this;
        }

        internal IDataFactoryProvider<TDrawingContext> GetFactory<TDrawingContext>()
            where TDrawingContext : DrawingContext
        {
            if (currentFactory == null) 
                throw new NotImplementedException($"There is no a {nameof(IDataFactoryProvider<TDrawingContext>)} registered");

            return (IDataFactoryProvider<TDrawingContext>) currentFactory;
        }

        public LiveChartsSettings RemoveMap<TModel>()
        {
            mappers.Remove(typeof(TModel));
            return this;
        }

        public LiveChartsSettings AddDefaultStyles<TDrawingContext>(Action<StyleBuilder<TDrawingContext>> builder)
            where TDrawingContext : DrawingContext
        {
            if (!seriesStyleBuilder.TryGetValue(typeof(TDrawingContext), out var stylesBuilder))
            {
                stylesBuilder = new StyleBuilder<TDrawingContext>();
                seriesStyleBuilder[typeof(TDrawingContext)] = stylesBuilder;
            }

            var sb = (StyleBuilder<TDrawingContext>)stylesBuilder;
            builder(sb);

            return this;
        }

        public StyleBuilder<TDrawingContext> GetStylesBuilder<TDrawingContext>()
            where TDrawingContext : DrawingContext
        {
            if (!seriesStyleBuilder.TryGetValue(typeof(TDrawingContext), out var stylesBuilder))
                throw new Exception($"The type {nameof(TDrawingContext)} is not registered.");

            return (StyleBuilder<TDrawingContext>)stylesBuilder;
        }

        /// <summary>
        /// Enables LiveCharts to be able to plot short, int, long, float, double, decimal and <see cref="ChartPoint2"/>.
        /// </summary>
        /// <returns></returns>
        public LiveChartsSettings AddDefaultMappers()
        {
            return HasMap<short>((model, point) =>
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
                     point.PrimaryValue = unchecked((float)model);
                     point.SecondaryValue = point.Context.Index;
                 })
                 .HasMap<decimal>((model, point) =>
                 {
                     point.PrimaryValue = unchecked((float)model);
                     point.SecondaryValue = point.Context.Index;
                 })
                 .HasMap<short?>((model, point) =>
                 {
                     if (model == null)
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
                     if (model == null)
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
                     if (model == null)
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
                     if (model == null)
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
                     if (model == null)
                     {
                         point.IsNull = true;
                         return;
                     }
                     point.IsNull = false;
                     point.PrimaryValue = unchecked((float)model.Value);
                     point.SecondaryValue = point.Context.Index;
                 })
                 .HasMap<decimal?>((model, point) =>
                 {
                     if (model == null)
                     {
                         point.IsNull = true;
                         return;
                     }
                     point.IsNull = false;
                     point.PrimaryValue = unchecked((float)model.Value);
                     point.SecondaryValue = point.Context.Index;
                 })
                 .HasMap<WeightedPoint>((model, point) =>
                 {
                     if (model == null)
                         throw new Exception(
                             $"A {nameof(WeightedPoint)} can not be null, instead set to null to any of its properties.");

                     if (model.Weight == null || model.X == null || model.Y == null)
                     {
                         point.IsNull = true;
                         return;
                     }

                     point.IsNull = false;
                     unchecked
                     {
                         point.PrimaryValue = (float)model.Y.Value;
                         point.SecondaryValue = (float)model.X.Value;
                         point.TertiaryValue = (float)model.Weight.Value;
                     }
                 })
                 .HasMap<ObservableValue>((model, point) =>
                 {
                     if (model == null)
                         throw new Exception(
                             $"A {nameof(ObservableValue)} can not be null, instead set to null to any of its properties.");

                     if (model.Value == null)
                     {
                         point.IsNull = true;
                         return;
                     }

                     point.IsNull = false;
                     unchecked
                     {
                         point.PrimaryValue = (float)model.Value.Value;
                         point.SecondaryValue = point.Context.Index;
                     }
                 })
                 .HasMap<ObservableValueF>((model, point) =>
                 {
                     if (model == null)
                         throw new Exception(
                             $"A {nameof(ObservableValueF)} can not be null, instead set to null to any of its properties.");

                     if (model.Value == null)
                     {
                         point.IsNull = true;
                         return;
                     }

                     point.IsNull = false;
                     unchecked
                     {
                         point.PrimaryValue = model.Value.Value;
                         point.SecondaryValue = point.Context.Index;
                     }
                 })
                 .HasMap<ObservablePoint>((model, point) =>
                 {
                     if (model == null)
                         throw new Exception(
                             $"A {nameof(ObservablePoint)} can not be null, instead set to null to any of its properties.");

                     if (model.X == null || model.Y == null)
                     {
                         point.IsNull = true;
                         return;
                     }

                     point.IsNull = false;
                     unchecked
                     {
                         point.PrimaryValue = (float)model.Y.Value;
                         point.SecondaryValue = (float)model.X.Value;
                     }
                 }).HasMap<ObservablePointF>((model, point) =>
                 {
                     if (model == null)
                         throw new Exception(
                             $"A {nameof(ObservablePointF)} can not be null, instead set to null to any of its properties.");

                     if (model.X == null || model.Y == null)
                     {
                         point.IsNull = true;
                         return;
                     }

                     point.IsNull = false;
                     point.PrimaryValue = model.Y.Value;
                     point.SecondaryValue = model.X.Value;
                 });
        }
    }
}
