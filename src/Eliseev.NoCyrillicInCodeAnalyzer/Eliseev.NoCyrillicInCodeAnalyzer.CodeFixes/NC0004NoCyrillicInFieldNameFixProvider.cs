using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NC0004NoCyrillicInFieldNameFixProvider)), Shared]
    public class NC0004NoCyrillicInFieldNameFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NC0004NoCyrillicInFieldName.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault();

            if (declaration == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.NC0004CodeFixTitle,
                    createChangedSolution: c => ReplaceBasicCyrillicWithLatin(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.NC0004CodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatin(Document document, VariableDeclaratorSyntax fieldDecl, CancellationToken cancellationToken)
        {
            var identifierToken = fieldDecl.Identifier;
            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(identifierToken.Text);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var fieldSymbol = semanticModel.GetDeclaredSymbol(fieldDecl, cancellationToken);

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(document.Project.Solution, fieldSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }
    }
}