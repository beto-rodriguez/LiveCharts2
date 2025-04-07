// based on:
// https://github.com/andrewlock/blog-examples/tree/master/NetEscapades.EnumGenerators

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace LiveChartsGenerators;

[Generator]
public class XamlFriendlyObjectsGenerator : IIncrementalGenerator
{
    private const string XamlAttribute = "LiveChartsCore.Generators.XamlClassAttribute";
    private const string MotionPropertyAttribute = "LiveChartsCore.Generators.MotionPropertyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx => ctx.AddSource(
            "Assets.g.cs", SourceText.From(AssetsTemplates.GetTemplate(), Encoding.UTF8)));

        var xamlObjects = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                XamlAttribute,
                predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetXamlObjectsToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(xamlObjects, static (spc, source) => ExecuteXamlObjects(source, spc));

        var motionProperties = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                MotionPropertyAttribute,
                predicate: static (syntaxNode, _) => syntaxNode is PropertyDeclarationSyntax,
                transform: static (ctx, _) => GetMotionPropertiesToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(motionProperties, static (spc, source) => ExecuteMotionProperties(source, spc));
    }

    private static void ExecuteXamlObjects(XamlObject? target, SourceProductionContext context)
    {
        if (target is { } value)
        {
            // generate the source code and add it to the output
            var result = XamlObjectTempaltes.GetTemplate(value);
            context.AddSource($"{value.Name}.g.cs".Replace('<', '_').Replace('>', '_'), SourceText.From(result, Encoding.UTF8));
        }
    }

    private static void ExecuteMotionProperties(MotionProperty? target, SourceProductionContext context)
    {
        if (target is { } value)
        {
            // generate the source code and add it to the output
            var result = MotionPropertyTemplates.GetTemplate(value);
            context.AddSource(
                $"{value.Property.ContainingType.Name}.{value.Property.Name}.g.cs".Replace('<', '_').Replace('>', '_'),
                SourceText.From(result, Encoding.UTF8));
        }
    }

    private static XamlObject? GetXamlObjectsToGenerate(SemanticModel semanticModel, SyntaxNode declarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(declarationSyntax) is not INamedTypeSymbol symbol)
            return null; // something went wrong

        var bindablePropertiesDic = new Dictionary<string, List<IPropertySymbol>>();
        var notBindableProperties = new Dictionary<string, IPropertySymbol>();
        var events = new List<IEventSymbol>();
        var methods = new Dictionary<string, IMethodSymbol>();
        var explicitMethods = new Dictionary<string, IMethodSymbol>();

        var targetAttribute = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == XamlAttribute);
        if (targetAttribute is null) return null;

        if (targetAttribute.ConstructorArguments.FirstOrDefault().Value is not ITypeSymbol baseType)
            return null;

        string? fileHeader = null;
        string? propertyChangeMap = null;
        string? overridenTypes = null;
        ITypeSymbol? alsoMap = null;
        string? alsoMapPath = null;
        var generateBaseTypeDeclaration = true;

        foreach (var arg in targetAttribute.NamedArguments)
        {
            switch (arg.Key)
            {
                case "FileHeader":
                    fileHeader = arg.Value.Value as string;
                    break;
                case "PropertyChangeMap":
                    propertyChangeMap = arg.Value.Value as string;
                    break;
                case "PropertyTypeOverride":
                    overridenTypes = arg.Value.Value as string;
                    break;
                case "Map":
                    alsoMap = arg.Value.Value as ITypeSymbol;
                    break;
                case "MapPath":
                    alsoMapPath = arg.Value.Value as string;
                    break;
                case "GenerateBaseTypeDeclaration":
                    generateBaseTypeDeclaration = ((bool?)arg.Value.Value) ?? true;
                    break;
                default:
                    break;
            }
        }

        var ns = symbol.ContainingNamespace.ToString();

        // use this method to get also the generic argument name.
        var nameParts = symbol.ToDisplayString().Split('.');
        var name = nameParts[nameParts.Length - 1];

        var members = new Dictionary<string, List<ISymbol>>
        {
            [string.Empty] = GetLiveChartsMembers(baseType)
        };

        if (alsoMap is not null && alsoMapPath is not null)
            members[alsoMapPath] = GetLiveChartsMembers(alsoMap);

        foreach (var pair in members)
        {
            var bindableProperties = new List<IPropertySymbol>();
            bindablePropertiesDic[pair.Key] = bindableProperties;

            foreach (var member in pair.Value)
            {
                if (member is IPropertySymbol property)
                {
                    if (property.DeclaredAccessibility == Accessibility.Protected || property.IsStatic)
                        continue;

                    var notExplicit = property.ExplicitInterfaceImplementations.Length == 0;
                    var hasSetter = property.SetMethod is not null;
                    var isPublic = property.DeclaredAccessibility == Accessibility.Public;

                    if (notExplicit && hasSetter && isPublic)
                    {
                        bindableProperties.Add(property);
                    }
                    else if (pair.Key == string.Empty)
                    {
                        // when key is empty, we are on the base type, we only map regular properties from the base type
                        var propertyKey = property.Name;
                        notBindableProperties[propertyKey] = property;
                    }
                }

                // ignore methods and events when not in the base type
                if (pair.Key != string.Empty) continue;

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
        }

        return new XamlObject(
            generateBaseTypeDeclaration, ns, name, symbol, baseType, bindablePropertiesDic, [.. notBindableProperties.Values], events,
            [.. methods.Values], [.. explicitMethods.Values], fileHeader, propertyChangeMap, overridenTypes);
    }

    private static MotionProperty? GetMotionPropertiesToGenerate(SemanticModel semanticModel, SyntaxNode declarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(declarationSyntax) is not IPropertySymbol symbol)
            return null; // something went wrong

        var targetAttribute = symbol.GetAttributes()
           .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MotionPropertyAttribute);
        if (targetAttribute is null) return null;

        var hasExplicitAcessors = false;

        foreach (var arg in targetAttribute.NamedArguments)
        {
            switch (arg.Key)
            {
                case "HasExplicitAcessors":
                    hasExplicitAcessors = ((bool?)arg.Value.Value) ?? true;
                    break;
                default:
                    break;
            }
        }

        return new(symbol, hasExplicitAcessors);
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
}
