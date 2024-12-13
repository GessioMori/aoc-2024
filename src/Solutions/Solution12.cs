using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution12 : ISolution
    {
        private char[][] matrix = null!;
        private List<CropRegion> cropRegions = [];
        private Dictionary<int, int> uniqueEntrances = [];
        private Dictionary<int, int> uniqueExits = [];
        private Dictionary<int, HashSet<(int, int)>> visitedEntrances = [];
        private Dictionary<int, HashSet<(int, int)>> visitedExits = [];
        private Dictionary<(int, int), CropRegion> tilesPerRegion = [];

        public string RunPartA(string inputData)
        {
            this.matrix = MatrixUtils.CreateCharMatrix(inputData);
            this.cropRegions = GetRegions(matrix);

            return this.cropRegions.Sum(region => region.Perimeter * region.Area).ToString();
        }

        public string RunPartB(string inputData)
        {
            this.matrix = MatrixUtils.CreateCharMatrix(inputData);
            this.cropRegions = GetRegions(matrix);

            foreach (CropRegion region in this.cropRegions)
            {
                foreach ((int, int) tile in region.Tiles)
                {
                    this.tilesPerRegion[tile] = region;
                }
            }

            InitializeRegionData();

            ProcessMatrix(isHorizontal: true);

            foreach (CropRegion region in this.cropRegions)
            {
                region.Sides += this.uniqueEntrances[region.Id] + this.uniqueExits[region.Id];
            }

            InitializeRegionData();

            ProcessMatrix(isHorizontal: false);

            foreach (CropRegion region in this.cropRegions)
            {
                region.Sides += this.uniqueEntrances[region.Id] + this.uniqueExits[region.Id];
            }

            return this.cropRegions.Sum(region => region.Sides * region.Area).ToString();
        }

        private void InitializeRegionData()
        {
            this.uniqueEntrances = this.cropRegions.ToDictionary(region => region.Id, _ => 0);
            this.uniqueExits = this.cropRegions.ToDictionary(region => region.Id, _ => 0);
            this.visitedEntrances = this.cropRegions.ToDictionary(region => region.Id, _ => new HashSet<(int, int)>());
            this.visitedExits = this.cropRegions.ToDictionary(region => region.Id, _ => new HashSet<(int, int)>());
        }

        private void ProcessMatrix(bool isHorizontal)
        {
            int outerLength = isHorizontal ? this.matrix.Length : this.matrix[0].Length;
            int innerLength = isHorizontal ? this.matrix[0].Length : this.matrix.Length;

            for (int outer = 0; outer < outerLength; outer++)
            {
                int currentRegion = -1;

                for (int inner = 0; inner < innerLength; inner++)
                {
                    int i = isHorizontal ? outer : inner;
                    int j = isHorizontal ? inner : outer;

                    CropRegion region = tilesPerRegion[(i, j)];

                    if (currentRegion != region.Id)
                    {
                        visitedEntrances[region.Id].Add((i, j));
                        if (!HasAdjacentVisit(visitedEntrances[region.Id], i, j, isHorizontal))
                        {
                            this.uniqueEntrances[region.Id]++;
                        }
                    }

                    if (IsExit(i, j, region.Type, isHorizontal))
                    {
                        visitedExits[region.Id].Add((i, j));
                        if (!HasAdjacentVisit(visitedExits[region.Id], i, j, isHorizontal))
                        {
                            this.uniqueExits[region.Id]++;
                        }
                    }

                    currentRegion = region.Id;
                }
            }
        }

        private bool IsExit(int i, int j, char regionType, bool horizontal)
        {
            if (horizontal)
            {
                return j + 1 == this.matrix[0].Length || this.matrix[i][j + 1] != regionType;
            }

            return i + 1 == this.matrix.Length || this.matrix[i + 1][j] != regionType;
        }

        private static bool HasAdjacentVisit(HashSet<(int, int)> visited, int i, int j, bool horizontal)
        {
            return horizontal
                ? visited.Contains((i - 1, j)) || visited.Contains((i + 1, j))
                : visited.Contains((i, j - 1)) || visited.Contains((i, j + 1));
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