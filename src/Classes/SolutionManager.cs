using aoc_2024.Interfaces;

namespace aoc_2024.Classes
{
    public class SolutionManager : ISolutionManager
    {
        private readonly ILogger logger;
        private int[] availableSolutions;

        public SolutionManager(ILogger logger)
        {
            this.logger = logger;
            this.availableSolutions = LoadAvailableSolutions();
        }

        public void CreateInitialFiles(int dayToInitialize, string inputContent)
        {
            if (!this.IsDayAlreadyInitialized(dayToInitialize))
            {
                CreateSolutionFile(dayToInitialize);
                CreateTestsFile(dayToInitialize);
            }

            CreateInputFile(dayToInitialize, inputContent);
            this.availableSolutions = LoadAvailableSolutions();
        }

        public int[] GetAvailableSolutions()
        {
            return this.availableSolutions;
        }

        public bool IsDayAlreadyInitialized(int dayToInitialize)
        {
            return this.availableSolutions.Contains(dayToInitialize);
        }

        public ISolution? CreateSolutionInstance(int dayToRun)
        {
            if (!this.availableSolutions.Contains(dayToRun))
            {
                this.logger.Log("Solution not found", LogSeverity.Error);
                return null;
            }

            string typeName = $"aoc_2024.Solutions.Solution{dayToRun.ToString().PadLeft(2, '0')}";
            Type? solutionType = Type.GetType(typeName);
            if (solutionType != null)
            {
                object? solution = Activator.CreateInstance(solutionType);
                if (solution != null)
                {
                    return (ISolution)solution;
                }
            }

            this.logger.Log("Unable to create solution", LogSeverity.Error);
            return null;
        }

        private static int[] LoadAvailableSolutions()
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath))
            {
                return [];
            }

            string solutionsPath = Path.Combine(basePath, "Solutions");

            Directory.CreateDirectory(solutionsPath);

            return new DirectoryInfo(solutionsPath)
                .GetFiles("*.cs")
                .Select(file => int.Parse(Path.GetFileNameWithoutExtension(file.Name).Replace("Solution", "")))
                .OrderByDescending(x => x)
                .ToArray();
        }

        private static void CreateInputFile(int dayNumber, string inputText)
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath)) return;

            string folderPath = Path.Combine(basePath, "Inputs");
            string filePath = Path.Combine(folderPath, $"input-{dayNumber.ToString().PadLeft(2, '0')}.txt");

            Directory.CreateDirectory(folderPath);

            File.WriteAllText(filePath, inputText);
        }

        public string ReadInputFile(int dayNumber)
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath))
            {
                this.logger.Log("Input folder not found", LogSeverity.Error);
                return string.Empty;
            }

            string folderPath = Path.Combine(basePath, "Inputs");
            string filePath = Path.Combine(folderPath, $"input-{dayNumber.ToString().PadLeft(2, '0')}.txt");

            if (!File.Exists(filePath))
            {
                this.logger.Log("Input file not found", LogSeverity.Error);
                return string.Empty;
            }

            return File.ReadAllText(filePath);
        }

        private static void CreateSolutionFile(int dayToInitialize)
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath)) return;

            string formatedDayNumber = $"Solution{dayToInitialize.ToString().PadLeft(2, '0')}";
            string folderPath = Path.Combine(basePath, "Solutions");
            string filePath = Path.Combine(folderPath, formatedDayNumber + ".cs");

            Directory.CreateDirectory(folderPath);

            string templatePath = Path.Combine("Templates", "solution-template.txt");
            string templateContent = File.ReadAllText(templatePath);

            string formatedTemplate = string.Format(templateContent, formatedDayNumber);

            File.WriteAllText(filePath, formatedTemplate);
        }

        private static void CreateTestsFile(int dayToInitialize)
        {
            string? basePath = FileUtils.FindProjectFolder();

            if (string.IsNullOrEmpty(basePath)) return;

            string formatedDayNumber = $"test-{dayToInitialize.ToString().PadLeft(2, '0')}.txt";
            string folderPath = Path.Combine(basePath, "Tests");
            string filePath = Path.Combine(folderPath, formatedDayNumber);

            Directory.CreateDirectory(folderPath);

            string templatePath = Path.Combine("Templates", "test-template.txt");
            string templateContent = File.ReadAllText(templatePath);

            File.WriteAllText(filePath, templateContent);
        }
    }
}
