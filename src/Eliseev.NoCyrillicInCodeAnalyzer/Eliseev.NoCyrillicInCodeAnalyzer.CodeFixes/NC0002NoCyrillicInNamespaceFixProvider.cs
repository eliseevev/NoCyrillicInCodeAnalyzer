using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NC0002NoCyrillicInNamespaceFixProvider)), Shared]
    public class NC0002NoCyrillicInNamespaceFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(NC0002NoCyrillicInTypeNamespace.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<NamespaceDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.NC0001CodeFixTitle,
                    createChangedSolution: c => ReplaceBasicCyrillicWithLatinForNamespace(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.NC0001CodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatinForNamespace(
            Document document,
            NamespaceDeclarationSyntax namespaceDecl,
            CancellationToken cancellationToken)
        {
            var nameSyntax = namespaceDecl.Name;
            var nameText = nameSyntax.ToString();

            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(nameText);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var namespaceSymbol = semanticModel.GetDeclaredSymbol(namespaceDecl, cancellationToken);

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(originalSolution, namespaceSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }
    }
}
