using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Core.Analyzer.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MustOverrideAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "MUSTOVERRIDE001";
    
    private static readonly LocalizableString Title = "Must override method";
    private static readonly LocalizableString MessageFormat = "Method '{0}' must be overridden in derived class";
    private static readonly LocalizableString Description = "Methods marked with [MustOverride] must be overridden.";
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId, Title, MessageFormat, Category, 
        DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
        
        if (namedTypeSymbol.BaseType == null || namedTypeSymbol.BaseType.SpecialType == SpecialType.System_Object)
            return;

        var mustOverrideMethods = GetMustOverrideMethods(namedTypeSymbol.BaseType);

        foreach (var mustOverrideMethod in mustOverrideMethods)
        {
            if (IsMethodOverridden(namedTypeSymbol, mustOverrideMethod)) 
                continue;
            
            var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], mustOverrideMethod.Name);
                
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static ImmutableArray<IMethodSymbol> GetMustOverrideMethods(INamedTypeSymbol baseType)
    {
        var methods = ImmutableArray.CreateBuilder<IMethodSymbol>();
        
        foreach (var member in baseType.GetMembers())
        {
            if (member is IMethodSymbol { IsVirtual: true, IsAbstract: false } method && HasMustOverrideAttribute(method))
            {
                methods.Add(method);
            }
        }

        if (baseType.BaseType != null && baseType.BaseType.SpecialType != SpecialType.System_Object)
        {
            methods.AddRange(GetMustOverrideMethods(baseType.BaseType));
        }

        return methods.ToImmutable();
    }

    private static bool HasMustOverrideAttribute(IMethodSymbol method)
    {
        foreach (var attribute in method.GetAttributes())
        {
            if (attribute.AttributeClass?.Name is "MustOverrideAttribute" or "MustOverride")
            {
                return true;
            }
        }
        
        return false;
    }

    private static bool IsMethodOverridden(INamedTypeSymbol currentType, IMethodSymbol baseMethod)
    {
        foreach (var member in currentType.GetMembers())
        {
            if (member is IMethodSymbol { IsOverride: true } method && method.OverriddenMethod?.Equals(baseMethod, SymbolEqualityComparer.Default) == true)
            {
                return true;
            }
        }
        
        return false;
    }
}