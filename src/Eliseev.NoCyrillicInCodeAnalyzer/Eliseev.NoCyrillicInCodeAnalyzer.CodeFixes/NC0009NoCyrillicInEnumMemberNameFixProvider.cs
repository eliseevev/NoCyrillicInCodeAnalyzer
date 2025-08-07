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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NC0009NoCyrillicInEnumMemberNameFixProvider)), Shared]
    public class NC0009NoCyrillicInEnumMemberNameFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NC0009NoCyrillicInEnumMemberName.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<EnumMemberDeclarationSyntax>()
                .FirstOrDefault();

            if (declaration == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.NC0010CodeFixTitle,
                    createChangedSolution: c => ReplaceBasicCyrillicWithLatin(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.NC0010CodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatin(Document document, EnumMemberDeclarationSyntax enumMemberDecl, CancellationToken cancellationToken)
        {
            var identifierToken = enumMemberDecl.Identifier;
            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(identifierToken.Text);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var enumMemberSymbol = semanticModel.GetDeclaredSymbol(enumMemberDecl, cancellationToken);

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(document.Project.Solution, enumMemberSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }
    }
}