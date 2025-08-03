using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0008NoCyrillicInTypeParameterName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0008NoCyrillicInTypeParameterNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0008NoCyrillicInTypeParameterNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass<TParam>
{
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
class TestClass<{|#0:Параметр|}>
{
}";
            var fixtest = @"
class TestClass<Parametr>
{
}";
            var expected = VerifyCS.Diagnostic("NC0008")
                                   .WithLocation(0)
                                   .WithArguments("Параметр");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}