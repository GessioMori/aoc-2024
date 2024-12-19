using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution19 : ISolution
    {
        public string RunPartA(string inputData)
        {
            (HashSet<string> patterns, List<string> designs) = ParseInput(inputData);

            Dictionary<string, bool> memo = [];

            int possibleCount = 0;
            foreach (string design in designs)
            {
                if (CheckDesign(design, patterns, memo))
                {
                    possibleCount++;
                }
            }

            return possibleCount.ToString();
        }

        public string RunPartB(string inputData)
        {
            (HashSet<string> patterns, List<string> designs) = ParseInput(inputData);

            Dictionary<string, long> memo = [];

            long possibleCount = 0;
            foreach (string design in designs)
            {
                possibleCount += CountPossibleDesigns(design, patterns, memo);
            }

            return possibleCount.ToString();
        }

        private static long CountPossibleDesigns(string design, HashSet<string> patterns, Dictionary<string, long> memo)
        {
            if (memo.ContainsKey(design))
            {
                return memo[design];
            }

            long count = 0;

            if (patterns.Contains(design))
            {
                count++;
            }

            for (int i = 1; i < design.Length; i++)
            {
                string left = design.Substring(0, i);
                string right = design.Substring(i);

                if (patterns.Contains(left))
                {
                    count += CountPossibleDesigns(right, patterns, memo);
                    memo[design] = count;
                }
            }

            memo[design] = count;
            return count;
        }

        private static bool CheckDesign(string design, HashSet<string> patterns, Dictionary<string, bool> memo)
        {
            if (memo.ContainsKey(design))
            {
                return memo[design];
            }
            if (patterns.Contains(design))
            {
                memo[design] = true;
                return true;
            }
            for (int i = 1; i < design.Length; i++)
            {
                string left = design.Substring(0, i);
                string right = design.Substring(i);

                if (patterns.Contains(left) && CheckDesign(right, patterns, memo))
                {
                    memo[design] = true;
                    return true;
                }
            }
            memo[design] = false;
            return false;
        }

        private static (HashSet<string> patterns, List<string> designs) ParseInput(string inputData)
        {
            List<string> lines = ParseUtils.ParseIntoLines(inputData).ToList();
            HashSet<string> patterns = new(lines[0].Split(",", StringSplitOptions.TrimEntries));
            List<string> designs = lines.Skip(1).Select(l => l.Trim()).ToList();

            return (patterns, designs);
        }
    }
}