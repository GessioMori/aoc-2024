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

            HashSet<(int, int, int)> visited = [];

            RunMap(matrix, visited, false);

            int totalSteps = visited.Select(s => (s.Item1, s.Item2)).Distinct().Count();

            return totalSteps.ToString();
        }

        public string RunPartB(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            int totalObstacles = 0;
            Lock lockObj = new();

            Parallel.For(0, matrix.Length, i =>
            {
                int localObstacles = 0;

                Parallel.For(0, matrix[i].Length, j =>
                {
                    if (matrix[i][j] == '.')
                    {
                        char[][] matrixCopy = matrix.Select(row => row.ToArray()).ToArray();
                        matrixCopy[i][j] = '#';

                        HashSet<(int, int, int)> visited = [];

                        if (RunMap(matrixCopy, visited, true))
                        {
                            Interlocked.Increment(ref localObstacles);
                        }
                    }
                });

                lock (lockObj)
                {
                    totalObstacles += localObstacles;
                }
            });

            return totalObstacles.ToString();
        }

        private static bool RunMap(char[][] matrix, HashSet<(int, int, int)> visited, bool shouldCheckVisited)
        {
            (int x, int y) currentPosition = FindInitialPosition(matrix);
            int currentDirectionIndex = 0;

            bool isLoop = true;

            while (!shouldCheckVisited || !visited.Contains((currentPosition.x, currentPosition.y, currentDirectionIndex)))
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

            return isLoop;
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