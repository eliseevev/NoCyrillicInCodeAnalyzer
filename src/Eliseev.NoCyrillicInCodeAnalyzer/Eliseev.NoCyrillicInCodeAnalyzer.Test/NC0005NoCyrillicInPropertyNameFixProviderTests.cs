using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0005NoCyrillicInPropertyName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0005NoCyrillicInPropertyNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0005NoCyrillicInPropertyNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass
{
    public int PropertyName { get; set; }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
class TestClass
{
    public int {|#0:Свойство|} { get; set; }
}";
            var fixtest = @"
class TestClass
{
    public int Svoystvo { get; set; }
}";
            var expected = VerifyCS.Diagnostic("NC0005")
                                   .WithLocation(0)
                                   .WithArguments("Свойство");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}