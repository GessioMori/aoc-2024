using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution10 : ISolution
    {
        public string RunPartA(string inputData)
        {
            int[][] map = MatrixUtils.CreateIntMatrix(inputData);

            (int, int)[] trailheads = GetTrailheads(map);

            long totalScore = 0;

            foreach ((int, int) trailhead in trailheads)
            {
                totalScore += GetTrailheadScore(trailhead, map);
            }

            return totalScore.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }

        private static int GetTrailheadScore((int, int) trailhead, int[][] map)
        {
            int score = 0;

            Queue<(int, int)> candidates = [];
            HashSet<(int, int)> visited = [];

            candidates.Enqueue(trailhead);

            while (candidates.Count > 0)
            {
                (int, int) tile = candidates.Dequeue();

                foreach ((int, int) neighbor in MatrixUtils.GetIntOrthogonalNeighbors(map, tile))
                {
                    Console.WriteLine(neighbor);
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

        //private (int, int)[] GetTrailheads(int[][] map)
        //{
        //    List<(int, int)> trailheads = [];

        //    int numOfRows = map.Length;
        //    int numOfCols = map[0].Length;

        //    for (int i = 0; i < numOfRows; i++)
        //    {
        //        if (map[i][0] == 0)
        //        {
        //            trailheads.Add((i, 0));
        //        }

        //        if (map[i][numOfCols - 1] == 0)
        //        {
        //            trailheads.Add((i, numOfCols - 1));
        //        }
        //    }

        //    for (int i = 0; i < numOfCols; i++)
        //    {
        //        if (map[0][i] == 0)
        //        {
        //            trailheads.Add((0, i));
        //        }

        //        if (map[numOfRows - 1][i] == 0)
        //        {
        //            trailheads.Add((numOfRows - 1, i));
        //        }
        //    }

        //    return trailheads.ToArray();
        //}
    }
}
