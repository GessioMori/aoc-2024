using Spectre.Console;

namespace aoc_2024.SolutionUtils
{
    public static class MatrixUtils
    {
        #region IntMatrix

        private static (int, int)[] orthogonalNeighbors = [(-1, 0), (1, 0), (0, -1), (0, 1)];

        public static int[][] CreateIntMatrix(string textBlock)
        {
            string[] lines = ParseUtils.ParseIntoLines(textBlock);

            return lines.Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
        }

        public static (int, int)[] GetOrthogonalNeighbors<T>(T[][] matrix, (int x, int y) position)
        {
            List<(int, int)> neighbors = [];

            foreach (var (dr, dc) in orthogonalNeighbors)
            {
                int newRow = position.x + dr;
                int newCol = position.y + dc;

                if (newRow >= 0 && newRow < matrix.Length && newCol >= 0 && newCol < matrix[newRow].Length)
                {
                    neighbors.Add((newRow, newCol));
                }
            }

            return neighbors.ToArray();
        }

        #endregion

        #region CharMatrix

        public static char[][] CreateCharMatrix(string textBlock)
        {
            string[] lines = ParseUtils.ParseIntoLines(textBlock);

            return lines.Select(line => line.ToCharArray()).ToArray();
        }

        public static MatrixAxis[] GetAllCharMatrixRows(char[][] matrix)
        {
            int numOfRows = matrix.Length;
            int numOfColumns = matrix[0].Length;

            MatrixAxis[] matrixRows = new MatrixAxis[numOfRows];

            for (int i = 0; i < numOfRows; i++)
            {
                (int, int)[] positions = new (int, int)[numOfColumns];

                for (int j = 0; j < numOfColumns; j++)
                {
                    positions[j] = (i, j);
                }

                matrixRows[i] = new MatrixAxis(new string(matrix[i]), positions);
            }

            return matrixRows;
        }

        public static MatrixAxis[] GetAllCharMatrixColumns(char[][] matrix)
        {
            int numOfRows = matrix.Length;
            int numOfColumns = matrix[0].Length;

            MatrixAxis[] matrixColumns = new MatrixAxis[numOfColumns];

            for (int i = 0; i < numOfColumns; i++)
            {
                (int, int)[] positions = new (int, int)[numOfRows];
                char[] chars = new char[numOfRows];

                for (int j = 0; j < numOfRows; j++)
                {
                    positions[j] = (j, i);
                    chars[j] = matrix[j][i];
                }

                matrixColumns[i] = new MatrixAxis(new string(chars), positions);
            }

            return matrixColumns;
        }

        public static MatrixAxis[] GetAllCharMatrixNegativeDiagonals(char[][] matrix)
        {
            int numOfRows = matrix.Length;
            int numOfColumns = matrix[0].Length;

            List<MatrixAxis> matrixDiagonals = [];

            for (int startCol = numOfColumns - 1; startCol >= 0; startCol--)
            {
                List<char> diagonalChars = [];
                List<(int, int)> diagonalCoordinates = [];

                for (int row = 0, col = startCol; row < numOfRows && col < numOfColumns; row++, col++)
                {
                    diagonalChars.Add(matrix[row][col]);
                    diagonalCoordinates.Add((row, col));
                }

                matrixDiagonals.Add(new MatrixAxis(new string(diagonalChars.ToArray()), diagonalCoordinates.ToArray()));
            }

            for (int startRow = 1; startRow < numOfRows; startRow++)
            {
                List<char> diagonalChars = [];
                List<(int, int)> diagonalCoordinates = [];

                for (int row = startRow, col = 0; row < numOfRows && col < numOfColumns; row++, col++)
                {
                    diagonalChars.Add(matrix[row][col]);
                    diagonalCoordinates.Add((row, col));
                }

                matrixDiagonals.Add(new MatrixAxis(new string(diagonalChars.ToArray()), diagonalCoordinates.ToArray()));
            }

            return matrixDiagonals.ToArray();
        }

        public static MatrixAxis[] GetAllCharMatrixPositiveDiagonals(char[][] matrix)
        {
            int numOfRows = matrix.Length;
            int numOfColumns = matrix[0].Length;

            List<MatrixAxis> matrixDiagonals = [];

            for (int startRow = 0; startRow < numOfRows; startRow++)
            {
                List<char> diagonalChars = [];
                List<(int, int)> diagonalCoordinates = [];

                for (int row = startRow, col = 0; row >= 0 && col < numOfColumns; row--, col++)
                {
                    diagonalChars.Add(matrix[row][col]);
                    diagonalCoordinates.Add((row, col));
                }

                matrixDiagonals.Add(new MatrixAxis(new string(diagonalChars.ToArray()), diagonalCoordinates.ToArray()));
            }

            for (int startCol = 1; startCol < numOfColumns; startCol++)
            {
                List<char> diagonalChars = [];
                List<(int, int)> diagonalCoordinates = [];

                for (int row = numOfRows - 1, col = startCol; row >= 0 && col < numOfColumns; row--, col++)
                {
                    diagonalChars.Add(matrix[row][col]);
                    diagonalCoordinates.Add((row, col));
                }

                matrixDiagonals.Add(new MatrixAxis(new string(diagonalChars.ToArray()), diagonalCoordinates.ToArray()));
            }

            return matrixDiagonals.ToArray();
        }

        #endregion
    }
}
