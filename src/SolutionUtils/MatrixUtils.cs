namespace aoc_2024.SolutionUtils
{
    public static class MatrixUtils
    {
        #region CharMatrix

        public static char[][] CreateCharMatrix(string textBlock)
        {
            string[] lines = ParseUtils.ParseIntoLines(textBlock);

            return lines.Select(line => line.ToCharArray()).ToArray();
        }

        public static string[] GetAllCharMatrixRows(char[][] matrix)
        {
            return matrix.Select(row => new string(row)).ToArray();
        }

        public static string[] GetAllCharMatrixColumns(char[][] matrix)
        {
            int rows = matrix.Length;
            int columns = matrix[0].Length;

            string[] result = new string[columns];

            for (int col = 0; col < columns; col++)
            {
                char[] column = new char[rows];
                for (int row = 0; row < rows; row++)
                {
                    column[row] = matrix[row][col];
                }
                result[col] = new string(column);
            }

            return result;
        }

        public static string[] GetAllCharMatrixNegativeDiagonals(char[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            List<string> diagonals = [];

            for (int startCol = cols - 1; startCol >= 0; startCol--)
            {
                List<char> diagonal = [];
                for (int row = 0, col = startCol; row < rows && col < cols; row++, col++)
                {
                    diagonal.Add(matrix[row][col]);
                }
                diagonals.Add(new string(diagonal.ToArray()));
            }

            for (int startRow = 1; startRow < rows; startRow++)
            {
                List<char> diagonal = [];
                for (int row = startRow, col = 0; row < rows && col < cols; row++, col++)
                {
                    diagonal.Add(matrix[row][col]);
                }
                diagonals.Add(new string(diagonal.ToArray()));
            }

            return diagonals.ToArray();
        }

        public static string[] GetAllCharMatrixPositiveDiagonals(char[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            List<string> diagonals = [];

            for (int startRow = 0; startRow < rows; startRow++)
            {
                List<char> diagonal = [];
                for (int row = startRow, col = 0; row >= 0 && col < cols; row--, col++)
                {
                    diagonal.Add(matrix[row][col]);
                }
                diagonals.Add(new string(diagonal.ToArray()));
            }

            for (int startCol = 1; startCol < cols; startCol++)
            {
                List<char> diagonal = [];
                for (int row = rows - 1, col = startCol; row >= 0 && col < cols; row--, col++)
                {
                    diagonal.Add(matrix[row][col]);
                }
                diagonals.Add(new string(diagonal.ToArray()));
            }

            return diagonals.ToArray();
        }

        #endregion
    }
}
