using aoc_2024.Interfaces;

namespace aoc_2024.Solutions
{
    public class Solution25 : ISolution
    {
        private readonly List<int[]> locks = [];
        private readonly List<int[]> keys = [];

        public string RunPartA(string inputData)
        {
            ParseInput(inputData);

            int totalMatches = 0;

            for (int i = 0; i < locks.Count; i++)
            {
                for (int j = 0; j < keys.Count; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (locks[i][k] + keys[j][k] > 5)
                        {
                            break;
                        }

                        if (k == 4)
                        {
                            totalMatches++;
                        }
                    }
                }
            }

            return totalMatches.ToString();
        }

        public string RunPartB(string inputData)
        {
            return "Merry Christmas!";
        }

        private void ParseInput(string inputData)
        {
            string[] parts = inputData.Split(["\r\n\r\n", "\n\n"],
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string[] lines = part.Split("\n");

                int[] countHashes = new int[5].ToArray();

                for (int i = 1; i < lines.Length - 1; i++)
                {
                    string line = lines[i];

                    for (int j = 0; j < line.Length; j++)
                    {
                        if (line[j] == '#')
                        {
                            countHashes[j]++;
                        }
                    }
                }

                if (lines[0].Contains('#'))
                {
                    locks.Add(countHashes);
                }
                else
                {
                    keys.Add(countHashes);
                }
            }
        }
    }
}