using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NC0007NoCyrillicInParameterName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC0007";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NC0007AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NC0007AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NC0007AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/eliseevev/NoCyrillicInCodeAnalyzer/blob/master/documentation/rules/NC0007.md"
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Parameter);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var parameterSymbol = (IParameterSymbol)context.Symbol;

            if (CyrillicHelper.ContainsBasicCyrillic(parameterSymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, parameterSymbol.Locations[0], parameterSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}