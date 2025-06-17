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

namespace LiveChartsCore.Generators;

/// <summary>
/// Marks a class to be generated as a XAML friendly object, LiveCharts will wrap the object in a XAML friendly object,
/// then when a property changes in the XAML object, it updates the LiveCharts object.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="XamlClassAttribute"/> class.
/// </remarks>
/// <param name="basedOn">The base type</param>
[AttributeUsage(AttributeTargets.Class)]
public class XamlClassAttribute(Type basedOn) : Attribute
{
    /// <summary>
    /// The base type.
    /// </summary>
    public Type BaseType { get; } = basedOn;

    /// <summary>
    /// Also maps the specified type.
    /// </summary>
    public Type? Map { get; set; }

    /// <summary>
    /// The path to the map.
    /// </summary>
    public string? MapPath { get; set; }

    /// <summary>
    /// The header to add to the generated file.
    /// </summary>
    public string? FileHeader { get; set; }

    /// <summary>
    /// Indicates whether the generator should generate the OnPropertyChanged method, default is true.
    /// </summary>
    public bool GenerateOnChange { get; set; } = true;

    /// <summary>
    /// A string with the property change map e.g.
    /// MyProperty{=}MyMapMethod{,}MyOtherProperty{=}MyOtherMapMethod.
    /// </summary>
    public string? PropertyChangeMap { get; set; }

    /// <summary>
    /// A string with the property type overrides e.g.
    /// MyProperty{=}double{,}MyOtherProperty{=}object.
    /// </summary>
    public string? PropertyTypeOverride { get; set; }

    /// <summary>
    /// Indicates whether the generator should generate the base type declaration, default is true.
    /// </summary>
    public bool GenerateBaseTypeDeclaration { get; set; } = true;

    /// <summary>
    /// Indicates the type of the model.
    /// </summary>
    public Type? TModel { get; set; }

    /// <summary>
    /// Indicates the type of the visual.
    /// </summary>
    public Type? TVisual { get; set; }

    /// <summary>
    /// Indicates the type of the label.
    /// </summary>
    public Type? TLabel { get; set; }
}
