using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Eliseev.NoCyrillicInCodeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NC0008NoCyrillicInTypeParameterNameFixProvider)), Shared]
    public class NC0008NoCyrillicInTypeParameterNameFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NC0008NoCyrillicInTypeParameterName.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Ищем TypeParameterSyntax по позиции диагностики
            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<TypeParameterSyntax>()
                .FirstOrDefault();

            if (declaration == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.NC0008CodeFixTitle,
                    createChangedSolution: c => ReplaceBasicCyrillicWithLatin(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.NC0008CodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> ReplaceBasicCyrillicWithLatin(Document document, TypeParameterSyntax typeParamDecl, CancellationToken cancellationToken)
        {
            var identifierToken = typeParamDecl.Identifier;
            var newName = CyrillicHelper.ReplaceBasicCyrillicWithLatin(identifierToken.Text);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeParamSymbol = semanticModel.GetDeclaredSymbol(typeParamDecl, cancellationToken);

            if (typeParamSymbol == null)
                return document.Project.Solution;

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer
                .RenameSymbolAsync(document.Project.Solution, typeParamSymbol, newName, optionSet, cancellationToken)
                .ConfigureAwait(false);
            // Возвращаем измененное решение
            return newSolution;
        }
    }
}