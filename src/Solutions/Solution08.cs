using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution08 : ISolution
    {
        public string RunPartA(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            Dictionary<char, List<(int, int)>> sameFrequencyLists = GetAntennas(matrix);

            HashSet<(int, int)> nodes = [];

            foreach (KeyValuePair<char, List<(int, int)>> sameFrequencyList in sameFrequencyLists)
            {
                CreateValidNodesCoords(matrix.Length, matrix[0].Length, sameFrequencyList.Value, nodes, true);
            }

            return nodes.Count.ToString();
        }

        public string RunPartB(string inputData)
        {
            char[][] matrix = MatrixUtils.CreateCharMatrix(inputData);

            Dictionary<char, List<(int, int)>> antenas = GetAntennas(matrix);

            HashSet<(int, int)> nodes = [];

            foreach (KeyValuePair<char, List<(int, int)>> antena in antenas)
            {
                CreateValidNodesCoords(matrix.Length, matrix[0].Length, antena.Value, nodes, false);
            }

            return nodes.Count.ToString();
        }

        private static void CreateValidNodesCoords(int height, int width, List<(int, int)> listOfAntennas,
            HashSet<(int, int)> nodes, bool shouldGetPair)
        {
            for (int i = 0; i < listOfAntennas.Count; i++)
            {
                for (int j = i + 1; j < listOfAntennas.Count; j++)
                {
                    (int, int) difference = listOfAntennas[i].Add(listOfAntennas[j].Mult(-1));

                    if (shouldGetPair)
                    {
                        int[] specificSteps = [1, -2];

                        foreach (int step in specificSteps)
                        {
                            (int, int) node = listOfAntennas[i].Add(difference.Mult(step));
                            CheckAndAddNode(height, width, node, nodes);
                        }
                    }
                    else
                    {
                        for (int direction = 1; direction >= -1; direction -= 2)
                        {
                            int step = 0;
                            while (true)
                            {
                                (int, int) node = listOfAntennas[i].Add(difference.Mult(step * direction));

                                if (!CheckAndAddNode(height, width, node, nodes))
                                {
                                    break;
                                }

                                step++;
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckAndAddNode(int height, int width, (int, int) node, HashSet<(int, int)> nodes)
        {
            if (node.Item1 >= 0 && node.Item1 < height &&
                node.Item2 >= 0 && node.Item2 < width)
            {
                nodes.Add(node);
                return true;
            }

            return false;
        }

        private static Dictionary<char, List<(int, int)>> GetAntennas(char[][] matrix)
        {
            Dictionary<char, List<(int, int)>> antennas = [];

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[0].Length; j++)
                {
                    char tile = matrix[i][j];
                    if (tile != '.')
                    {
                        if (!antennas.TryGetValue(tile, out List<(int, int)>? coordList))
                        {
                            antennas.Add(tile, [(i, j)]);
                        }
                        else
                        {
                            coordList.Add((i, j));
                        }
                    }
                }
            }

            return antennas;
        }
    }
}