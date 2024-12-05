using aoc_2024.Interfaces;

namespace aoc_2024.Solutions
{
    public class Solution05 : ISolution
    {
        private static readonly string[] newLineSeparators = ["\r\n", "\n", "\r"];

        public string RunPartA(string inputData)
        {
            (HashSet<(int, int)> orderList, List<int[]> pageList) = ParseInput(inputData);

            return GetMiddleSum(GetLists(orderList, pageList, true)).ToString();
        }

        public string RunPartB(string inputData)
        {
            (HashSet<(int, int)> orderList, List<int[]> pageList) = ParseInput(inputData);

            List<int[]> incorrectLists = GetLists(orderList, pageList, false);

            foreach (int[] incorrectList in incorrectLists)
            {
                Array.Sort(incorrectList, (int x, int y) => orderList.Contains((x, y)) ? -1 : 1);
            }

            return GetMiddleSum(incorrectLists).ToString();
        }

        private static long GetMiddleSum(List<int[]> lists)
        {
            long totalMidPage = 0;

            foreach (int[] list in lists)
            {
                totalMidPage += list[list.Length / 2];
            }

            return totalMidPage;
        }

        private static List<int[]> GetLists(HashSet<(int, int)> orderList, List<int[]> completePageList, bool shouldFindCorrect)
        {
            List<int[]> result = [];

            foreach (int[] pageSequence in completePageList)
            {
                bool isSequenceValid = true;
                int lastPage = pageSequence[0];

                for (int i = 1; i < pageSequence.Length; i++)
                {
                    if (!orderList.Contains((lastPage, pageSequence[i])))
                    {
                        isSequenceValid = false;
                        break;
                    }

                    lastPage = pageSequence[i];
                }

                if (isSequenceValid == shouldFindCorrect)
                {
                    result.Add(pageSequence);
                }
            }

            return result;
        }

        private static (HashSet<(int, int)> orderList, List<int[]> pageList) ParseInput(string inputData)
        {
            string[] lines = inputData.Split(newLineSeparators, StringSplitOptions.None);
            int splitIndex = Array.IndexOf(lines, "");

            HashSet<(int, int)> orderList = lines.Take(splitIndex)
                .Select(s =>
                {
                    string[] pages = s.Split('|');
                    return (int.Parse(pages[0]), int.Parse(pages[1]));
                })
                .ToHashSet();

            List<int[]> pageList = lines.Skip(splitIndex + 1)
                .Select(s => s.Split(',').Select(int.Parse).ToArray())
                .ToList();

            return (orderList, pageList);
        }
    }
}