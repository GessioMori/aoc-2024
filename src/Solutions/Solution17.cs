using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution17 : ISolution
    {
        public string RunPartA(string inputData)
        {
            (int a, int b, int c, int[] program) = ParseInput(inputData);

            Computer computer = new(a, b, c, program);

            List<long> results = [];

            long? result = -1;

            while (result.HasValue)
            {
                result = computer.Run();

                if (result.HasValue && result != -1)
                {
                    results.Add(result.Value);
                }
            }

            return string.Join(",", results);
        }

        public string RunPartB(string inputData)
        {
            (int _, int _, int _, int[] program) = ParseInput(inputData);

            Computer computer = new(0, 0, 0, program);

            long? result = computer.RecursiveAValue(0, program.Length - 1);

            return result.HasValue ? result.Value.ToString() : "No solution found";
        }

        private static (int a, int b, int c, int[] program) ParseInput(string inputData)
        {
            string[] lines = ParseUtils.ParseIntoLines(inputData);
            int[] registers = new int[4];
            int[] program = [];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (i < 3)
                {
                    registers[i] = int.Parse(parts[1]);
                }
                else
                {
                    program = parts[1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(int.Parse)
                        .ToArray();
                }
            }

            return (registers[0], registers[1], registers[2], program);
        }
    }

    public class Computer
    {
        private long aRegister;
        private long bRegister;
        private long cRegister;
        private readonly int[] program;
        private int currentPosition;

        public Computer(long aRegister, int bRegister, int cRegister, int[] program)
        {
            this.aRegister = aRegister;
            this.bRegister = bRegister;
            this.cRegister = cRegister;
            this.program = program;
            this.currentPosition = 0;
        }

        public long? Run()
        {
            if (this.currentPosition >= this.program.Length)
            {
                return null;
            }

            int opcode = this.program[this.currentPosition];

            switch (opcode)
            {
                case 0:
                    this.aRegister = (int)(this.aRegister / Math.Pow(2, GetComboOperandValue(this.program[this.currentPosition + 1])));
                    this.currentPosition += 2;
                    break;
                case 1:
                    this.bRegister ^= this.program[this.currentPosition + 1];
                    this.currentPosition += 2;
                    break;
                case 2:
                    this.bRegister = GetComboOperandValue(this.program[this.currentPosition + 1]) % 8;
                    this.currentPosition += 2;
                    break;
                case 3:
                    if (this.aRegister != 0)
                    {
                        this.currentPosition = this.program[this.currentPosition + 1];
                    }
                    else
                    {
                        this.currentPosition += 2;
                    }
                    break;
                case 4:
                    this.bRegister ^= this.cRegister;
                    this.currentPosition += 2;
                    break;
                case 5:
                    long returnValue = GetComboOperandValue(this.program[this.currentPosition + 1]) % 8;
                    this.currentPosition += 2;
                    return returnValue;
                case 6:
                    this.bRegister = (int)(this.aRegister / Math.Pow(2, GetComboOperandValue(this.program[this.currentPosition + 1])));
                    this.currentPosition += 2;
                    break;
                case 7:
                    this.cRegister = (int)(this.aRegister / Math.Pow(2, GetComboOperandValue(this.program[this.currentPosition + 1])));
                    this.currentPosition += 2;
                    break;
                default:
                    break;
            }

            return -1;
        }

        public long? RecursiveAValue(long currentResult, int currentIndex)
        {
            if (currentIndex < 0)
            {
                return currentResult;
            }

            for (int b = 0; b < 8; b++)
            {
                long curB = b;
                long a = (long)(currentResult * 8) + curB;
                curB ^= 2;
                long c = (long)(a / Math.Pow(2, curB));
                curB ^= 3;
                curB ^= c;
                if (curB % 8 == this.program[currentIndex])
                {
                    long? subAnswer = RecursiveAValue(a, currentIndex - 1);
                    if (!subAnswer.HasValue) continue;
                    return subAnswer;
                }
            }

            return null;
        }

        private long GetComboOperandValue(int operand)
        {
            return operand switch
            {
                0 or 1 or 2 or 3 => operand,
                4 => this.aRegister,
                5 => this.bRegister,
                6 => this.cRegister,
                _ => throw new NotImplementedException(),
            };
        }
    }
}