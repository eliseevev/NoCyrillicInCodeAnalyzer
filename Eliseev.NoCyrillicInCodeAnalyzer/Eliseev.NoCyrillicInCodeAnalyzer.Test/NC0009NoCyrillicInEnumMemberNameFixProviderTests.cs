using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0009NoCyrillicInEnumMemberName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0009NoCyrillicInEnumMemberNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0009NoCyrillicInEnumMemberNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
enum TestEnum
{
    ValueOne,
    ValueTwo
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
enum TestEnum
{
    {|#0:ï|},
    ValueTwo
}";
            var fixtest = @"
enum TestEnum
{
    p,
    ValueTwo
}";
            var expected = VerifyCS.Diagnostic(NC0009NoCyrillicInEnumMemberName.DiagnosticId)
                                   .WithLocation(0)
                                   .WithArguments("ï");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}