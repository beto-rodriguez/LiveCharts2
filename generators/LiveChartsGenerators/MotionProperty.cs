using Microsoft.CodeAnalysis;

namespace LiveChartsGenerators;

public readonly record struct MotionProperty
{
    public readonly IPropertySymbol Property;
    public readonly bool HasExplicitAcessors;

    public MotionProperty(IPropertySymbol symbol, bool hasExplicitAcessors)
    {
        Property = symbol;
        HasExplicitAcessors = hasExplicitAcessors;
    }
}
