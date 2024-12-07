using aoc_2024.Interfaces;
using System.Text;

namespace aoc_2024.Classes
{
    public class SolutionChecker : ISolutionChecker
    {
        private readonly ILogger logger;
        private readonly List<SolutionResult> results;

        public SolutionChecker(ILogger logger)
        {
            this.logger = logger;
            this.results = [];
            LoadDayResults();
        }

        public List<SolutionResult> GetSolutionResults()
        {
            this.results.Sort((x, y) =>
            {
                int dayComparison = x.DayNumber.CompareTo(y.DayNumber);

                if (dayComparison != 0)
                {
                    return dayComparison;
                }

                return x.Part.CompareTo(y.Part);
            });

            return this.results;
        }

        public void SaveDayResult(int dayNumber, Part part, string result)
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath))
            {
                this.logger.Log("Unable to find base folder", LogSeverity.Error);
                return;
            }

            string folderPath = Path.Combine(basePath, "ProgramUtils");
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, "results.txt");

            try
            {
                string content = AddOrReplaceDayAndReturnNewString(dayNumber, part, result);
                File.WriteAllText(filePath, content);

                this.logger.Log($"Result saved for Day #{dayNumber} - Part {part}", LogSeverity.Log);
            }
            catch (Exception ex)
            {
                this.logger.Log($"Failed to write result: {ex.Message}", LogSeverity.Error);
            }
        }

        private string AddOrReplaceDayAndReturnNewString(int dayNumber, Part part, string result)
        {
            int index = this.results.FindIndex(r => r.DayNumber == dayNumber && r.Part == part);

            if (index != -1)
            {
                SolutionResult updatedResult = this.results[index];
                updatedResult.Result = result;
                this.results[index] = updatedResult;
            }
            else
            {
                this.results.Add(new SolutionResult
                {
                    DayNumber = dayNumber,
                    Part = part,
                    Result = result
                });
            }

            return GetResultsString();
        }

        private string GetResultsString()
        {
            StringBuilder content = new();

            foreach (SolutionResult solutionResult in this.results)
            {
                content.AppendLine($"{solutionResult.DayNumber}|{solutionResult.Part}|{solutionResult.Result}");
            }

            return content.ToString();
        }

        private void LoadDayResults()
        {
            this.results.Clear();

            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath))
            {
                this.logger.Log("Tests folder not found", LogSeverity.Error);
                return;
            }

            string filePath = Path.Combine(basePath, "ProgramUtils", "results.txt");

            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                string[] fileContent = File.ReadAllLines(filePath);

                if (fileContent.Length == 0)
                {
                    this.logger.Log("Empty results file.", LogSeverity.Error);
                    return;
                }

                foreach (string line in fileContent)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string[] parts = line.Split('|');
                        this.results.Add(new SolutionResult()
                        {
                            DayNumber = int.Parse(parts[0]),
                            Part = Enum.Parse<Part>(parts[1]),
                            Result = parts[2]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Log($"Failed to load results: {ex.Message}", LogSeverity.Error);
                this.results.Clear();
            }
        }
    }
}
