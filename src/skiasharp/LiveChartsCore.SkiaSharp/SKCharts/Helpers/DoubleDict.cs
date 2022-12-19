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

using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharpView.SKCharts.Helpers;

// Maybe we should go for another alternative instead of using this class..
internal class DoubleDict<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly Dictionary<T1, T2> _keys = new();
    private readonly Dictionary<T2, T1> _values = new();

    public void Add(T1 key, T2 value)
    {
        _keys.Add(key, value);
        _values.Add(value, key);
    }

    public bool Remove(T1 key)
    {
        var r2 = _values.Remove(_keys[key]);
        var r1 = _keys.Remove(key);
        return r1 & r2;
    }

    public bool Remove(T2 value)
    {
        var r1 = _keys.Remove(_values[value]);
        var r2 = _values.Remove(value);
        return r1 & r2;
    }

    public bool TryGetValue(T1 key, out T2 value)
    {
        return _keys.TryGetValue(key, out value!);
    }

    public bool TryGetValue(T2 key, out T1 value)
    {
        return _values.TryGetValue(key, out value!);
    }
}
