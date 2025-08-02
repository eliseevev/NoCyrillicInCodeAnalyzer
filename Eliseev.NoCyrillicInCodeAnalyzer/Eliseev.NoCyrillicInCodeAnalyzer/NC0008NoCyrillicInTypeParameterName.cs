using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NC0008NoCyrillicInTypeParameterName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC0008";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NC0008AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NC0008AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NC0007AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeTypeParameter, Microsoft.CodeAnalysis.CSharp.SyntaxKind.TypeParameter);
        }

        private static void AnalyzeTypeParameter(SyntaxNodeAnalysisContext context)
        {
            var typeParam = (Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax)context.Node;
            var name = typeParam.Identifier.Text;

            if (CyrillicHelper.ContainsBasicCyrillic(name))
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(typeParam, context.CancellationToken);
                if (symbol is ITypeParameterSymbol)
                {
                    var diagnostic = Diagnostic.Create(Rule, typeParam.Identifier.GetLocation(), name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}