using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NC0010NoCyrillicInEventName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC0010";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NC0010AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NC0010AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NC0010AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDecl = (EventDeclarationSyntax)context.Node;
            var name = eventDecl.Identifier.Text;

            if (CyrillicHelper.ContainsBasicCyrillic(name))
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(eventDecl, context.CancellationToken);
                if (symbol is IEventSymbol)
                {
                    var diagnostic = Diagnostic.Create(Rule, eventDecl.Identifier.GetLocation(), name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDecl = (EventFieldDeclarationSyntax)context.Node;
            foreach (var variable in eventFieldDecl.Declaration.Variables)
            {
                var name = variable.Identifier.Text;
                if (CyrillicHelper.ContainsBasicCyrillic(name))
                {
                    var symbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken);
                    if (symbol is IEventSymbol)
                    {
                        var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}