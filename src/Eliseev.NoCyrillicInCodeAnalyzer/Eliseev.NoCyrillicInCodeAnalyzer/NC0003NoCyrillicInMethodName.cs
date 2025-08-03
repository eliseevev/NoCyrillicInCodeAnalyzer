using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NC0003NoCyrillicInMethodName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC0003";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NC0003AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NC0003AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NC0003AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/eliseevev/NoCyrillicInCodeAnalyzer/blob/master/documentation/rules/NC0003.md"
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            if (CyrillicHelper.ContainsBasicCyrillic(methodSymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}