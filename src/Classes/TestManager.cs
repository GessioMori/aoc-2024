using aoc_2024.Interfaces;

namespace aoc_2024.Classes
{
    public class TestManager : ITestManager
    {
        private readonly ILogger logger;
        private readonly int[] availableTests;
        private static readonly string[] keywords = ["TestNumber=", "AnswerA=", "AnswerB=", "Input="];

        public TestManager(ILogger logger)
        {
            this.logger = logger;
            this.availableTests = LoadAvailableTests();
        }

        public int[] GetAvailableTests()
        {
            return this.availableTests;
        }

        public List<TestCase> Parse(int dayNumber)
        {
            List<TestCase> testCases = [];
            string[] lines = ReadTestFile(dayNumber);
            TestCase currentTest = new();
            string currentKey = string.Empty;
            bool isReadingInput = false;

            foreach (string line in lines)
            {
                if (line.Trim() == "================================")
                {
                    if (currentTest.TestNumber != 0)
                    {
                        if (IsValidTestCase(currentTest))
                        {
                            currentTest.Input = currentTest.Input.TrimEnd();
                            testCases.Add(currentTest);
                        }
                        else
                        {
                            this.logger.Log($"Test #{currentTest.TestNumber} is invalid.", LogSeverity.Error);
                        }

                        currentTest = new TestCase();
                    }
                    continue;
                }

                if (keywords.Any(line.Contains))
                {
                    string[] parts = line.Split('=', 2);
                    currentKey = parts[0].Trim();

                    if (currentKey == "Input")
                    {
                        isReadingInput = true;
                        currentTest.Input = string.Empty;
                    }
                    else
                    {
                        isReadingInput = false;
                    }
                }
                else
                {
                    string trimmedLine = line.Trim();

                    if (isReadingInput)
                    {
                        if (trimmedLine != "[Empty]")
                        {
                            if (!string.IsNullOrEmpty(currentTest.Input))
                            {
                                currentTest.Input += Environment.NewLine;
                            }
                            currentTest.Input += trimmedLine;
                        }
                    }
                    else
                    {
                        switch (currentKey)
                        {
                            case "TestNumber":
                                currentTest.TestNumber = int.TryParse(trimmedLine, out int number) ? number : 0;
                                break;
                            case "AnswerA":
                                currentTest.AnswerA = trimmedLine == "[Empty]" ? null : trimmedLine;
                                break;
                            case "AnswerB":
                                currentTest.AnswerB = trimmedLine == "[Empty]" ? null : trimmedLine;
                                break;
                        }
                    }
                }
            }

            if (currentTest.TestNumber != 0)
            {
                if (string.IsNullOrWhiteSpace(currentTest.Input))
                {
                    throw new InvalidDataException($"Test #{currentTest.TestNumber} has an empty Input.");
                }
                testCases.Add(currentTest);
            }

            return testCases;
        }

        private string[] ReadTestFile(int dayNumber)
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath))
            {
                this.logger.Log("Tests folder not found", LogSeverity.Error);
                return [];
            }

            string filePath = Path.Combine(basePath, "Tests", $"test-{dayNumber.ToString().PadLeft(2, '0')}.txt");

            if (!File.Exists(filePath))
            {
                this.logger.Log("Test file not found", LogSeverity.Error);
                return [];
            }

            return File.ReadAllLines(filePath);
        }

        private static bool IsValidTestCase(TestCase testCase)
        {
            return !string.IsNullOrWhiteSpace(testCase.Input) &&
                   (!string.IsNullOrWhiteSpace(testCase.AnswerA) || !string.IsNullOrWhiteSpace(testCase.AnswerB));
        }

        private int[] LoadAvailableTests()
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath))
            {
                this.logger.Log("Tests folder not found", LogSeverity.Error);
                return [];
            }

            string testsPath = Path.Combine(basePath, "Tests");

            Directory.CreateDirectory(testsPath);

            return new DirectoryInfo(Path.Combine(testsPath))
                .GetFiles("*.txt")
                .Select(file => int.Parse(Path.GetFileNameWithoutExtension(file.Name).Replace("test-", "")))
                .OrderByDescending(x => x)
                .ToArray();
        }
    }
}
