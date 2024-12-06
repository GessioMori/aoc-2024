using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution06 : ISolution
    {
        private static readonly (int, int)[] directions = [(-1, 0), (0, 1), (1, 0), (0, -1)];

        public string RunPartA(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            (int x, int y) currentPosition = FindInitialPosition(matrix);

            int currentDirectionIndex = 0;

            matrix[currentPosition.x][currentPosition.y] = 'X';

            while (true)
            {
                (int x, int y) nextPosition = currentPosition.Add(directions[currentDirectionIndex]);

                if (nextPosition.x < 0 || nextPosition.x >= matrix.Length ||
                    nextPosition.y < 0 || nextPosition.y >= matrix[0].Length)
                {
                    break;
                }

                char nextTile = matrix[nextPosition.x][nextPosition.y];

                if (nextTile == '#')
                {
                    currentDirectionIndex = (currentDirectionIndex + 1) % 4;
                }
                else
                {
                    currentPosition = nextPosition;
                    matrix[currentPosition.x][currentPosition.y] = 'X';
                }
            }

            int totalSteps = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j] == 'X')
                    {
                        totalSteps++;
                    }
                }
            }

            return totalSteps.ToString();
        }

        public string RunPartB(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            (int x, int y) initialPos = FindInitialPosition(matrix);

            int totalObstacles = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j] == '.')
                    {
                        int currentDirectionIndex = 0;
                        matrix[i][j] = '#';

                        HashSet<(int, int, int)> visited = [];

                        (int x, int y) currentPosition = initialPos;

                        bool isLoop = true;

                        do
                        {
                            visited.Add((currentPosition.x, currentPosition.y, currentDirectionIndex));
                            (int x, int y) nextPosition = currentPosition.Add(directions[currentDirectionIndex]);

                            if (nextPosition.x < 0 || nextPosition.x >= matrix.Length ||
                                nextPosition.y < 0 || nextPosition.y >= matrix[0].Length)
                            {
                                isLoop = false;
                                break;
                            }

                            char nextTile = matrix[nextPosition.x][nextPosition.y];

                            if (nextTile == '#')
                            {
                                currentDirectionIndex = (currentDirectionIndex + 1) % 4;
                            }
                            else
                            {
                                currentPosition = nextPosition;
                            }
                        }
                        while (!visited.Contains((currentPosition.x, currentPosition.y, currentDirectionIndex)));

                        if (isLoop)
                        {
                            totalObstacles++;
                        }

                        matrix[i][j] = '.';
                    }
                }
            }

            return totalObstacles.ToString();
        }

        private static (int, int) FindInitialPosition(char[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (matrix[i][j] == '^')
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }
    }
}