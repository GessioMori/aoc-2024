using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution14 : ISolution
    {
        const int gridWidth = 101;
        const int gridHeight = 103;

        private struct Robot
        {
            public int p0x;
            public int p0y;
            public int vx;
            public int vy;
        }

        public string RunPartA(string inputData)
        {
            Robot[] robots = ParseInput(inputData);

            int[] quadrantsCount = new int[4];

            (int, int)[] finalPositions = GetRobotPositions(robots, 100);

            foreach ((int px, int py) in finalPositions)
            {
                if (px != gridWidth / 2 && py != gridHeight / 2)
                {
                    if (py < gridHeight / 2)
                    {
                        if (px < gridWidth / 2)
                        {
                            quadrantsCount[0]++;
                        }
                        else
                        {
                            quadrantsCount[1]++;
                        }
                    }
                    else
                    {
                        if (px < gridWidth / 2)
                        {
                            quadrantsCount[2]++;
                        }
                        else
                        {
                            quadrantsCount[3]++;
                        }
                    }
                }
            }

            return quadrantsCount.Aggregate(1, (acc, cur) => acc * cur).ToString();
        }

        public string RunPartB(string inputData)
        {
            Robot[] robots = ParseInput(inputData);

            int statesCount = 10000;

            double currentMin = double.MaxValue;
            double currentMinIdx = 0;

            for (int i = 0; i < statesCount; i++)
            {
                double entropy = CalculateStateEntropy(robots, i + 1);

                if (entropy < currentMin)
                {
                    currentMin = entropy;
                    currentMinIdx = i;
                }
            }

            return (currentMinIdx + 1).ToString();
        }

        private static double CalculateStateEntropy(Robot[] robots, int deltaT)
        {
            (int, int)[] finalPositions = GetRobotPositions(robots, deltaT);
            int[,] matrix = new int[gridHeight, gridWidth];

            foreach ((int x, int y) in finalPositions)
            {
                matrix[y, x] = 1;
            }

            Dictionary<string, int> transitionCounts = new()
            {
                { "1->1", 0 },
                { "1->0", 0 },
                { "0->1", 0 },
                { "0->0", 0 }
            };

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    if (j + 1 < gridWidth)
                    {
                        CountTransition(matrix[i, j], matrix[i, j + 1], transitionCounts);
                    }

                    if (i + 1 < gridHeight)
                    {
                        CountTransition(matrix[i, j], matrix[i + 1, j], transitionCounts);
                    }

                    if (i + 1 < gridHeight && j + 1 < gridWidth)
                    {
                        CountTransition(matrix[i, j], matrix[i + 1, j + 1], transitionCounts);
                    }

                    if (i + 1 < gridHeight && j - 1 >= 0)
                    {
                        CountTransition(matrix[i, j], matrix[i + 1, j - 1], transitionCounts);
                    }
                }
            }

            int totalTransitions = 0;
            foreach (int count in transitionCounts.Values)
            {
                totalTransitions += count;
            }

            Dictionary<string, double> probabilities = [];

            foreach (KeyValuePair<string, int> transition in transitionCounts)
            {
                probabilities[transition.Key] = (double)transition.Value / totalTransitions;
            }

            double entropy = 0;
            foreach (double p in probabilities.Values)
            {
                entropy -= p * Math.Log2(p);
            }

            return entropy;
        }

        private static void CountTransition(int current, int next, Dictionary<string, int> transitionCounts)
        {
            transitionCounts[$"{current}->{next}"]++;
        }

        private static (int x, int y)[] GetRobotPositions(Robot[] robots, int deltaTime)
        {
            List<(int, int)> positions = [];

            foreach (Robot robot in robots)
            {
                (int px, int py) = CalculateRobotPosition(robot, deltaTime);

                positions.Add((px, py));
            }

            return positions.ToArray();
        }

        private static (int px, int py) CalculateRobotPosition(Robot robot, int deltaTime)
        {
            int px = (robot.p0x + robot.vx * deltaTime) % gridWidth;
            int py = (robot.p0y + robot.vy * deltaTime) % gridHeight;

            px = (px + gridWidth) % gridWidth;
            py = (py + gridHeight) % gridHeight;

            return (px, py);
        }

        private static Robot[] ParseInput(string inputData)
        {
            List<Robot> result = [];

            string[] lines = ParseUtils.ParseIntoLines(inputData);

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, "p=(\\d+),(\\d+) v=(-?\\d+),(-?\\d+)");

                result.Add(new Robot
                {
                    p0x = int.Parse(match.Groups[1].Value),
                    p0y = int.Parse(match.Groups[2].Value),
                    vx = int.Parse(match.Groups[3].Value),
                    vy = int.Parse(match.Groups[4].Value),
                });
            }

            return result.ToArray();
        }
    }
}