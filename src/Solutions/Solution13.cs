using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution13 : ISolution
    {
        private struct Machine
        {
            public long aX;
            public long aY;
            public long bX;
            public long bY;
            public long targetX;
            public long targetY;
        }

        public string RunPartA(string inputData)
        {
            Machine[] machines = ParseInput(inputData);

            double totalCost = 0;

            for (int i = 0; i < machines.Length; i++)
            {
                (double a, double b) = SolveMachine(machines[i]);

                if (a % 1 == 0 && b % 1 == 0 && a <= 100 && b <= 100 && a >= 0 && b >= 0)
                {
                    totalCost += a * 3 + b;
                }
            }

            return totalCost.ToString();
        }

        public string RunPartB(string inputData)
        {
            Machine[] machines = ParseInput(inputData);

            double totalCost = 0;

            for (int i = 0; i < machines.Length; i++)
            {
                machines[i].targetX += 10000000000000;
                machines[i].targetY += 10000000000000;

                (double a, double b) = SolveMachine(machines[i]);

                if (a % 1 == 0 && b % 1 == 0 && a >= 0 && b >= 0)
                {
                    totalCost += a * 3 + b;
                }
            }

            return totalCost.ToString();
        }

        private static (double, double) SolveMachine(Machine machine)
        {
            double det = machine.aX * machine.bY - machine.bX * machine.aY;

            if (det == 0)
            {
                return (-1, -1);
            }

            double detA = machine.targetX * machine.bY - machine.bX * machine.targetY;
            double detB = machine.aX * machine.targetY - machine.targetX * machine.aY;

            double a = detA / det;
            double b = detB / det;

            return (a, b);
        }

        private static Machine[] ParseInput(string inputData)
        {
            List<Machine> machines = [];
            string[] lines = ParseUtils.ParseIntoLines(inputData);

            for (int i = 0; i < lines.Length; i += 3)
            {
                Machine curMachine = new();

                Regex buttonsRegex = new("Button [A|B]: X\\+(?<x>\\d+), Y\\+(?<y>\\d+)");

                MatchCollection aMatches = buttonsRegex.Matches(lines[i]);
                MatchCollection bMatches = buttonsRegex.Matches(lines[i + 1]);

                curMachine.aX = int.Parse(aMatches[0].Groups["x"].Value);
                curMachine.bX = int.Parse(bMatches[0].Groups["x"].Value);
                curMachine.aY = int.Parse(aMatches[0].Groups["y"].Value);
                curMachine.bY = int.Parse(bMatches[0].Groups["y"].Value);

                Regex targetRegex = new("Prize: X=(?<x>\\d+), Y=(?<y>\\d+)");

                MatchCollection targetMatches = targetRegex.Matches(lines[i + 2]);

                curMachine.targetX = int.Parse(targetMatches[0].Groups["x"].Value);
                curMachine.targetY = int.Parse(targetMatches[0].Groups["y"].Value);

                machines.Add(curMachine);
            }

            return machines.ToArray();
        }
    }
}