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

public readonly record struct XamlObject
{
    public readonly bool GenerateBaseTypeDeclaration;
    public readonly string NameSpace;
    public readonly string Name;
    public readonly INamedTypeSymbol Type;
    public readonly ITypeSymbol BasedOn;
    public readonly Dictionary<string, List<IPropertySymbol>> BindableProperties;
    public readonly List<IPropertySymbol> NotBindableProperties;
    public readonly List<IEventSymbol> Events;
    public readonly List<IMethodSymbol> Methods;
    public readonly List<IMethodSymbol> ExplicitMethods;
    public readonly string? FileHeader;
    public readonly bool ManualOnPropertyChanged = false;
    public readonly Dictionary<string, string> PropertyChangedMap = [];
    public readonly Dictionary<string, string> OverridenTypes = [];
    public readonly Dictionary<string, string> OverridenNames = [];
    public readonly bool IsSeries = false;
    public readonly Dictionary<string, string> SeriesArgs = [];

    public XamlObject(
        bool generateBaseTypeDeclaration,
        string nameSpace,
        string name,
        INamedTypeSymbol type,
        ITypeSymbol basedOn,
        Dictionary<string, List<IPropertySymbol>> bindableProperties,
        List<IPropertySymbol> notBindableProperites,
        List<IEventSymbol> events,
        List<IMethodSymbol> methods,
        List<IMethodSymbol> explicitMethods,
        string? fileHeader,
        bool manualOnPropertyChanged,
        string? propertyChangedMap,
        string? overridenTypes,
        string? overridenNames,
        ITypeSymbol? tModel,
        ITypeSymbol? tVisual,
        ITypeSymbol? tLabel)
    {
        GenerateBaseTypeDeclaration = generateBaseTypeDeclaration;
        NameSpace = nameSpace;
        Name = name;
        Type = type;
        BasedOn = basedOn;
        BindableProperties = bindableProperties;
        NotBindableProperties = notBindableProperites;
        Events = events;
        Methods = methods;
        ExplicitMethods = explicitMethods;
        FileHeader = fileHeader;
        ManualOnPropertyChanged = manualOnPropertyChanged;

        IsSeries = GetIsSeries(basedOn);
        if (IsSeries)
        {
            propertyChangedMap = "Values{=}ValuesMap";
            overridenTypes =
                "Values{=}object?{,}" +
                "DataLabelsFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>?{,}" +
                "ToolTipLabelFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>?{,}" +
                "XToolTipLabelFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>?{,}" +
                "YToolTipLabelFormatter{=}System.Func<LiveChartsCore.Kernel.ChartPoint, string>?";
            SeriesArgs["TModel"] = tModel?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                ?? "global::System.Object";
            if (tVisual is not null)
                SeriesArgs["TVisual"] = tVisual.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            SeriesArgs["TLabel"] = tLabel?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                ?? "global::LiveChartsCore.SkiaSharpView.Drawing.Geometries.LabelGeometry";
        }

        if (propertyChangedMap is not null)
        {
            var maps = propertyChangedMap.Split(["{,}"], StringSplitOptions.None);

            foreach (var map in maps)
            {
                var parts = map.Split(["{=}"], StringSplitOptions.None);
                PropertyChangedMap[parts[0]] = parts[1];
            }
        }

        if (overridenTypes is not null)
        {
            var types = overridenTypes.Split(["{,}"], StringSplitOptions.None);
            foreach (var t in types)
            {
                var parts = t.Split(["{=}"], StringSplitOptions.None);
                OverridenTypes[parts[0]] = parts[1];
            }
        }

        // by default, do not allow the Name property,
        // it is reserved by some XAML frameworks

        var baseName = basedOn.Name;
        if (baseName.EndsWith("Series")) baseName = "Series";
        overridenNames ??= $"Name{{=}}{baseName}Name";
        var names = overridenNames.Split(["{,}"], StringSplitOptions.None);
        foreach (var n in names)
        {
            var parts = n.Split(["{=}"], StringSplitOptions.None);
            OverridenNames[parts[0]] = parts[1];
        }
    }

    public static bool GetIsSeries(ITypeSymbol typeSymbol)
    {
        // Check direct interface implementation
        if (typeSymbol.OriginalDefinition.AllInterfaces.Any(x => x.ToDisplayString() == "LiveChartsCore.ISeries"))
            return true;

        // Traverse the inheritance chain
        var baseType = typeSymbol.OriginalDefinition.BaseType;
        while (baseType != null)
        {
            if (baseType.AllInterfaces.Any(x => x.ToDisplayString() == "LiveChartsCore.ISeries"))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

}
