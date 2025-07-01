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

namespace LiveChartsGenerators.Frameworks;

public class WinUITemplate(FrameworkTemplate.Context context) : FrameworkTemplate(context)
{
    public override string Key => "WinUI";

    public override string DeclareBindableProperty(string propertyName, string propertyType)
        => @$"public static readonly new global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property";

    public override string CreateBindableProperty(
        string propertyName, string propertyType, string bindableType, string defaultValue, string? onChanged = null)
    {
        var sanitizedPropertyType = propertyType.EndsWith("?")
            ? propertyType.Substring(0, propertyType.Length - 1)
            : propertyType;

        return @$"global::Microsoft.UI.Xaml.DependencyProperty.Register(name: ""{propertyName}"", propertyType: typeof({sanitizedPropertyType}), ownerType: typeof({bindableType}), typeMetadata: new Microsoft.UI.Xaml.PropertyMetadata(defaultValue:{defaultValue}, propertyChangedCallback: {GetOnChangedExpression(onChanged, bindableType, propertyName, propertyType)}));";
    }

    public override string GetPropertyChangedMetod() => string.Empty;

    private string GetOnChangedExpression(string? expression, string bindableType, string propertyName, string propertyType)
    {
        var changeExpression = expression is null
            ? string.Empty
            : $"{expression}(t, ({propertyType})args.OldValue, ({propertyType})args.NewValue);";

        var mapExpression = ActiveContext != Context.XamlObject
            ? string.Empty
            : @$"t.MapChangeToBaseType(""{propertyName}"");";

        return $@"(o, args) => {{ var t = ({bindableType})o; {mapExpression} {changeExpression} }}";
    }
}
