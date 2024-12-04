using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution04 : ISolution
    {
        public string RunPartA(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            int total = 0;

            List<string[]> allSections =
                [
                    MatrixUtils.GetAllCharMatrixRows(matrix),
                    MatrixUtils.GetAllCharMatrixColumns(matrix),
                    MatrixUtils.GetAllCharMatrixPositiveDiagonals(matrix),
                    MatrixUtils.GetAllCharMatrixNegativeDiagonals(matrix)
                ];

            foreach (string[] section in allSections)
            {
                total += section.Sum(CountOccurrences);
            }

            return total.ToString();
        }

        public string RunPartB(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);
            int numOfRows = matrix.Length;
            int numOfColumns = matrix[0].Length;

            string[] positiveDiagonals = MatrixUtils.GetAllCharMatrixPositiveDiagonals(matrix);

            List<(int, int)> positiveCandidates = [];

            for (int i = 0; i < positiveDiagonals.Length; i++)
            {
                positiveCandidates.AddRange(GetPositiveCandidates(i, numOfRows, positiveDiagonals[i], "MAS"));
                positiveCandidates.AddRange(GetPositiveCandidates(i, numOfRows, positiveDiagonals[i], "SAM"));
            }

            string[] negativeDiagonals = MatrixUtils.GetAllCharMatrixNegativeDiagonals(matrix);

            List<(int, int)> negativeCandidates = [];

            for (int i = 0; i < negativeDiagonals.Length; i++)
            {
                negativeCandidates.AddRange(GetNegativeCandidates(i, numOfColumns, negativeDiagonals[i], "MAS"));
                negativeCandidates.AddRange(GetNegativeCandidates(i, numOfColumns, negativeDiagonals[i], "SAM"));
            }

            return positiveCandidates.Intersect(negativeCandidates).Count().ToString();
        }

        private static int CountOccurrences(string part)
        {
            return Regex.Matches(part, @"(XMAS)").Count + Regex.Matches(part, @"(SAMX)").Count;
        }

        private static List<(int, int)> GetPositiveCandidates(int startPos, int numOfRows, string diagonal, string target)
        {
            List<int> positions = FindPositionsOfAInSubstring(diagonal, target);
            List<(int, int)> positiveCandidates = [];

            foreach (int aPosition in positions)
            {
                if (startPos < numOfRows)
                {
                    positiveCandidates.Add((startPos - aPosition, aPosition));
                }
                else
                {
                    positiveCandidates.Add((numOfRows - 1 - aPosition, startPos - numOfRows + 1 + aPosition));
                }
            }

            return positiveCandidates;
        }

        private static List<(int, int)> GetNegativeCandidates(int startPos, int numOfCols, string diagonal, string target)
        {
            List<int> positions = FindPositionsOfAInSubstring(diagonal, target);
            List<(int, int)> negativeCandidates = [];

            foreach (int aPosition in positions)
            {
                if (startPos < numOfCols)
                {
                    negativeCandidates.Add((aPosition, numOfCols - 1 - startPos + aPosition));
                }
                else
                {
                    negativeCandidates.Add((startPos - numOfCols + 1 + aPosition, aPosition));
                }
            }

            return negativeCandidates;
        }

        private static List<int> FindPositionsOfAInSubstring(string input, string target)
        {
            List<int> positions = [];
            int targetLength = target.Length;
            int startIndex = 0;

            while ((startIndex = input.IndexOf(target, startIndex)) != -1)
            {
                for (int i = 0; i < targetLength; i++)
                {
                    if (target[i] == 'A')
                    {
                        positions.Add(startIndex + i);
                    }
                }

                startIndex += targetLength;
            }

            return positions;
        }
    }
}