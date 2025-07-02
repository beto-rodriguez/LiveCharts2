
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

public class WPFTemplate(FrameworkTemplate.Context context) : FrameworkTemplate(context)
{
    public override string Key => "WPF";

    public override string DeclareBindableProperty(string propertyName, string propertyType)
        => @$"public static readonly new global::System.Windows.DependencyProperty {propertyName}Property";

    public override string CreateBindableProperty(
        string propertyName, string propertyType, string bindableType, string defaultValue, OnChangeInfo? onChangeInfo = null)
    {
        var sanitizedPropertyType = propertyType.EndsWith("?")
            ? propertyType.Substring(0, propertyType.Length - 1)
            : propertyType;

        return @$"global::System.Windows.DependencyProperty.Register(name: ""{propertyName}"", propertyType: typeof({sanitizedPropertyType}), ownerType: typeof({bindableType}), typeMetadata: new System.Windows.PropertyMetadata(defaultValue:{defaultValue}{(onChangeInfo is null ? string.Empty : $", propertyChangedCallback: {GetOnChangedExpression(onChangeInfo.Value, bindableType, propertyType)}")}));";
    }

    public override string GetPropertyChangedMetod() =>
        @$"protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs args)
    {{
        base.OnPropertyChanged(args);
        MapChangeToBaseType(args.Property.Name);
    }}";

    private string GetOnChangedExpression(OnChangeInfo onChangeInfo, string bindableType, string propertyType)
    {
        if (!onChangeInfo.HasChangeParams)
            return $@"(bo, o, n) => {onChangeInfo.Expression}(({bindableType})bo)";

        var cast = onChangeInfo.HasChangeObjectParams ? string.Empty : $"({propertyType})";
        return $@"(o, args) => {onChangeInfo.Expression}(({bindableType})o, {cast}args.OldValue, {cast}args.NewValue)";
    }
}
