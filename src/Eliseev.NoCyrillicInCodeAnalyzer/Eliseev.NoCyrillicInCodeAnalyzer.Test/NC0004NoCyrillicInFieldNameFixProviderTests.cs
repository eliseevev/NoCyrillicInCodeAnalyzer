using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0004NoCyrillicInFieldName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0004NoCyrillicInFieldNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0004NoCyrillicInFieldNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass
{
    private int fieldName;
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
class TestClass
{
    private int {|#0:пoле|};
}";
            var fixtest = @"
class TestClass
{
    private int pole;
}";
            var expected = VerifyCS.Diagnostic("NC0004")
                                   .WithLocation(0)
                                   .WithArguments("пoле");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}