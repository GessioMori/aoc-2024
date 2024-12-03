using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution02 : ISolution
    {
        public string RunPartA(string inputData)
        {
            int[][] reports = ParseReports(inputData);

            int safeReportsCount = 0;

            for (int i = 0; i < reports.Length; i++)
            {
                if (IsReportSafe(reports[i]))
                {
                    safeReportsCount++;
                }
            }

            return safeReportsCount.ToString();
        }

        public string RunPartB(string inputData)
        {
            int[][] reports = ParseReports(inputData);

            int safeReportsCount = 0;

            for (int i = 0; i < reports.Length; i++)
            {
                int[] originalReport = reports[i];
                int[] report = new int[originalReport.Length - 1];

                for (int j = 0; j < originalReport.Length; j++)
                {
                    Array.Copy(originalReport, 0, report, 0, j);
                    Array.Copy(originalReport, j + 1, report, j, originalReport.Length - j - 1);

                    if (IsReportSafe(report))
                    {
                        safeReportsCount++;
                        break;
                    }
                }
            }

            return safeReportsCount.ToString();
        }

        private static int[][] ParseReports(string inputData)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            int[][] result = new int[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i]
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(int.Parse)
                    .ToArray();
            }

            return result;
        }

        private static bool IsReportSafe(int[] report)
        {
            bool isSafe = true;

            bool isAcending = report[0] < report[1];

            for (int j = 0; j < report.Length - 1; j++)
            {
                int difference = report[j] - report[j + 1];

                if ((isAcending ^ difference < 0) ||
                    Math.Abs(difference) > 3 ||
                    Math.Abs(difference) < 1)
                {
                    isSafe = false;
                    break;
                }
            }

            return isSafe;
        }
    }
}