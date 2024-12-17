using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution16 : ISolution
    {
        private char[][] map = [];
        public string RunPartA(string inputData)
        {
            this.map = MatrixUtils.CreateCharMatrix(inputData);

            return GetLowestCost().ToString();
        }

        public string RunPartB(string inputData)
        {
            this.map = MatrixUtils.CreateCharMatrix(inputData);
            int lowestCost = GetLowestCost();

            ((int x, int y) start, (int x, int y) end) = GetStartAndEnd();

            Dictionary<(int, int, (int, int)), int> visitedStatesByCost = [];
            PriorityQueue<(int, int, (int, int)), int> candidates = new();
            Dictionary<(int, int, (int, int)), HashSet<(int, int, (int, int), int)>> previousStates = [];

            candidates.Enqueue((start.x, start.y, (0, 1)), 0);
            visitedStatesByCost.Add((start.x, start.y, (0, 1)), 0);

            while (candidates.Count > 0)
            {
                (int x, int y, (int, int) dir) state = candidates.Dequeue();

                (int, int)[] neighbors = MatrixUtils.GetOrthogonalNeighbors(this.map, (state.x, state.y));

                for (int i = 0; i < neighbors.Length; i++)
                {
                    (int x, int y) newPosition = neighbors[i];

                    if (this.map[newPosition.x][newPosition.y] == '#') continue;

                    int currentCost = visitedStatesByCost[(state.x, state.y, state.dir)] + 1;

                    (int x, int y) newDirection = newPosition.Add((state.x, state.y).Mult(-1));

                    if (newDirection != state.dir)
                    {
                        currentCost += 1000;
                    }

                    if (currentCost > lowestCost) continue;

                    if (visitedStatesByCost.TryGetValue((newPosition.x, newPosition.y, newDirection), out int cost))
                    {
                        if (cost < currentCost)
                        {
                            continue;
                        }
                        else
                        {
                            visitedStatesByCost[(newPosition.x, newPosition.y, newDirection)] = currentCost;
                            previousStates[(newPosition.x, newPosition.y, newDirection)] =
                                [..previousStates[(newPosition.x, newPosition.y, newDirection)].Where(prevState => prevState.Item4 <= currentCost),
                                (state.x, state.y, state.dir, currentCost)];
                        }
                    }
                    else
                    {
                        visitedStatesByCost.Add((newPosition.x, newPosition.y, newDirection), currentCost);
                        previousStates.Add((newPosition.x, newPosition.y, newDirection), [(state.x, state.y, state.dir, currentCost)]);
                    }

                    if (newPosition != end)
                    {
                        candidates.Enqueue((newPosition.x, newPosition.y, newDirection), currentCost);
                    }
                }
            }

            HashSet<(int, int)> bestSeats = [(end.x, end.y)];

            IEnumerable<(int, int, (int, int))> finalStates = previousStates.Keys.Where(k => k.Item1 == end.x && k.Item2 == end.y);

            foreach ((int x, int y, (int, int) dir) finalState in finalStates)
            {
                AddPathToBestSeats(bestSeats, previousStates, (finalState.x, finalState.y, finalState.dir, lowestCost));
            }

            return bestSeats.Count.ToString();
        }

        private static void AddPathToBestSeats(HashSet<(int, int)> bestSeats,
            Dictionary<(int, int, (int, int)), HashSet<(int, int, (int, int), int)>> dictPreviousStates,
            (int, int, (int, int), int) currentState)
        {
            if (dictPreviousStates.TryGetValue((currentState.Item1, currentState.Item2, currentState.Item3),
                out HashSet<(int, int, (int, int), int)>? previousStates))
            {
                foreach ((int x, int y, (int, int), int) state in previousStates)
                {
                    bestSeats.Add((state.x, state.y));
                    AddPathToBestSeats(bestSeats, dictPreviousStates, state);
                }
            }
        }

        private int GetLowestCost()
        {
            ((int x, int y) start, (int x, int y) end) = GetStartAndEnd();

            Dictionary<(int, int, (int, int)), int> visitedTilesByCost = [];
            PriorityQueue<(int, int, (int, int)), int> candidates = new();

            candidates.Enqueue((start.x, start.y, (0, 1)), 0);
            visitedTilesByCost.Add((start.x, start.y, (0, 1)), 0);
            bool hasFoundEnd = false;

            while (candidates.Count > 0 && !hasFoundEnd)
            {
                (int x, int y, (int, int) dir) state = candidates.Dequeue();

                (int, int)[] neighbors = MatrixUtils.GetOrthogonalNeighbors(this.map, (state.x, state.y));

                for (int i = 0; i < neighbors.Length; i++)
                {
                    (int x, int y) neighbor = neighbors[i];

                    if (this.map[neighbor.x][neighbor.y] == '#') continue;

                    int costToChange = 1;
                    int currentCost = visitedTilesByCost[state];

                    (int x, int y) newDirection = neighbor.Add((state.x, state.y).Mult(-1));

                    if (newDirection != state.dir)
                    {
                        costToChange += 1000;
                    }

                    if (visitedTilesByCost.TryGetValue((neighbor.x, neighbor.y, newDirection), out int cost))
                    {
                        if (cost < costToChange + currentCost)
                        {
                            continue;
                        }
                        else
                        {
                            visitedTilesByCost[(neighbor.x, neighbor.y, newDirection)] = currentCost + costToChange;
                        }
                    }
                    else
                    {
                        visitedTilesByCost.Add((neighbor.x, neighbor.y, newDirection), currentCost + costToChange);
                    }

                    if (neighbor == end)
                    {
                        hasFoundEnd = true;
                        break;
                    }
                    else
                    {
                        candidates.Enqueue((neighbor.x, neighbor.y, newDirection), currentCost + costToChange);
                    }
                }
            }

            (int, int, (int, int)) finalState = visitedTilesByCost.Keys.First(t => t.Item1 == end.x && t.Item2 == end.y);

            return visitedTilesByCost[finalState];
        }

        private ((int, int), (int, int)) GetStartAndEnd()
        {
            (int, int) start = (-1, -1);
            (int, int) end = (-1, -1);

            for (int i = 0; i < this.map.Length; i++)
            {
                for (int j = 0; j < this.map[i].Length; j++)
                {
                    if (this.map[i][j] == 'S')
                    {
                        start = (i, j);
                    }
                    else if (this.map[i][j] == 'E')
                    {
                        end = (i, j);
                    }
                }
            }

            return (start, end);
        }
    }
}