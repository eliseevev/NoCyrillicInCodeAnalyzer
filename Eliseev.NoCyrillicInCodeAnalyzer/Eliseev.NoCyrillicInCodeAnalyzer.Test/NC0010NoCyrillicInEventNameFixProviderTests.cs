using System.Threading.Tasks;
using Xunit;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0010NoCyrillicInEventName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0010NoCyrillicInEventNameFixProvider>;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0010NoCyrillicInEventNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"
class TestClass
{
    public event System.EventHandler EventName;
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered_AutoEvent()
        {
            var test = @"
class TestClass
{
    public event System.EventHandler {|#0:adsâàûôæææ|};
}";
            var fixtest = @"
class TestClass
{
    public event System.EventHandler adsvayfzhzhzh;
}";
            var expected = VerifyCS.Diagnostic(NC0010NoCyrillicInEventName.DiagnosticId)
                                   .WithLocation(0)
                                   .WithArguments("adsâàûôæææ");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}