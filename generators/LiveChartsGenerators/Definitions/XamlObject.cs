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
    public readonly Dictionary<string, string> PropertyChangedMap = [];
    public readonly Dictionary<string, string> OverridenTypes = [];

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
        string? propertyChangedMap,
        string? overridenTypes)
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
    }
}
