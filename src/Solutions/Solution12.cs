using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution12 : ISolution
    {
        public string RunPartA(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            long totalCost = 0;

            HashSet<(int, int)> visited = [];

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (visited.Contains((i, j)))
                    {
                        continue;
                    }

                    Queue<(int, int)> queue = [];

                    int perimeter = 0;
                    int area = 0;

                    visited.Add((i, j));

                    queue.Enqueue((i, j));

                    //Console.WriteLine($"First coord: ({i},{j}). Char: {matrix[i][j]}");

                    while (queue.Count > 0)
                    {
                        (int x, int y) tile = queue.Dequeue();

                        (int, int)[] neighbors = MatrixUtils.GetOrthogonalNeighbors(matrix, (tile.x, tile.y));

                        area++;
                        perimeter += 4 - neighbors.Length;

                        foreach ((int x, int y) neighbor in neighbors)
                        {
                            if (matrix[neighbor.x][neighbor.y] == matrix[tile.x][tile.y])
                            {
                                if (visited.Contains(neighbor))
                                {
                                    continue;
                                }

                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                            else
                            {
                                perimeter++;
                            }
                        }
                    }

                    //Console.WriteLine($"Area: {area}. Perimeter: {perimeter}");
                    //Console.WriteLine("--------------------------------------");

                    totalCost += perimeter * area;
                }
            }

            return totalCost.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }
    }
}