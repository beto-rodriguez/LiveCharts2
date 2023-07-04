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

using LiveChartsCore.Kernel;

namespace LiveChartsCore.Defaults;

/// <summary>
/// Defines the <see cref="MappedChartEntity"/> class, a helper class to map any object that does not implements <see cref="IChartEntity"/>,
/// when you need a better performance you should implement <see cref="IChartEntity"/> in your DTOs, the default objects in the library
/// already implement <see cref="IChartEntity"/>.
/// </summary>
public sealed class MappedChartEntity : IChartEntity
{
    /// <inheritdoc cref="IChartEntity.MetaData"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public ChartEntityMetaData? MetaData { get; set; }

    /// <inheritdoc cref="IChartEntity.Coordinate"/>
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#else
    [Newtonsoft.Json.JsonIgnore]
#endif
    public Coordinate Coordinate { get; set; } = Coordinate.Empty;
}
