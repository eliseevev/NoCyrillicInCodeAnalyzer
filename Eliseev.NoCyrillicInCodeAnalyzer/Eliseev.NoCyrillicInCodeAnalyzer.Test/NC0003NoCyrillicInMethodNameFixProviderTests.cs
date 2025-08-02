using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0003NoCyrillicInMethodName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0003NoCyrillicInMethodNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0003NoCyrillicInMethodNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass
{
    void TestMethod() { }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
class TestClass
{
    void {|#0:ТестМетод|}() { }
}";
            var fixtest = @"
class TestClass
{
    void TestMetod() { }
}";
            var expected = VerifyCS.Diagnostic("NC0003")
                                   .WithLocation(0)
                                   .WithArguments("ТестМетод");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}