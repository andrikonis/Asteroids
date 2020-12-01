using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Bullshitnalazer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BullshitnalazerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "INLINE_IF";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Style";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.IfStatement);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            AnalyzeIfNode(node, context);
        }

        private static void AnalyzeIfNode(IfStatementSyntax node, SyntaxNodeAnalysisContext context)
        {
            if (node.Statement is ExpressionStatementSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, node.Statement.GetLocation()));
            }

            if (node.Else is object)
            {
                if (node.Else.Statement is IfStatementSyntax ifNode)
                {
                    AnalyzeIfNode(ifNode, context);
                }
                else if (node.Else.Statement is ExpressionStatementSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, node.Else.Statement.GetLocation()));
                }
            }
        }
    }
}
