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

using LiveChartsGenerators.Definitions;
using Microsoft.CodeAnalysis;

namespace LiveChartsGenerators.Frameworks;

public class WinformsTemplate(FrameworkTemplate.Context context) : FrameworkTemplate(context)
{
    public override string Key => "WinForms";

    public override string DeclareBindableProperty(string propertyName, string propertyType) => string.Empty;

    public override string CreateBindableProperty(
        string propertyName, string propertyType, bool isValueTypeProperty, string bindableType, string defaultValue, OnChangeInfo? onChangeInfo = null)
            => string.Empty;

    public override string GetPropertyChangedMetod() => throw new NotImplementedException();

    public override string FullPropertySyntax(XamlProperty property)
    {
        var propertyName = property.Name;

        var displayFormat2 = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

        var propertyType = $"global::{property.Type.ToDisplayString(displayFormat2)}";

        var changeExpression = string.Empty;
        if (property.OnChangeInfo is { } change)
        {
            var cast = change.HasChangeObjectParams ? string.Empty : $"({propertyType})";

            changeExpression = change.HasChangeParams
                ? $@"{change.Expression}(this, {cast}oldValue, {cast}value);"
                : $@"{change.Expression}(this);";
        }

        var field = $"__field{propertyName}";

        return @$"
    private {propertyType} {field}{(property.DefaultValueExpression is null ? "" : $" = {property.DefaultValueExpression}")};

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public {propertyType} {propertyName}
    {{
        get => {field};
        set
        {{
            var oldValue = {field};
            {field} = value;
            {changeExpression}
        }}
    }}";
    }
}
