using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0006NoCyrillicInLocalName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0006NoCyrillicInLocalNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0006NoCyrillicInLocalNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass
{
    void Method()
    {
        int localVar = 0;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
class TestClass
{
    void Method()
    {
        int {|#0:ïïï|} = 0;
    }
}";
            var fixtest = @"
class TestClass
{
    void Method()
    {
        int ppp = 0;
    }
}";
            var expected = VerifyCS.Diagnostic("NC0006")
                                   .WithLocation(0)
                                   .WithArguments("ïïï");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}