using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution23 : ISolution
    {
        public string RunPartA(string inputData)
        {
            HashSet<(string, string)> connections = GetConnectionsFromInput(inputData);
            Dictionary<string, List<string>> graph = GetAdjacencyList(connections);
            HashSet<(string, string)> visited = [];
            HashSet<string> uniqueSets = [];

            List<(string, string, string)> nodeGroups = [];

            Queue<(string, string)> queue = new();

            queue.Enqueue((connections.ElementAt(0).Item1, connections.ElementAt(0).Item2));

            while (queue.Count > 0)
            {
                (string prevNode, string node) = queue.Dequeue();

                if (visited.Contains((node, prevNode)))
                {
                    continue;
                }

                visited.Add((node, prevNode));

                foreach (string neighbour in graph[node])
                {
                    if (neighbour == prevNode)
                    {
                        continue;
                    }

                    if (graph[prevNode].Contains(neighbour))
                    {
                        string normalizedKey = string.Join(",", new[] { prevNode, node, neighbour }.OrderBy(x => x));

                        if (uniqueSets.Contains(normalizedKey))
                        {
                            continue;
                        }

                        uniqueSets.Add(normalizedKey);
                        nodeGroups.Add((prevNode, node, neighbour));
                    }

                    queue.Enqueue((node, neighbour));
                }
            }

            int totalTGroups = nodeGroups.Count(g => g.Item1.StartsWith('t') || g.Item2.StartsWith('t') || g.Item3.StartsWith('t'));

            return totalTGroups.ToString();
        }

        public string RunPartB(string inputData)
        {
            HashSet<(string, string)> connections = GetConnectionsFromInput(inputData);
            Dictionary<string, List<string>> graph = GetAdjacencyList(connections);
            List<string> nodes = graph.Keys.ToList();

            int currentMaxSubset = 0;
            string currentMaxSubsetString = "";

            for (int i = 0; i < nodes.Count; i++)
            {
                List<string> neighbors = graph[nodes[i]];

                List<List<string>> neighborCombinations = GenerateSubsets(neighbors);

                foreach (List<string> combination in neighborCombinations)
                {
                    if (combination.Count + 1 <= currentMaxSubset)
                    {
                        break;
                    }

                    if (CheckAllNeighborsSeeEachOther(combination, graph))
                    {
                        combination.Add(nodes[i]);
                        currentMaxSubset = combination.Count;
                        currentMaxSubsetString = string.Join(",", combination.Order());
                        break;
                    }
                }
            }

            return currentMaxSubsetString;
        }

        private static bool CheckAllNeighborsSeeEachOther(List<string> neighbors,
            Dictionary<string, List<string>> graph)
        {
            foreach (string neighbor in neighbors)
            {
                if (graph.ContainsKey(neighbor))
                {
                    foreach (string neighbor2 in neighbors)
                    {
                        if (neighbor != neighbor2 && !graph[neighbor].Contains(neighbor2))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static HashSet<(string, string)> GetConnectionsFromInput(string inputData)
        {
            HashSet<(string, string)> connections = [];

            foreach (string line in ParseUtils.ParseIntoLines(inputData))
            {
                string[] parts = line.Split("-");
                connections.Add((parts[0], parts[1]));
            }

            return connections;
        }

        private static Dictionary<string, List<string>> GetAdjacencyList(HashSet<(string, string)> connections)
        {
            Dictionary<string, List<string>> graph = [];

            foreach ((string a, string b) in connections)
            {
                if (!graph.TryGetValue(a, out List<string>? value))
                {
                    value = [];
                    graph[a] = value;
                }

                value.Add(b);

                if (!graph.TryGetValue(b, out List<string>? value2))
                {
                    value2 = [];
                    graph[b] = value2;
                }

                value2.Add(a);
            }
            return graph;
        }

        private static List<List<string>> GenerateSubsets(List<string> input)
        {
            List<List<string>> subsets = [];
            int count = 1 << input.Count;

            for (int i = 1; i < count; i++)
            {
                List<string> subset = [];

                for (int j = 0; j < input.Count; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        subset.Add(input[j]);
                    }
                }
                subsets.Add(subset);
            }

            return subsets.OrderByDescending(s => s.Count).ToList();
        }
    }
}