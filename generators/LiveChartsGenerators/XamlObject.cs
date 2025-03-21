using Microsoft.CodeAnalysis;

namespace LiveChartsGenerators;

public readonly record struct XamlObject
{
    public readonly string NameSpace;
    public readonly string Name;
    public readonly INamedTypeSymbol Type;
    public readonly ITypeSymbol BasedOn;
    public readonly List<IPropertySymbol> BindableProperties;
    public readonly List<IPropertySymbol> NotBindableProperties;
    public readonly List<IEventSymbol> Events;
    public readonly List<IMethodSymbol> Methods;
    public readonly List<IMethodSymbol> ExplicitMethods;
    public readonly string? FileHeader;

    public XamlObject(
        string nameSpace,
        string name,
        INamedTypeSymbol type,
        ITypeSymbol basedOn,
        List<IPropertySymbol> bindableProperties,
        List<IPropertySymbol> notBindableProperites,
        List<IEventSymbol> events,
        List<IMethodSymbol> methods,
        List<IMethodSymbol> explicitMethods,
        string? fileHeader)
    {
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
    }
}
