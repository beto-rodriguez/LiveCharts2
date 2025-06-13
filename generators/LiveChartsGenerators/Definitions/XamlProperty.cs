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

using Microsoft.CodeAnalysis;

namespace LiveChartsGenerators.Definitions;

public readonly record struct XamlProperty
{
    public readonly string Name;
    public readonly ITypeSymbol Type;
    public readonly ITypeSymbol DeclaringType;
    public readonly string Headers;
    public readonly string? DefaultValueExpression = null;
    public readonly string? OnChangedExpression = null;

    public XamlProperty(
        string name,
        ITypeSymbol type,
        ITypeSymbol declaringType,
        string headers,
        string? defaultValueExpression,
        string? onChangedExpression)
    {
        Name = name;
        Type = type;
        DeclaringType = declaringType;
        Headers = headers;
        DefaultValueExpression = defaultValueExpression;
        OnChangedExpression = onChangedExpression;
    }

    public static Dictionary<int, string> ByPosition = new()
    {
        [0] = "defaultValue:",
        [1] = "onChanged:",
    };
}
