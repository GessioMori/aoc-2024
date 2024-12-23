using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution22 : ISolution
    {
        public string RunPartA(string inputData)
        {
            long[] initialSecrets = ParseUtils.ParseIntoLines(inputData).Select(long.Parse).ToArray();

            long totalFinalSecrets = 0;

            foreach (long initialSecret in initialSecrets)
            {
                long currentSecret = initialSecret;

                for (int i = 0; i < 2000; i++)
                {
                    currentSecret = GetNextSecretNumber(currentSecret);
                }

                totalFinalSecrets += currentSecret;
            }

            return totalFinalSecrets.ToString();
        }

        public string RunPartB(string inputData)
        {
            long[] initialSecrets = ParseUtils.ParseIntoLines(inputData).Select(long.Parse).ToArray();

            int[][] pricesByBuyer = initialSecrets.Select(GetPrices).ToArray();

            int[][] differencesByBuyer = pricesByBuyer.Select(GetDiferences).ToArray();

            Dictionary<(int, int, int, int), long> sumBySequence = [];

            for (int i = 0; i < differencesByBuyer.Length; i++)
            {
                HashSet<(int, int, int, int)> sequences = [];

                for (int j = 3; j < differencesByBuyer[i].Length; j++)
                {
                    (int, int, int, int) sequence =
                        (differencesByBuyer[i][j],
                        differencesByBuyer[i][j - 1],
                        differencesByBuyer[i][j - 2],
                        differencesByBuyer[i][j - 3]);

                    if (sequences.Contains(sequence)) continue;

                    sequences.Add(sequence);

                    if (sumBySequence.ContainsKey(sequence))
                    {
                        sumBySequence[sequence] += pricesByBuyer[i][j + 1];
                    }
                    else
                    {
                        sumBySequence[sequence] = pricesByBuyer[i][j + 1];
                    }
                }
            }

            return sumBySequence.Values.Max().ToString();
        }

        private int[] GetPrices(long initialSecret)
        {
            int[] prices = new int[2001];

            long currentSecret = initialSecret;

            for (int i = 0; i < 2001; i++)
            {
                prices[i] = (int)(currentSecret % 10);
                currentSecret = GetNextSecretNumber(currentSecret);
            }

            return prices;
        }

        private int[] GetDiferences(int[] prices)
        {
            int[] differences = new int[2000];

            for (int i = 1; i < 2001; i++)
            {
                differences[i - 1] = prices[i] - prices[i - 1];
            }

            return differences;
        }

        private static long GetNextSecretNumber(long currentSecretNumber)
        {
            long result = ((currentSecretNumber * 64) ^ currentSecretNumber) % 16777216;

            result = ((int)(result / 32) ^ result) % 16777216;

            result = ((result * 2048) ^ result) % 16777216;

            return result;
        }
    }
}