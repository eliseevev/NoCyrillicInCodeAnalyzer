using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0007NoCyrillicInParameterName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0007NoCyrillicInParameterNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0007NoCyrillicInParameterNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass
{
    void Method(int param) { }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
class TestClass
{
    void Method(int {|#0:параметр|}) { }
}";
            var fixtest = @"
class TestClass
{
    void Method(int parametr) { }
}";
            var expected = VerifyCS.Diagnostic("NC0007")
                                   .WithLocation(0)
                                   .WithArguments("параметр");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}