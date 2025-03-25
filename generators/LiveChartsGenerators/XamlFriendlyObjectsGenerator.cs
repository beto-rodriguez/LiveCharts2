// based on:
// https://github.com/andrewlock/blog-examples/tree/master/NetEscapades.EnumGenerators

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace LiveChartsGenerators;

[Generator]
public class XamlFriendlyObjectsGenerator : IIncrementalGenerator
{
    private const string XamlAttribute = "LiveChartsCore.Generators.XamlClassAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx => ctx.AddSource(
            "XamlClassAttribute.g.cs", SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        var enumsToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                XamlAttribute,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetXamlObjectsToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(enumsToGenerate,
            static (spc, source) => Execute(source, spc));
    }

    private static void Execute(XamlObject? enumToGenerate, SourceProductionContext context)
    {
        if (enumToGenerate is { } value)
        {
            // generate the source code and add it to the output
            var result = SourceGenerationHelper.GenerateXamlObject(value);
            context.AddSource(
                $"{value.Name}.g.cs".Replace('<', '_').Replace('>', '_'),
                SourceText.From(result, Encoding.UTF8));
        }
    }

    private static XamlObject? GetXamlObjectsToGenerate(SemanticModel semanticModel, SyntaxNode declarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(declarationSyntax) is not INamedTypeSymbol symbol)
            return null; // something went wrong

        var bindableProperties = new List<IPropertySymbol>();
        var notBindableProperties = new List<IPropertySymbol>();
        var events = new List<IEventSymbol>();
        var methods = new Dictionary<string, IMethodSymbol>();
        var explicitMethods = new Dictionary<string, IMethodSymbol>();

        var targetAttribute = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == XamlAttribute);
        if (targetAttribute is null) return null;

        if (targetAttribute.ConstructorArguments.FirstOrDefault().Value is not ITypeSymbol baseType)
            return null;

        string? fileHeader = null;
        string? propertyChangeHandlers = null;
        string? overridenTypes = null;

        foreach (var arg in targetAttribute.NamedArguments)
        {
            switch (arg.Key)
            {
                case "FileHeader":
                    fileHeader = arg.Value.Value as string;
                    break;
                case "PropertyChangeHandlers":
                    propertyChangeHandlers = arg.Value.Value as string;
                    break;
                case "PropertyTypeOverride":
                    overridenTypes = arg.Value.Value as string;
                    break;
                default:
                    break;
            }
        }

        var ns = symbol.ContainingNamespace.ToString();

        // use this method to get also the generic argument name.
        var nameParts = symbol.ToDisplayString().Split('.');
        var name = nameParts[nameParts.Length - 1];

        foreach (var member in GetLiveChartsMembers(baseType))
        {
            if (member is IPropertySymbol property)
            {
                // ignore the DefaultValues property, it is for internal use only
                if (property.Name == "DefaultValues")
                    continue;

                if (IsBindable(property))
                    bindableProperties.Add(property);
                else
                    notBindableProperties.Add(property);
            }

            if (member is IMethodSymbol method)
            {
                // the key is the method name, this is to avoid duplicated methods
                var methodDisplayString = method.Name;
                var methodParts = method.ToDisplayString().Split('(');
                var methodKey = $"{method.Name}({methodParts[methodParts.Length - 1]}";

                if (method.MethodKind == MethodKind.Ordinary && method.DeclaredAccessibility == Accessibility.Public)
                    methods[methodKey] = method;

                if (method.MethodKind == MethodKind.ExplicitInterfaceImplementation)
                    explicitMethods[methodKey] = method;
            }

            if (member is IEventSymbol @event)
            {
                // normally, the property changed event is already implemented in XAML objects
                // for now we just ignore it...
                //if (@event.Name == "PropertyChanged")
                //    continue;
                events.Add(@event);
            }
        }

        return new XamlObject(
            ns, name, symbol, baseType, bindableProperties, notBindableProperties, events,
            [.. methods.Values], [.. explicitMethods.Values], fileHeader, propertyChangeHandlers, overridenTypes);
    }

    private static List<ISymbol> GetLiveChartsMembers(ITypeSymbol typeSymbol)
    {
        var allMembers = new List<ISymbol>();

        while (typeSymbol is not null)
        {
            allMembers.AddRange(typeSymbol.GetMembers());

            if (typeSymbol.OriginalDefinition.BaseType is null) break;
            typeSymbol = typeSymbol.OriginalDefinition.BaseType;

            if (!typeSymbol.ContainingNamespace.ToDisplayString().StartsWith("LiveChartsCore"))
                break;
        }

        return allMembers;
    }

    private static bool IsBindable(IPropertySymbol property)
    {
        // a property will be bindable when:
        return
            // 1. it is not explicitly implemented
            !property.Name.Contains('.') &&
            // 2. it has a setter.
            property.SetMethod is not null;
    }
}
