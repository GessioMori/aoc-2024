using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution20 : ISolution
    {
        public string RunPartA(string inputData)
        {
            return CountValidCheats(inputData, 2).ToString();
        }

        public string RunPartB(string inputData)
        {
            return CountValidCheats(inputData, 20).ToString();
        }

        private static int CountValidCheats(string inputData, int cheatLength)
        {
            char[][] map = MatrixUtils.CreateCharMatrix(inputData);

            ((int x, int y) start, (int x, int y) end) = GetStartAndEnd(map);

            Dictionary<(int, int), int> path = GetPath(map, start, end);
            (int, int)[] visited = path.Keys.ToArray();

            int minSavedTime = 100;
            int validCheats = 0;

            for (int i = 0; i < visited.Length; i++)
            {
                for (int j = i + 1; j < visited.Length; j++)
                {
                    (int x, int y) s = visited[i];
                    (int x, int y) e = visited[j];

                    int manhattanDistance = GetManhattanDistance(s, e);

                    if (manhattanDistance > cheatLength)
                    {
                        continue;
                    }

                    int sCost = path[(s.x, s.y)];
                    int eCost = path[(e.x, e.y)];

                    if (sCost + manhattanDistance - eCost + minSavedTime <= 0)
                    {
                        validCheats++;
                    }
                }
            }

            return validCheats;
        }

        private static int GetManhattanDistance((int x, int y) start, (int x, int y) end)
        {
            return Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y);
        }

        private static Dictionary<(int, int), int> GetPath(char[][] map, (int, int) start, (int, int) end)
        {
            Dictionary<(int, int), int> distances = [];
            distances[start] = 0;
            Queue<(int, int)> queue = [];
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                (int x, int y) current = queue.Dequeue();
                foreach ((int x, int y) neighbor in MatrixUtils.GetOrthogonalNeighbors(map, current))
                {
                    if (map[neighbor.x][neighbor.y] == '#')
                    {
                        continue;
                    }
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = distances[current] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return distances;
        }

        private static ((int, int) start, (int, int) end) GetStartAndEnd(char[][] map)
        {
            int numOfRows = map.Length;
            int numOfColumns = map[0].Length;

            (int, int) start = (-1, -1);
            (int, int) end = (-1, -1);

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfColumns; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        start = (i, j);
                    }
                    else if (map[i][j] == 'E')
                    {
                        end = (i, j);
                    }
                }
            }
            return (start, end);
        }
    }
}