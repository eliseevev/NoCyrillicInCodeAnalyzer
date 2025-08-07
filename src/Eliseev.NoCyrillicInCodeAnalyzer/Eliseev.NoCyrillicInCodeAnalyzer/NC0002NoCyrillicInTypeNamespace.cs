using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NC0002NoCyrillicInTypeNamespace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC0002";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NC0002AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NC0002AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NC0002AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/eliseevev/NoCyrillicInCodeAnalyzer/blob/master/documentation/rules/NC0002.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamespaceSymbol)context.Symbol;

            if (ContainsCyrillic(namedTypeSymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool ContainsCyrillic(string input)
        {
            return CyrillicHelper.ContainsBasicCyrillic(input);
        }
    }
}
