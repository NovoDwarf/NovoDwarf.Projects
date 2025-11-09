using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Analyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Core.Analyzer.Providers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MustOverrideCodeFixProvider))]
public class MustOverrideCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds 
        => ImmutableArray.Create(MustOverrideAnalyzer.DiagnosticId);
    
    public sealed override FixAllProvider GetFixAllProvider() 
        => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var syntaxNode = root?.FindToken(diagnosticSpan.Start).Parent;
        
        if (syntaxNode != null)
        {
            var declaration = syntaxNode.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Override must-override method",
                    createChangedSolution: c => OverrideMethodAsync(context.Document, declaration, diagnostic, c),
                    equivalenceKey: "OverrideMustOverrideMethod"),
                diagnostic);
        }
    }

    private async Task<Solution> OverrideMethodAsync(Document document, 
        ClassDeclarationSyntax classDecl, 
        Diagnostic diagnostic, 
        CancellationToken cancellationToken)
    {
        var methodName = diagnostic.Properties["methodName"] ?? (diagnostic.Properties.TryGetValue("methodName", out var property) ? property : "UnknownMethod");

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        var symbol = semanticModel?.GetDeclaredSymbol(classDecl, cancellationToken);

        if (symbol == null || methodName == null) 
            return document.Project.Solution;

        var baseMethod = FindBaseMethod(symbol, methodName);

        if (baseMethod == null) 
            return document.Project.Solution;
                
        var methodSyntax = GenerateOverrideMethod(baseMethod);
        var newClassDecl = classDecl.AddMembers(methodSyntax)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root.ReplaceNode(classDecl, newClassDecl);

        return newRoot != null 
            ? document.WithSyntaxRoot(newRoot).Project.Solution 
            : document.Project.Solution;
    }

    private IMethodSymbol? FindBaseMethod(INamedTypeSymbol classSymbol, string methodName)
    {
        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            var method = baseType.GetMembers(methodName)
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m => m.IsVirtual && !m.IsAbstract);
            
            if (method != null) 
                return method;
            
            baseType = baseType.BaseType;
        }
        
        return null;
    }

    private MethodDeclarationSyntax GenerateOverrideMethod(IMethodSymbol methodSymbol)
    {
        var parameters = methodSymbol.Parameters.Select(p =>
            SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name))
                .WithType(SyntaxFactory.ParseTypeName(p.Type.ToDisplayString())))
            .ToArray();
        
        var baseCall = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.BaseExpression(),
                SyntaxFactory.IdentifierName(methodSymbol.Name)));

        if (parameters.Any())
        {
            baseCall = baseCall.WithArgumentList(
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(parameters.Select(p =>
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Identifier))))));
        }

        var body = methodSymbol.ReturnsVoid 
            ? SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(baseCall))
            : SyntaxFactory.Block(SyntaxFactory.ReturnStatement(baseCall));

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName(methodSymbol.ReturnType.ToDisplayString()),
                SyntaxFactory.Identifier(methodSymbol.Name))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                  SyntaxFactory.Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)))
            .WithBody(body)
            .WithAdditionalAnnotations(Formatter.Annotation);
    }
}