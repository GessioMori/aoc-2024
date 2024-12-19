using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution18 : ISolution
    {
        public string RunPartA(string inputData)
        {
            HashSet<(int, int)> corruptedPositions = ParseCoordinates(inputData);

            int corruptedNumber = 1024;
            int mapSize = 71;

            char[][] map = CreateMap(mapSize, corruptedPositions, corruptedNumber);

            int? minimalDistance = GetMinimalDistance(map);

            return minimalDistance.HasValue ? minimalDistance.Value.ToString() : "No path found";
        }

        public string RunPartB(string inputData)
        {
            HashSet<(int, int)> corruptedPositions = ParseCoordinates(inputData);

            int corruptedNumber = 1024;
            int mapSize = 71;

            char[][] map = CreateMap(mapSize, corruptedPositions, corruptedNumber);

            for (int i = corruptedNumber; i < corruptedPositions.Count; i++)
            {
                (int, int) corruptedPosition = corruptedPositions.ElementAt(i);

                map[corruptedPosition.Item2][corruptedPosition.Item1] = '#';

                int? minimalDistance = GetMinimalDistance(map);

                if (!minimalDistance.HasValue)
                {
                    return $"{corruptedPosition.Item1},{corruptedPosition.Item2}";
                }
            }

            throw new Exception("No solution found");
        }

        private static int? GetMinimalDistance(char[][] map)
        {
            (int, int) start = (0, 0);
            (int, int) end = (map.Length - 1, map[0].Length - 1);

            Dictionary<(int, int), int> visitedTilesByCost = [];
            PriorityQueue<(int, int), int> candidates = new();
            HashSet<(int, int)> visitedTiles = [];

            candidates.Enqueue(start, 0);
            visitedTilesByCost.Add((0, 0), 0);
            bool hasFoundEnd = false;

            while (candidates.Count > 0 && !hasFoundEnd)
            {
                (int, int) currentPos = candidates.Dequeue();

                if (visitedTiles.Contains(currentPos)) continue;

                visitedTiles.Add(currentPos);

                (int, int)[] neighbors = MatrixUtils.GetOrthogonalNeighbors(map, currentPos);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    (int x, int y) neighbor = neighbors[i];

                    if (map[neighbor.x][neighbor.y] == '#') continue;

                    int currentCost = visitedTilesByCost[currentPos] + 1;
                    int addedCost = currentCost + GetManhattanDistance(neighbor, end);

                    if (visitedTilesByCost.TryGetValue(neighbor, out int cost))
                    {
                        if (cost < currentCost)
                        {
                            continue;
                        }
                        else
                        {
                            visitedTilesByCost[neighbor] = currentCost;
                        }
                    }
                    else
                    {
                        visitedTilesByCost.Add(neighbor, currentCost);
                    }

                    if (neighbor == end)
                    {
                        hasFoundEnd = true;
                        break;
                    }
                    else
                    {
                        candidates.Enqueue(neighbor, addedCost);
                    }
                }
            }

            return visitedTilesByCost.TryGetValue(end, out int value) ? value : null;
        }

        private static char[][] CreateMap(int mapSize, HashSet<(int, int)> corruptedPositions, int numOfCorruptedPositions)
        {
            HashSet<(int, int)> firstCorruptedPositions = corruptedPositions.Take(numOfCorruptedPositions).ToHashSet();

            char[][] map = new char[mapSize][];

            for (int i = 0; i < mapSize; i++)
            {
                map[i] = new char[mapSize];
            }

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (firstCorruptedPositions.Contains((i, j)))
                    {
                        map[j][i] = '#';
                    }
                    else
                    {
                        map[j][i] = '.';
                    }
                }
            }

            return map;
        }

        private static int GetManhattanDistance((int x, int y) current, (int x, int y) end)
        {
            return Math.Abs(current.x - end.x) + Math.Abs(current.y - end.y);
        }

        private static HashSet<(int, int)> ParseCoordinates(string inputData)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);
            HashSet<(int, int)> coordinates = [];

            foreach (string line in lines)
            {
                string[] strings = line.Split(",");
                coordinates.Add((int.Parse(strings[0]), int.Parse(strings[1])));
            }

            return coordinates;
        }
    }
}