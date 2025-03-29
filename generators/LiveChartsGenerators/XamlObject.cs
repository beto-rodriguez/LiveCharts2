using Microsoft.CodeAnalysis;

namespace LiveChartsGenerators;

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
