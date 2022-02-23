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

using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines bounds for both, X and Y axes.
/// </summary>
public class DimensionalBounds
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionalBounds"/> class.
    /// </summary>
    /// <param name="setMinBounds">
    /// Indicates if the intiial bounds should contain some values,
    /// just to prevent the chart to be in an invalid range when the control is initializes without any data.
    /// </param>
    internal DimensionalBounds(bool setMinBounds)
    {
        PrimaryBounds = new Bounds();
        SecondaryBounds = new Bounds();
        TertiaryBounds = new Bounds();
        VisiblePrimaryBounds = new Bounds();
        VisibleSecondaryBounds = new Bounds();
        VisibleTertiaryBounds = new Bounds();

        if (!setMinBounds) return;

        VisiblePrimaryBounds.AppendValue(0);
        VisiblePrimaryBounds.AppendValue(10);
        PrimaryBounds.AppendValue(0);
        PrimaryBounds.AppendValue(10);

        VisibleSecondaryBounds.AppendValue(0);
        VisibleSecondaryBounds.AppendValue(10);
        SecondaryBounds.AppendValue(0);
        SecondaryBounds.AppendValue(10);

        VisibleTertiaryBounds.AppendValue(1);
        TertiaryBounds.AppendValue(1);

        IsEmpty = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionalBounds"/> class.
    /// </summary>
    public DimensionalBounds() : this(false)
    { }

    internal bool IsEmpty { get; }

    /// <summary>
    /// Gets or sets the primary bounds.
    /// </summary>
    public Bounds PrimaryBounds { get; set; }

    /// <summary>
    /// Gets or sets the secondary bounds.
    /// </summary>
    public Bounds SecondaryBounds { get; set; }

    /// <summary>
    /// Gets or sets the tertiary bounds.
    /// </summary>
    public Bounds TertiaryBounds { get; set; }

    /// <summary>
    /// Gets or sets the primary bounds.
    /// </summary>
    public Bounds VisiblePrimaryBounds { get; set; }

    /// <summary>
    /// Gets or sets the secondary bounds.
    /// </summary>
    public Bounds VisibleSecondaryBounds { get; set; }

    /// <summary>
    /// Gets or sets the tertiary bounds.
    /// </summary>
    public Bounds VisibleTertiaryBounds { get; set; }
}
