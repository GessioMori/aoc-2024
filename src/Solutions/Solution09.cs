using aoc_2024.Interfaces;

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
            List<int>[] emptySpaces = GetEmptySpaces(disk);
            HashSet<int> movedFiles = [];

            for (int i = disk.Count - 1; i >= 0; i--)
            {
                int fileId = disk[i];
                if (fileId == -1 || movedFiles.Contains(fileId)) continue;

                int fileSize = 0;

                while (i - fileSize >= 0 && disk[i - fileSize] == fileId)
                {
                    fileSize++;
                }

                int firstEmptyIndex = emptySpaces
                    .Where(l => l.Count != 0)
                    .Min(l => l[0]);

                if (i <= firstEmptyIndex) break;

                int minIndex = int.MaxValue;
                int emptySize = -1;

                for (int j = fileSize - 1; j < emptySpaces.Length; j++)
                {
                    if (emptySpaces[j].Count > 0 && emptySpaces[j][0] < minIndex)
                    {
                        minIndex = emptySpaces[j][0];
                        emptySize = j + 1;
                    }
                }

                if (emptySize > 0 && minIndex < i)
                {
                    for (int j = 0; j < fileSize; j++)
                    {
                        disk[minIndex + j] = fileId;
                        disk[i - j] = -1;
                    }

                    emptySpaces[emptySize - 1].RemoveAt(0);

                    if (fileSize < emptySize)
                    {
                        int index = emptySpaces[emptySize - fileSize - 1].BinarySearch(minIndex + fileSize);
                        if (index < 0)
                        {
                            index = ~index;
                        }

                        emptySpaces[emptySize - fileSize - 1].Insert(index, minIndex + fileSize);
                    }

                    movedFiles.Add(fileId);
                }

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

            int[] partValues = inputData.Trim()
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

        private static List<int>[] GetEmptySpaces(List<int> disk)
        {
            List<int>[] emptySpaces = new List<int>[9];

            for (int i = 0; i < 9; i++)
            {
                emptySpaces[i] = [];
            }

            int emptyCount = 0;

            for (int i = 0; i < disk.Count; i++)
            {
                if (disk[i] == -1)
                {
                    emptyCount++;
                }
                else if (emptyCount != 0)
                {
                    emptySpaces[emptyCount - 1].Add(i - emptyCount);
                    emptyCount = 0;
                }
            }

            return emptySpaces;
        }
    }
}