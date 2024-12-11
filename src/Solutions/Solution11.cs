using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution11 : ISolution
    {
        public string RunPartA(string inputData)
        {
            return CountStones(inputData, 25).ToString();
        }

        public string RunPartB(string inputData)
        {
            return CountStones(inputData, 75).ToString();
        }

        private static long CountStones(string inputData, int maxNumOfBlinks)
        {
            string[] stones = ParseUtils.ParseIntoLines(inputData)
                .First()
                .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            long numOfStones = 0;

            Dictionary<(string, int), long> memo = [];

            foreach (string stone in stones)
            {
                numOfStones += GetNumOfStones(memo, maxNumOfBlinks, 0, stone);
            }

            return numOfStones;
        }

        private static long GetNumOfStones(Dictionary<(string, int), long> memo, int maxNumOfBlinks,
            int blinkCount, string stone)
        {
            if (blinkCount == maxNumOfBlinks)
            {
                return 1;
            }

            if (memo.TryGetValue((stone, blinkCount), out long value))
            {
                return value;
            }

            long result;

            if (stone == "0")
            {
                result = GetNumOfStones(memo, maxNumOfBlinks, blinkCount + 1, "1");
                memo[(stone, blinkCount)] = result;
                return result;
            }
            else if (stone.Length % 2 == 0)
            {
                string firstHalf = stone[..(stone.Length / 2)];
                string secondHalf = stone[(stone.Length / 2)..];

                result = GetNumOfStones(memo, maxNumOfBlinks, blinkCount + 1, firstHalf) +
                    GetNumOfStones(memo, maxNumOfBlinks, blinkCount + 1, long.Parse(secondHalf).ToString());
                memo[(stone, blinkCount)] = result;
                return result;
            }

            result = GetNumOfStones(memo, maxNumOfBlinks, blinkCount + 1, (long.Parse(stone) * 2024).ToString());
            memo[(stone, blinkCount)] = result;
            return result;
        }
    }
}