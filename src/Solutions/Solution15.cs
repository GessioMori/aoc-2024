using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution15 : ISolution
    {
        private struct Movement
        {
            public (int x, int y) from;
            public (int x, int y) to;
            public char fromChar;
        }

        private static readonly (int, int)[] directions = [(-1, 0), (0, 1), (1, 0), (0, -1)];
        private char[][] map = [];
        private (int, int)[] commands = [];
        private readonly HashSet<Movement> movementsToPerform = [];

        public string RunPartA(string inputData)
        {
            ParseInput(inputData, false);

            (int, int) currentPosition = FindInitialPosition();

            foreach ((int, int) command in this.commands)
            {
                if (MoveSmallObject(currentPosition, command))
                {
                    currentPosition = currentPosition.Add(command);

                    foreach (Movement movement in movementsToPerform)
                    {
                        this.map[movement.to.x][movement.to.y] = movement.fromChar;
                        this.map[movement.from.x][movement.from.y] = '.';
                    }
                }

                this.movementsToPerform.Clear();
            }

            return SumBoxesLocations().ToString();
        }

        public string RunPartB(string inputData)
        {
            ParseInput(inputData, true);

            (int, int) currentPosition = FindInitialPosition();

            foreach ((int, int) command in this.commands)
            {
                if (MoveLargeObject(currentPosition, command, false))
                {
                    currentPosition = currentPosition.Add(command);

                    foreach (Movement movement in movementsToPerform)
                    {
                        this.map[movement.to.x][movement.to.y] = movement.fromChar;
                        this.map[movement.from.x][movement.from.y] = '.';
                    }
                }

                this.movementsToPerform.Clear();
            }

            return SumBoxesLocations().ToString();
        }

        private long SumBoxesLocations()
        {
            long total = 0;

            for (int i = 0; i < this.map.Length; i++)
            {
                for (int j = 0; j < this.map[i].Length; j++)
                {
                    if (this.map[i][j] == 'O' || this.map[i][j] == '[')
                    {
                        total += 100 * i + j;
                    }
                }
            }

            return total;
        }

        private bool MoveSmallObject((int x, int y) initial, (int, int) direction)
        {
            (int x, int y) final = initial.Add(direction);
            char finalChar = this.map[final.x][final.y];

            if (finalChar == '#')
            {
                return false;
            }
            else if (finalChar == '.')
            {
                AddMovement(initial, final);
                return true;
            }

            if (MoveSmallObject(final, direction))
            {
                AddMovement(initial, final);
                return true;
            }

            return false;
        }

        private void AddMovement((int x, int y) initial, (int x, int y) final)
        {
            this.movementsToPerform.Add(new Movement
            {
                from = initial,
                to = final,
                fromChar = this.map[initial.x][initial.y],
            });
        }

        private bool MoveLargeObject((int x, int y) initial, (int x, int y) direction, bool isComplement)
        {
            (int x, int y) final = initial.Add(direction);
            char finalChar = this.map[final.x][final.y];
            char initialChar = this.map[initial.x][initial.y];
            bool isHorizontalMovement = direction.y != 0;

            if (isHorizontalMovement)
            {
                return MoveSmallObject(initial, direction);
            }

            if (finalChar == '#')
            {
                return false;
            }

            if (finalChar == '.')
            {
                if (isComplement || initialChar == '@')
                {
                    AddMovement(initial, final);
                    return true;
                }

                (int, int) complement = GetComplement(initial, initialChar);

                if (MoveLargeObject(complement, direction, true))
                {
                    AddMovement(initial, final);
                    return true;
                }

                return false;
            }

            if (isComplement)
            {
                if (MoveLargeObject(final, direction, false))
                {
                    AddMovement(initial, final);
                    return true;
                }

                return false;
            }

            if (initialChar == '[' || initialChar == ']')
            {
                (int, int) complement = GetComplement(initial, initialChar);
                if (MoveLargeObject(final, direction, false) && MoveLargeObject(complement, direction, true))
                {
                    AddMovement(initial, final);
                    return true;
                }
                return false;
            }

            if (MoveLargeObject(final, direction, false))
            {
                AddMovement(initial, final);
                return true;
            }

            return false;
        }

        private static (int, int) GetComplement((int x, int y) position, char initialChar)
        {
            int offset = initialChar == '[' ? 1 : -1;
            return (position.x, position.y + offset);
        }

        private (int, int) FindInitialPosition()
        {
            for (int i = 0; i < this.map.Length; i++)
            {
                for (int j = 0; j < this.map[i].Length; j++)
                {
                    if (this.map[i][j] == '@')
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        private void ParseInput(string inputData, bool isDoubleSize)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            string map = "";
            List<(int, int)> commandsList = [];

            foreach (string line in lines)
            {
                if (line.StartsWith('#'))
                {
                    if (!isDoubleSize)
                    {
                        map += line + Environment.NewLine;
                    }
                    else
                    {
                        map += line.Replace("#", "##")
                            .Replace(".", "..")
                            .Replace("O", "[]")
                            .Replace("@", "@.")
                            + Environment.NewLine;
                    }
                }
                else
                {
                    foreach (char d in line)
                    {
                        switch (d)
                        {
                            case '^':
                                commandsList.Add(directions[0]);
                                break;
                            case '>':
                                commandsList.Add(directions[1]);
                                break;
                            case 'v':
                                commandsList.Add(directions[2]);
                                break;
                            case '<':
                                commandsList.Add(directions[3]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            this.map = MatrixUtils.CreateCharMatrix(map);
            this.commands = commandsList.ToArray();
        }
    }
}