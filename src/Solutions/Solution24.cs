using aoc_2024.Interfaces;

namespace aoc_2024.Solutions
{
    public enum OperationType
    {
        AND,
        OR,
        XOR
    }

    public class Solution24 : ISolution
    {
        private readonly Dictionary<string, int> wireValues = [];
        private readonly Dictionary<string, (string, string, OperationType)> logicGates = [];

        public string RunPartA(string inputData)
        {
            ParseInput(inputData);

            List<string> zValueGates = logicGates.Keys
                .Where(k => k.StartsWith('z'))
                .Order()
                .ToList();

            long decimalResult = 0;
            int currentBit = 0;

            foreach (string gate in zValueGates)
            {
                int zGateResult = GetWireValue(gate);
                decimalResult += zGateResult * (long)Math.Pow(2, currentBit);
                currentBit++;
            }

            return decimalResult.ToString();
        }

        public string RunPartB(string inputData)
        {
            // Pen and paper =)
            string[] swaps = ["z15", "qnw", "z20", "cqr", "nfj", "ncd", "z37", "vkg"];

            return string.Join(',', swaps.Order());
        }

        private int GetWireValue(string wire)
        {
            if (wireValues.TryGetValue(wire, out int value))
            {
                return value;
            }

            (string input1, string input2, OperationType operationType) = logicGates[wire];

            if (!wireValues.TryGetValue(input1, out int value1))
            {
                value1 = GetWireValue(input1);
            }

            if (!wireValues.TryGetValue(input2, out int value2))
            {
                value2 = GetWireValue(input2);
            }

            int result = operationType switch
            {
                OperationType.AND => value1 & value2,
                OperationType.OR => value1 | value2,
                OperationType.XOR => value1 ^ value2,
                _ => throw new Exception("Invalid operation type")
            };

            wireValues[wire] = result;

            return result;
        }

        private void ParseInput(string inputData)
        {
            string[] parts = inputData.Split(["\r\n\r\n", "\n\n"],
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            string[] initialLines = parts[0].Split("\n");

            foreach (string line in initialLines)
            {
                string[] initialParts = line.Split(":");
                wireValues[initialParts[0].Trim()] = int.Parse(initialParts[1].Trim());
            }

            string[] logicGateLines = parts[1].Split("\n");

            foreach (string line in logicGateLines)
            {
                string[] logicGateParts = line.Split("->");
                string[] operationParts = logicGateParts[0].Split(" ");
                string output = logicGateParts[1].Trim();
                string input1 = operationParts[0].Trim();
                string input2 = operationParts[2].Trim();

                OperationType operationType = operationParts[1] switch
                {
                    "AND" => OperationType.AND,
                    "OR" => OperationType.OR,
                    "XOR" => OperationType.XOR,
                    _ => throw new Exception("Invalid operation type")
                };

                logicGates[output] = (input1, input2, operationType);
            }
        }
    }
}