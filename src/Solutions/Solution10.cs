using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution10 : ISolution
    {
        public string RunPartA(string inputData)
        {
            return RunMap(inputData, true).ToString();
        }

        public string RunPartB(string inputData)
        {
            return RunMap(inputData, false).ToString();
        }

        private static long RunMap(string inputData, bool shouldCountUniqueEnds)
        {
            int[][] map = MatrixUtils.CreateIntMatrix(inputData);

            (int, int)[] trailheads = GetTrailheads(map);

            long totalScore = 0;

            foreach ((int, int) trailhead in trailheads)
            {
                totalScore += GetTrailheadScore(trailhead, map, shouldCountUniqueEnds);
            }

            return totalScore;
        }

        private static int GetTrailheadScore((int x, int y) trailhead, int[][] map, bool shouldCountUniqueEnds)
        {
            int score = 0;

            Queue<(int, int)> candidates = [];
            HashSet<(int x, int y)> visited = [];

            candidates.Enqueue(trailhead);

            while (candidates.Count > 0)
            {
                (int x, int y) tile = candidates.Dequeue();
                visited.Add(tile);

                foreach ((int x, int y) neighbor in MatrixUtils.GetOrthogonalNeighbors(map, tile))
                {
                    if (!visited.Contains(neighbor) &&
                        map[neighbor.x][neighbor.y] == map[tile.x][tile.y] + 1)
                    {
                        if (shouldCountUniqueEnds)
                        {
                            visited.Add(neighbor);
                        }

                        if (map[neighbor.x][neighbor.y] == 9)
                        {
                            score++;
                        }

                        candidates.Enqueue(neighbor);
                    }
                }
            }

            return score;
        }

        private static (int, int)[] GetTrailheads(int[][] map)
        {
            List<(int, int)> trailheads = [];

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 0)
                    {
                        trailheads.Add((i, j));
                    }
                }
            }

            return trailheads.ToArray();
        }
    }
}
