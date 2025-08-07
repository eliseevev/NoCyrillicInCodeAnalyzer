using System.Collections.Generic;
using System.Text;

namespace Eliseev.NoCyrillicInCodeAnalyzer.Common
{
    internal static class CyrillicHelper
    {
        private static readonly Dictionary<char, string> _basicCyrillicToLatinMap = new Dictionary<char, string>
        {
            ['А'] = "A",
            ['а'] = "a",
            ['Б'] = "B",
            ['б'] = "b",
            ['В'] = "V",
            ['в'] = "v",
            ['Г'] = "G",
            ['г'] = "g",
            ['Д'] = "D",
            ['д'] = "d",
            ['Е'] = "E",
            ['е'] = "e",
            ['Ё'] = "E",
            ['ё'] = "e",
            ['Ж'] = "Zh",
            ['ж'] = "zh",
            ['З'] = "Z",
            ['з'] = "z",
            ['И'] = "I",
            ['и'] = "i",
            ['Й'] = "Y",
            ['й'] = "y",
            ['К'] = "K",
            ['к'] = "k",
            ['Л'] = "L",
            ['л'] = "l",
            ['М'] = "M",
            ['м'] = "m",
            ['Н'] = "N",
            ['н'] = "n",
            ['О'] = "O",
            ['о'] = "o",
            ['П'] = "P",
            ['п'] = "p",
            ['Р'] = "R",
            ['р'] = "r",
            ['С'] = "S",
            ['с'] = "s",
            ['Т'] = "T",
            ['т'] = "t",
            ['У'] = "U",
            ['у'] = "u",
            ['Ф'] = "F",
            ['ф'] = "f",
            ['Х'] = "Kh",
            ['х'] = "kh",
            ['Ц'] = "Ts",
            ['ц'] = "ts",
            ['Ч'] = "Ch",
            ['ч'] = "ch",
            ['Ш'] = "Sh",
            ['ш'] = "sh",
            ['Щ'] = "Shch",
            ['щ'] = "shch",
            ['Ы'] = "Y",
            ['ы'] = "y",
            ['Э'] = "E",
            ['э'] = "e",
            ['Ю'] = "Yu",
            ['ю'] = "yu",
            ['Я'] = "Ya",
            ['я'] = "ya",
            ['Ъ'] = "",
            ['ъ'] = "",
            ['Ь'] = "",
            ['ь'] = ""
        };

        public static bool ContainsBasicCyrillic(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            foreach (char c in input)
            {
                if (c >= '\u0400' && c <= '\u04FF')
                    return true;
            }

            return false;
        }

        public static string ReplaceBasicCyrillicWithLatin(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder(input.Length * 2);

            foreach (char c in input)
            {
                if (c >= '\u0400' && c <= '\u04FF' && _basicCyrillicToLatinMap.TryGetValue(c, out string replacement))
                    sb.Append(replacement);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
