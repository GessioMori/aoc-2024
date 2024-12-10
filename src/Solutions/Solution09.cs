using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution09 : ISolution
    {
        public string RunPartA(string inputData)
        {
            List<int> disk = GetDiskList(inputData);

            int currentEmptyIndex = disk.FindIndex(p => p == -1);

            for (int i = disk.Count - 1; i >= 0 && currentEmptyIndex < i; i--)
            {
                if (disk[i] == -1) continue;

                disk[currentEmptyIndex] = disk[i];
                disk[i] = -1;

                for (int j = currentEmptyIndex + 1; j < disk.Count; j++)
                {
                    if (disk[j] == -1)
                    {
                        currentEmptyIndex = j;
                        break;
                    }
                }
            }

            return FindCheckSum(disk).ToString();
        }

        public string RunPartB(string inputData)
        {
            List<int> disk = GetDiskList(inputData);

            int firstEmptyIndex = disk.FindIndex(p => p == -1);

            for (int i = disk.Count - 1; i >= 0 && firstEmptyIndex < i; i--)
            {
                if (disk[i] == -1) continue;

                int fileId = disk[i];
                int fileSize = 0;

                while (i - fileSize >= 0 && disk[i - fileSize] == fileId)
                {
                    fileSize++;
                }

                int emptySpace = 0;
                int firstEmptyIndexAvailable = firstEmptyIndex;

                for (int j = firstEmptyIndex; j <= i - fileSize && emptySpace < fileSize; j++)
                {
                    if (disk[j] == -1 && emptySpace == 0)
                    {
                        firstEmptyIndexAvailable = j;
                    }

                    emptySpace = disk[j] != -1 ? 0 : emptySpace + 1;
                }

                if (emptySpace == fileSize)
                {
                    for (int j = 0; j < fileSize; j++)
                    {
                        disk[firstEmptyIndexAvailable + j] = fileId;
                        disk[i - j] = -1;
                    }
                }

                firstEmptyIndex = disk.FindIndex(p => p == -1);
                i -= fileSize - 1;
            }

            return FindCheckSum(disk).ToString();
        }

        private static long FindCheckSum(List<int> disk)
        {
            long checksum = 0;

            for (int i = 0; i < disk.Count; i++)
            {
                if (disk[i] != -1)
                {
                    checksum += disk[i] * i;
                }
            }

            return checksum;
        }

        private static List<int> GetDiskList(string inputData)
        {
            int currentId = 0;

            List<int> disk = [];

            int[] partValues = ParseUtils.ParseIntoLines(inputData)[0]
                .Select(c => int.Parse(c.ToString()))
                .ToArray();

            for (int i = 0; i < partValues.Length; i++)
            {
                for (int j = 0; j < partValues[i]; j++)
                {
                    disk.Add(i % 2 == 0 ? currentId : -1);
                }

                if (i % 2 == 0)
                {
                    currentId++;
                }
            }

            return disk;
        }
    }
}