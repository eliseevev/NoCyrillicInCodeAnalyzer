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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NC0001NoCyrillicInTypeNameFixProvider)), Shared]
    public class NC0001NoCyrillicInTypeNameFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(NC0001NoCyrillicInTypeName.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.NC0001CodeFixTitle,
                    createChangedSolution: c => ReplaceBasicCyrillicWithLatin(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.NC0001CodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatin(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            var identifierToken = typeDecl.Identifier;
            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(identifierToken.Text);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }
    }
}
