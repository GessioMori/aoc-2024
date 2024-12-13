using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution12 : ISolution
    {
        public string RunPartA(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);
            List<CropRegion> regions = GetRegions(matrix);

            return regions.Sum(region => region.Perimeter * region.Area).ToString();
        }

        public string RunPartB(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);
            List<CropRegion> regions = GetRegions(matrix);

            Dictionary<int, int> entrancesX = [];
            Dictionary<int, int> exitsX = [];
            Dictionary<int, HashSet<(int, int)>> visitedEntrancesX = [];
            Dictionary<int, HashSet<(int, int)>> visitedExitsX = [];

            foreach (CropRegion region in regions)
            {
                entrancesX[region.Id] = 0;
                exitsX[region.Id] = 0;
                visitedEntrancesX[region.Id] = [];
                visitedExitsX[region.Id] = [];
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                int currentRegion = -1;

                for (int j = 0; j < matrix[i].Length; j++)
                {
                    CropRegion region = regions.First(r => r.Tiles.Contains((i, j)));

                    if (currentRegion != region.Id)
                    {
                        visitedEntrancesX[region.Id].Add((i, j));

                        if (!visitedEntrancesX[region.Id].Contains((i - 1, j)) && !visitedEntrancesX[region.Id].Contains((i + 1, j)))
                        {
                            entrancesX[region.Id]++;
                        }
                    }

                    if (j + 1 == matrix[i].Length || matrix[i][j + 1] != region.Type)
                    {
                        visitedExitsX[region.Id].Add((i, j));

                        if (!visitedExitsX[region.Id].Contains((i - 1, j)) && !visitedExitsX[region.Id].Contains((i + 1, j)))
                        {
                            exitsX[region.Id]++;
                        }
                    }

                    currentRegion = region.Id;
                }
            }

            Dictionary<int, int> entrancesY = [];
            Dictionary<int, int> exitsY = [];
            Dictionary<int, HashSet<(int, int)>> visitedEntrancesY = [];
            Dictionary<int, HashSet<(int, int)>> visitedExitsY = [];

            foreach (CropRegion region in regions)
            {
                entrancesY[region.Id] = 0;
                exitsY[region.Id] = 0;
                visitedEntrancesY[region.Id] = [];
                visitedExitsY[region.Id] = [];
            }

            for (int j = 0; j < matrix[0].Length; j++)
            {
                int currentRegion = -1;

                for (int i = 0; i < matrix.Length; i++)
                {
                    CropRegion region = regions.First(r => r.Tiles.Contains((i, j)));

                    if (currentRegion != region.Id)
                    {
                        visitedEntrancesY[region.Id].Add((i, j));

                        if (!visitedEntrancesY[region.Id].Contains((i, j - 1)) && !visitedEntrancesX[region.Id].Contains((i, j + 1)))
                        {
                            entrancesY[region.Id]++;
                        }
                    }

                    if (i + 1 == matrix.Length || matrix[i + 1][j] != region.Type)
                    {
                        visitedExitsY[region.Id].Add((i, j));

                        if (!visitedExitsY[region.Id].Contains((i, j - 1)) && !visitedExitsY[region.Id].Contains((i, j + 1)))
                        {
                            exitsY[region.Id]++;
                        }
                    }

                    currentRegion = region.Id;
                }
            }

            foreach (CropRegion region in regions)
            {
                region.Sides = entrancesX[region.Id] + entrancesY[region.Id] + exitsX[region.Id] + exitsY[region.Id];
            }

            return regions.Sum(region => region.Sides * region.Area).ToString();
        }

        private static List<CropRegion> GetRegions(char[][] matrix)
        {
            List<CropRegion> regions = [];

            HashSet<(int, int)> visited = [];

            int id = 0;

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    if (visited.Contains((i, j)))
                    {
                        continue;
                    }

                    CropRegion region = new()
                    {
                        Type = matrix[i][j],
                        Id = id++
                    };

                    Queue<(int, int)> queue = [];

                    int perimeter = 0;

                    region.Tiles.Add((i, j));
                    visited.Add((i, j));
                    queue.Enqueue((i, j));

                    while (queue.Count > 0)
                    {
                        (int x, int y) tile = queue.Dequeue();
                        region.Tiles.Add((tile.x, tile.y));

                        (int, int)[] neighbors = MatrixUtils.GetOrthogonalNeighbors(matrix, (tile.x, tile.y));

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

                    region.Perimeter = perimeter;
                    regions.Add(region);
                }
            }

            return regions;
        }
    }

    public class CropRegion
    {
        public int Id;
        public char Type;
        public int Perimeter;
        public int Sides;
        public HashSet<(int x, int y)> Tiles = [];
        public int Area => Tiles.Count;
    }
}