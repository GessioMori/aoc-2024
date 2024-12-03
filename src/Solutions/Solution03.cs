using aoc_2024.Interfaces;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution03 : ISolution
    {
        public string RunPartA(string inputData)
        {
            MatchCollection matches = Regex.Matches(inputData, @"(mul\((?<val1>\d{1,3}),(?<val2>\d{1,3})\))");

            long total = 0;

            foreach (Match match in matches)
            {
                total += int.Parse(match.Groups["val1"].Value) * int.Parse(match.Groups["val2"].Value);
            }

            return total.ToString();
        }

        public string RunPartB(string inputData)
        {
            MatchCollection matches = Regex.Matches(inputData, @"(?<mul>mul\((?<val1>\d{1,3}),(?<val2>\d{1,3})\))|(?<n>don't\(\))|(?<y>do\(\))");
            long total = 0;
            bool isEnabled = true;

            foreach (Match match in matches)
            {
                if (match.Groups["mul"].Success && isEnabled)
                {
                    total += int.Parse(match.Groups["val1"].Value) * int.Parse(match.Groups["val2"].Value);
                }
                else if (match.Groups["n"].Success || match.Groups["y"].Success)
                {
                    isEnabled = match.Groups["y"].Success;
                }
            }

            return total.ToString();
        }
    }
}