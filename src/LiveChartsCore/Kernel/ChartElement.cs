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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines a visual element in a chart.
/// </summary>
public abstract class ChartElement<TDrawingContext> : IChartElement<TDrawingContext>, INotifyPropertyChanged
    where TDrawingContext : DrawingContext
{
    internal bool _isInternalSet = false;
    internal bool _isThemeSet = false;
    internal readonly HashSet<string> _userSets = new();
    private readonly List<IPaint<TDrawingContext>> _deletingTasks = new();

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc cref="IChartElement{TDrawingContext}.Tag" />
    public object? Tag { get; set; }

    /// <inheritdoc cref="IChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})" />
    public abstract void Invalidate(Chart<TDrawingContext> chart);

    /// <inheritdoc cref="IChartElement{TDrawingContext}.RemoveOldPaints(IChartView{TDrawingContext})" />
    public void RemoveOldPaints(IChartView<TDrawingContext> chart)
    {
        if (_deletingTasks.Count == 0) return;

        foreach (var item in _deletingTasks)
        {
            chart.CoreCanvas.RemovePaintTask(item);
            item.Dispose();
        }

        _deletingTasks.Clear();
    }

    /// <inheritdoc cref="IChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})" />
    public virtual void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        foreach (var item in GetPaintTasks())
        {
            if (item is null) continue;
            chart.Canvas.RemovePaintTask(item);
            item.ClearGeometriesFromPaintTask(chart.Canvas);
        }
    }

    /// <summary>
    /// Gets the paint tasks registered by the <see cref="ChartElement{TDrawingContext}"/>.
    /// </summary>
    /// <returns></returns>
    internal abstract IPaint<TDrawingContext>?[] GetPaintTasks();

    /// <summary>
    /// Sets a property value and handles the paints in the canvas.
    /// </summary>
    /// <param name="reference">The referenced paint task.</param>
    /// <param name="value">The value</param>
    /// <param name="isStroke">if set to <c>true</c> [is stroke].</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected virtual void SetPaintProperty(
        ref IPaint<TDrawingContext>? reference,
        IPaint<TDrawingContext>? value,
        bool isStroke = false,
        [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));
        if (!CanSetProperty(propertyName)) return;

        if (reference is not null) _deletingTasks.Add(reference);
        reference = value;

        if (reference is not null)
        {
            reference.IsStroke = isStroke;
            reference.IsFill = !isStroke; // seems unnecessary ????
            if (!isStroke) reference.StrokeThickness = 0;
        }

        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Sets a property value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reference"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    protected virtual void SetProperty<T>(
        ref T reference,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));
        if (!CanSetProperty(propertyName)) return;

        reference = value;
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Determines whether the property can be set.
    /// 1. The user always can.
    /// 2. A style can set it only if the user did not.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected bool CanSetProperty(string propertyName)
    {
        return                                  // a property can be set if:
            !_isInternalSet                     // 1. it is an user action (not set by a theme).
            ||                                  // or
            !_userSets.Contains(propertyName);  // 2. the user has not set the property.
    }

    /// <summary>
    /// Schedules the delete for thew given <see cref="IPaint{TDrawingContext}"/> instance.
    /// </summary>
    /// <returns></returns>
    protected void ScheduleDeleteFor(IPaint<TDrawingContext> paintTask)
    {
        _deletingTasks.Add(paintTask);
    }

    /// <summary>
    /// Called when the fill changes.
    /// </summary>
    /// <returns></returns>
    protected virtual void OnPaintChanged(string? propertyName) { }

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        // only invoke property change event when the user set the property.
        if (_isInternalSet) return;

        _ = _userSets.Add(propertyName ?? throw new ArgumentNullException(nameof(propertyName)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
