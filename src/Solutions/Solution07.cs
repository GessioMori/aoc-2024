using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution07 : ISolution
    {
        public string RunPartA(string inputData)
        {
            return GetValidSequencesSum(inputData, false);
        }

        public string RunPartB(string inputData)
        {
            return GetValidSequencesSum(inputData, true);
        }

        private static string GetValidSequencesSum(string inputData, bool shouldConcatenate)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            long validSum = 0;

            foreach (string line in lines)
            {
                (long result, int[] nums) = ParseInput(line);

                Queue<(long, int)> partialResults = [];

                partialResults.Enqueue((nums[0], 1));

                bool isResultValid = false;

                while (partialResults.Count > 0)
                {
                    (long partialResult, int currentIndex) = partialResults.Dequeue();

                    long sumResult = partialResult + nums[currentIndex];

                    if (currentIndex == nums.Length - 1 && sumResult == result)
                    {
                        isResultValid = true;
                        break;
                    }
                    else if (currentIndex < nums.Length - 1 && sumResult <= result)
                    {
                        partialResults.Enqueue((sumResult, currentIndex + 1));
                    }

                    long productResult = partialResult * nums[currentIndex];

                    if (currentIndex == nums.Length - 1 && productResult == result)
                    {
                        isResultValid = true;
                        break;
                    }
                    else if (currentIndex < nums.Length - 1 && productResult <= result)
                    {
                        partialResults.Enqueue((productResult, currentIndex + 1));
                    }

                    if (shouldConcatenate)
                    {
                        long concatenationResult = long.Parse($"{partialResult}{nums[currentIndex]}");

                        if (currentIndex == nums.Length - 1 && concatenationResult == result)
                        {
                            isResultValid = true;
                            break;
                        }
                        else if (currentIndex < nums.Length - 1 && concatenationResult <= result)
                        {
                            partialResults.Enqueue((concatenationResult, currentIndex + 1));
                        }
                    }
                }

                if (isResultValid)
                {
                    validSum += result;
                }
            }

            return validSum.ToString();
        }

        private static (long result, int[] nums) ParseInput(string line)
        {
            string[] parts = line.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            long result = long.Parse(parts[0]);

            int[] nums = parts[1]
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            return (result, nums);
        }
    }
}