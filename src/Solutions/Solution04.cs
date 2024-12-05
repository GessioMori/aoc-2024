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

            List<MatrixAxis[]> allSections =
                [
                    MatrixUtils.GetAllCharMatrixRows(matrix),
                    MatrixUtils.GetAllCharMatrixColumns(matrix),
                    MatrixUtils.GetAllCharMatrixPositiveDiagonals(matrix),
                    MatrixUtils.GetAllCharMatrixNegativeDiagonals(matrix)
                ];

            foreach (MatrixAxis[] axis in allSections)
            {
                total += axis.Select(a => a.AxisString).Sum(CountOccurrences);
            }

            return total.ToString();
        }

        public string RunPartB(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            MatrixAxis[] positiveDiagonals = MatrixUtils.GetAllCharMatrixPositiveDiagonals(matrix);
            MatrixAxis[] negativeDiagonals = MatrixUtils.GetAllCharMatrixNegativeDiagonals(matrix);

            List<(int, int)> positiveCandidates = [];
            List<(int, int)> negativeCandidates = [];

            for (int i = 0; i < positiveDiagonals.Length; i++)
            {
                positiveCandidates.AddRange(
                    FindPositionsOfAInSubstring(positiveDiagonals[i].AxisString)
                    .Select(pos => positiveDiagonals[i].GetCoordinatesByAxisPosition(pos)));

                negativeCandidates.AddRange(
                    FindPositionsOfAInSubstring(negativeDiagonals[i].AxisString)
                    .Select(pos => negativeDiagonals[i].GetCoordinatesByAxisPosition(pos)));
            }

            return positiveCandidates.Intersect(negativeCandidates).Count().ToString();
        }

        private static int CountOccurrences(string part)
        {
            return Regex.Matches(part, @"(XMAS)").Count + Regex.Matches(part, @"(SAMX)").Count;
        }

        private static int[] FindPositionsOfAInSubstring(string input)
        {
            return [..Regex.Matches(input, @"(MAS)").Index().Select(i => i.Item.Index + 1),
                ..Regex.Matches(input, @"(SAM)").Index().Select(i => i.Item.Index + 1)];
        }
    }
}