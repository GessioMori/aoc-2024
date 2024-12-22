using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Text;

namespace aoc_2024.Solutions
{
    public class Solution21 : ISolution
    {
        private readonly Dictionary<char, (int, int)> numpadCoordinates =
            new() {
                {'1', (2,0)},
                {'2', (2,1)},
                {'3', (2,2)},
                {'4', (1,0)},
                {'5', (1,1)},
                {'6', (1,2)},
                {'7', (0,0)},
                {'8', (0,1)},
                {'9', (0,2)},
                {'0', (3,1)},
                {'A', (3,2)}
            };

        private readonly Dictionary<char, (int, int)> directionalPadCoordinates =
            new()
            {
                {'^', (0, 1)},
                {'A', (0, 2)},
                {'<', (1, 0)},
                {'v', (1, 1)},
                {'>', (1, 2)}
            };

        private readonly Dictionary<(char, char), string[]> directionalPadMovs = [];

        private readonly Dictionary<(char, char), string[]> numpadMovs = [];

        private readonly Dictionary<(char, char, int), long> numOfCommands = [];

        private string[] GetMovements(char from, char to, bool isNumpad)
        {
            List<string> movements = [];

            (int x, int y) fromCoord = isNumpad ? numpadCoordinates[from] : directionalPadCoordinates[from];
            (int x, int y) toCoord = isNumpad ? numpadCoordinates[to] : directionalPadCoordinates[to];

            int xDiff = toCoord.x - fromCoord.x;
            int yDiff = toCoord.y - fromCoord.y;

            string xMove = xDiff switch
            {
                0 => "",
                > 0 => new string('v', xDiff),
                < 0 => new string('^', -xDiff)
            };

            string yMove = yDiff switch
            {
                0 => "",
                > 0 => new string('>', yDiff),
                < 0 => new string('<', -yDiff)
            };

            if (IsVerticalFirstValid(from, to, isNumpad))
            {
                movements.Add(xMove + yMove);
            }

            if (IsHorizontalFirstValid(from, to, isNumpad))
            {
                movements.Add(yMove + xMove);
            }

            if (movements.Count == 2 && movements[0] == movements[1])
            {
                movements.RemoveAt(1);
            }

            return movements.ToArray();
        }

        private static bool IsVerticalFirstValid(char from, char to, bool isNumpad)
        {
            if (isNumpad)
            {
                return !((from == '7' || from == '4' || from == '1') && (to == '0' || to == 'A'));
            }
            else
            {
                return !(from == '<' && (to == '^' || to == 'A'));
            }
        }

        private static bool IsHorizontalFirstValid(char from, char to, bool isNumpad)
        {
            if (isNumpad)
            {
                return !((to == '7' || to == '4' || to == '1') && (from == '0' || from == 'A'));
            }
            else
            {
                return !(to == '<' && (from == '^' || from == 'A'));
            }
        }

        private void InitiateMovs()
        {
            List<char> numpadCoordinatesKeys = numpadCoordinates.Keys.ToList();

            for (int i = 0; i < numpadCoordinatesKeys.Count; i++)
            {
                for (int j = 0; j < numpadCoordinatesKeys.Count; j++)
                {
                    numpadMovs.Add((numpadCoordinatesKeys[i], numpadCoordinatesKeys[j]),
                        GetMovements(numpadCoordinatesKeys[i], numpadCoordinatesKeys[j], true));
                }
            }

            List<char> directionalPadCoordinatesKeys = directionalPadCoordinates.Keys.ToList();

            for (int i = 0; i < directionalPadCoordinatesKeys.Count; i++)
            {
                for (int j = 0; j < directionalPadCoordinatesKeys.Count; j++)
                {
                    directionalPadMovs.Add((directionalPadCoordinatesKeys[i], directionalPadCoordinatesKeys[j]),
                        GetMovements(directionalPadCoordinatesKeys[i], directionalPadCoordinatesKeys[j], false));
                }
            }
        }

        private long GetNumberOfCommands(char from, char to, int currentDepth, int maxDepth)
        {
            if (numOfCommands.ContainsKey((from, to, currentDepth)))
            {
                return numOfCommands[(from, to, currentDepth)];
            }

            string[] commands = directionalPadMovs[(from, to)];

            if (string.IsNullOrEmpty(commands[0]))
            {
                return 1;
            }

            if (currentDepth == maxDepth)
            {
                return commands[0].Length + 1;
            }

            long minimumCommands = long.MaxValue;

            foreach (string command in commands)
            {
                long commandTotal = 0;
                char currentPosition = 'A';

                foreach (char c in command)
                {
                    commandTotal += GetNumberOfCommands(currentPosition, c, currentDepth + 1, maxDepth);
                    currentPosition = c;
                }

                commandTotal += GetNumberOfCommands(currentPosition, 'A', currentDepth + 1, maxDepth);

                if (commandTotal < minimumCommands)
                {
                    minimumCommands = commandTotal;
                    numOfCommands[(from, to, currentDepth)] = minimumCommands;
                }
            }

            return minimumCommands;
        }

        private string[] GetDirectionsFromNumpad(char initialKey, string command)
        {
            List<string> movementSequences = [];
            StringBuilder directions = new();
            GenerateCombinations(initialKey, command, 0, directions, movementSequences);
            return movementSequences.ToArray();
        }

        private void GenerateCombinations(
            char currentKey,
            string command,
            int index,
            StringBuilder directions,
            List<string> movementSequences)
        {
            if (index >= command.Length)
            {
                movementSequences.Add(directions.ToString());
                return;
            }

            char nextKey = command[index];

            string[] movs = numpadMovs[(currentKey, nextKey)];

            foreach (string move in movs)
            {
                directions.Append(move);
                directions.Append('A');
                GenerateCombinations(nextKey, command, index + 1, directions, movementSequences);
                directions.Length -= move.Length + 1;
            }
        }

        private long GetMinimumCommandsForInput(string input, int depth)
        {
            long minimumCommands = long.MaxValue;

            string[] dirs = GetDirectionsFromNumpad('A', input);

            foreach (string dir in dirs)
            {
                long total = 0;

                char currentPosition = 'A';

                foreach (char c in dir)
                {
                    total += GetNumberOfCommands(currentPosition, c, 1, depth);
                    currentPosition = c;
                }

                if (total < minimumCommands)
                {
                    minimumCommands = total;
                }
            }

            return minimumCommands;
        }

        public string RunPartA(string inputData)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            InitiateMovs();

            long complexities = 0;

            foreach (string line in lines)
            {
                complexities += GetMinimumCommandsForInput(line, 2) * int.Parse(line[..3]);
            }

            return complexities.ToString();
        }

        public string RunPartB(string inputData)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            InitiateMovs();

            long complexities = 0;

            foreach (string line in lines)
            {
                complexities += GetMinimumCommandsForInput(line, 25) * int.Parse(line[..3]);
            }

            return complexities.ToString();
        }
    }
}