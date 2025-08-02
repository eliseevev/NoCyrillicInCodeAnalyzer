using System.Threading.Tasks;
using VerifyCS = Eliseev.NoCyrillicInCodeAnalyzer.Test.CSharpCodeFixVerifier<
    Eliseev.NoCyrillicInCodeAnalyzer.NC0001NoCyrillicInTypeName,
    Eliseev.NoCyrillicInCodeAnalyzer.NC0001NoCyrillicInTypeNameFixProvider>;
using Xunit;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Test
{
    public class NC0001NoCyrillicInTypeNameFixProviderTests
    {
        [Fact]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DiagnosticAndCodeFixTriggered()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TyпeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("NC0001")
                                   .WithLocation(0)
                                   .WithArguments("TyпeName");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}