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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NC0010NoCyrillicInEventNameFixProvider)), Shared]
    public class NC0010NoCyrillicInEventNameFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NC0010NoCyrillicInEventName.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // EventDeclarationSyntax (event ñ add/remove)
            var eventDecl = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<EventDeclarationSyntax>()
                .FirstOrDefault();
            if (eventDecl != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixResources.NC0010CodeFixTitle,
                        createChangedSolution: c => ReplaceBasicCyrillicWithLatin(context.Document, eventDecl, c),
                        equivalenceKey: nameof(CodeFixResources.NC0010CodeFixTitle)),
                    diagnostic);
                return;
            }

            // VariableDeclaratorSyntax (auto-event)
            var variable = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<VariableDeclaratorSyntax>()
                .FirstOrDefault();
            if (variable != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixResources.NC0010CodeFixTitle,
                        createChangedSolution: c => ReplaceBasicCyrillicWithLatin(context.Document, variable, c),
                        equivalenceKey: nameof(CodeFixResources.NC0010CodeFixTitle)),
                    diagnostic);
            }
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatin(Document document, EventDeclarationSyntax eventDecl, CancellationToken cancellationToken)
        {
            var identifierToken = eventDecl.Identifier;
            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(identifierToken.Text);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var eventSymbol = semanticModel.GetDeclaredSymbol(eventDecl, cancellationToken);

            if (eventSymbol == null)
                return document.Project.Solution;

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(document.Project.Solution, eventSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatin(Document document, VariableDeclaratorSyntax variable, CancellationToken cancellationToken)
        {
            var identifierToken = variable.Identifier;
            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(identifierToken.Text);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var eventSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

            if (eventSymbol == null)
                return document.Project.Solution;

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(document.Project.Solution, eventSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }
    }
}