using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NC0006NoCyrillicInLocalName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC0006";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NC0006AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NC0006AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NC0006AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeVariable, Microsoft.CodeAnalysis.CSharp.SyntaxKind.VariableDeclarator);
        }

        private static void AnalyzeVariable(SyntaxNodeAnalysisContext context)
        {
            var variableDeclarator = (Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax)context.Node;
            var name = variableDeclarator.Identifier.Text;

            if (CyrillicHelper.ContainsBasicCyrillic(name))
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(variableDeclarator, context.CancellationToken);
                if (symbol is ILocalSymbol)
                {
                    var diagnostic = Diagnostic.Create(Rule, variableDeclarator.Identifier.GetLocation(), name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}