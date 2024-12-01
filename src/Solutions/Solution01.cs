using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution01 : ISolution
    {
        public string RunPartA(string inputData)
        {
            (int[] leftList, int[] rightList) = ParseLists(inputData);

            long totalDifference = 0;

            for (int i = 0; i < leftList.Length; i++)
            {
                totalDifference += Math.Abs(leftList[i] - rightList[i]);
            }

            return totalDifference.ToString();
        }

        public string RunPartB(string inputData)
        {
            (int[] leftList, int[] rightList) = ParseLists(inputData);

            Dictionary<int, int> rightListCount = [];

            for (int i = 0; i < rightList.Length; i++)
            {
                if (rightListCount.TryGetValue(rightList[i], out int value))
                {
                    rightListCount[rightList[i]] = ++value;
                }
                else
                {
                    rightListCount.Add(rightList[i], 1);
                }
            }

            long similarity = 0;

            for (int i = 0; i < leftList.Length; i++)
            {
                if (rightListCount.TryGetValue(leftList[i], out int value))
                {
                    similarity += leftList[i] * value;
                }
            }

            return similarity.ToString();
        }

        private static (int[], int[]) ParseLists(string inputData)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            int[] leftList = new int[lines.Length];
            int[] rightList = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                leftList[i] = int.Parse(parts[0]);
                rightList[i] = int.Parse(parts[1]);
            }

            leftList = [.. leftList.Order()];
            rightList = [.. rightList.Order()];

            return (leftList, rightList);
        }
    }
}