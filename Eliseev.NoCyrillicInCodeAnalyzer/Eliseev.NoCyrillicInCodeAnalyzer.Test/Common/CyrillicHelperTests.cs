using Eliseev.NoCyrillicInCodeAnalyzer.Common;
using Xunit;
namespace Eliseev.NoCyrillicInCodeAnalyzer.Test.Common
{
    public class CyrillicHelperTests
    {
        [Theory]
        [InlineData("Привет", true)]
        [InlineData("Test123", false)]
        [InlineData("Hello, мир!", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ContainsBasicCyrillic_WorksCorrectly(string input, bool expected)
        {
            var result = CyrillicHelper.ContainsBasicCyrillic(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Привет", "Privet")]
        [InlineData("Test", "Test")]
        [InlineData("123", "123")]
        [InlineData("Привет, мир!", "Privet, mir!")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void ReplaceBasicCyrillicWithLatin_WorksCorrectly(string input, string expected)
        {
            var result = CyrillicHelper.ReplaceBasicCyrillicWithLatin(input);
            Assert.Equal(expected, result);
        }
    }
}
