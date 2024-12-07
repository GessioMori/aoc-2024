using aoc_2024.AocClient;
using aoc_2024.Interfaces;
using Spectre.Console;

namespace aoc_2024.Classes
{
    public class ConsoleController : IController
    {
        private readonly IRunner runner;
        private readonly IAocClient aocClient;
        private readonly ILogger logger;
        private readonly ISolutionManager solutionManager;
        private readonly ILastExecutionManager lastExecutionManager;
        private readonly ITestManager testManager;
        private readonly ISolutionChecker solutionChecker;

        public ConsoleController(IRunner runner,
                                 IAocClient aocClient,
                                 ILogger logger,
                                 ISolutionManager solutionManager,
                                 ILastExecutionManager lastExecutionManager,
                                 ITestManager testManager,
                                 ISolutionChecker solutionChecker)
        {
            this.runner = runner;
            this.aocClient = aocClient;
            this.logger = logger;
            this.solutionManager = solutionManager;
            this.lastExecutionManager = lastExecutionManager;
            this.testManager = testManager;
            this.solutionChecker = solutionChecker;
        }

        public void Run()
        {
            bool shouldExit = false;

            while (!shouldExit)
            {
                Console.Clear();
                PrintHeader();
                Mode mode = SelectMode();

                switch (mode)
                {
                    case Mode.Run:
                        SelectAndRunDay();
                        break;
                    case Mode.Test:
                        SelectAndRunTest();
                        break;
                    case Mode.Init:
                        InitializeDay();
                        break;
                    case Mode.Repeat:
                        RunLastCommand();
                        break;
                    case Mode.Check:
                        CheckSolutions();
                        break;
                    case Mode.Exit:
                        shouldExit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void PrintHeader()
        {
            Rule rule = new("🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟")
            {
                Justification = Justify.Center,
                Border = BoxBorder.None,
            };

            AnsiConsole.Write(rule);

            AnsiConsole.Write(
                new FigletText($"AoC {Consts.year}")
                .Centered()
                .Color(Color.Red));

            AnsiConsole.WriteLine();
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
        }

        private void RunLastCommand()
        {
            if (!this.lastExecutionManager.HasLastExecution())
            {
                this.logger.Log("Invalid last execution.", LogSeverity.Error);
                return;
            }

            ExecutionSettings execution = this.lastExecutionManager.GetLastExecution();

            if (execution.mode == Mode.Run)
            {
                RunDay(execution.day, execution.part);
            }
            else if (execution.mode == Mode.Test)
            {
                TestCase? testCase = this.testManager.Parse(execution.day)
                    .FirstOrDefault(t => t.TestNumber == execution.testNumber);

                if (testCase != null)
                {
                    RunTest(execution.day, execution.part, testCase);
                }
                else
                {
                    this.logger.Log($"Unable to find Test #{execution.testNumber}", LogSeverity.Error);
                }
            }
            else
            {
                this.logger.Log("Invalid last execution.", LogSeverity.Error);
            }
        }

        private Mode SelectMode()
        {
            string[] baseChoices = new List<Mode> { Mode.Run, Mode.Test, Mode.Init, Mode.Check, Mode.Exit }
                .Select(m => m.ToString())
                .ToArray();

            string[] choices = this.lastExecutionManager.HasLastExecution() ?
                [GetLastExecutionString(), .. baseChoices] :
                [.. baseChoices];

            string modeText = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(choices)
                );

            if (!Enum.TryParse(modeText, out Mode mode))
            {
                return Mode.Repeat;
            }
            else
            {
                return mode;
            }
        }

        private static Part SelectPart()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<Part>()
                .UseConverter(m => m.ToString())
                .Title("[bold]Part selection[/]")
                .AddChoices(Enum.GetValues<Part>())
                );
        }

        private void CheckSolutions()
        {
            List<SolutionResult> results = this.solutionChecker.GetSolutionResults();

            TableColumn[] columns = new List<string> { "Day", "Part", "Is Correct", "Time (ms)" }
                .Select(s => new TableColumn(s))
                .ToArray();

            Table table = new Table().Centered();
            table.Border = TableBorder.Rounded;
            table.ShowRowSeparators = true;

            AnsiConsole.Live(table)
                .Start(ctx =>
                {
                    table.AddColumns(columns);

                    foreach (SolutionResult prevResult in results)
                    {
                        table.AddRow(new Markup(prevResult.DayNumber.ToString()),
                                    new Markup(prevResult.Part.ToString()),
                                    new Markup("[dodgerblue3]Running[/]"),
                                    new Markup("0"));

                        ctx.Refresh();
                    }

                    Parallel.For(0, results.Count, i =>
                    {
                        SolutionResult prevResult = results[i];

                        ISolution? solution = this.solutionManager.CreateSolutionInstance(prevResult.DayNumber);
                        string input = this.solutionManager.ReadInputFile(prevResult.DayNumber);
                        ExecutionResult? result = null;

                        if (solution != null && !string.IsNullOrEmpty(input))
                        {
                            result = this.runner.Run(solution, prevResult.DayNumber, prevResult.Part, input);
                        }

                        if (result != null)
                        {
                            if (result.ResultType == ExecutionResultType.Success)
                            {
                                table.Rows.Update(i, 2, prevResult.Result == result.Result ?
                                    new Markup("[green]Yes[/]") :
                                    new Markup("[orange3]No[/]"));
                                table.Rows.Update(i, 3, new Markup(result.ElapsedTimeInMs.ToString()));
                            }
                            else
                            {
                                table.Rows.Update(i, 2, new Markup("[red]Error[/]"));
                                table.Rows.Update(i, 3, new Markup("-"));
                            }
                        }

                        ctx.Refresh();
                    });
                });

            Console.ReadKey();
        }

        private static Part SelectAvailableTestPart(TestCase testCase)
        {
            List<Part> parts = [];

            if (!string.IsNullOrEmpty(testCase.AnswerA))
            {
                parts.Add(Part.A);
            }
            if (!string.IsNullOrEmpty(testCase.AnswerB))
            {
                parts.Add(Part.B);
            }

            return AnsiConsole.Prompt(
                new SelectionPrompt<Part>()
                .UseConverter(m => m.ToString())
                .Title("[bold]Part selection[/]")
                .AddChoices(parts)
                );
        }

        private void SelectAndRunDay()
        {
            int dayToRun = ChoseAvailableSolution();

            if (dayToRun == 0)
            {
                this.logger.Log("There isn't any available solution. Press any key to continue.", LogSeverity.Error);
                Console.ReadKey();
                return;
            }

            Part partToRun = SelectPart();

            Console.Clear();

            PrintHeader();

            RunDay(dayToRun, partToRun);
        }

        private void RunDay(int dayToRun, Part partToRun)
        {
            this.lastExecutionManager.WriteLastChoice(dayToRun, Mode.Run, partToRun);

            ExecutionResult? result = null;

            AnsiConsole.Status()
                .Start($"Running Day #{dayToRun} - Part {partToRun}...",
                ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    ISolution? solution = this.solutionManager.CreateSolutionInstance(dayToRun);
                    string input = this.solutionManager.ReadInputFile(dayToRun);

                    if (solution != null && !string.IsNullOrEmpty(input))
                    {
                        result = this.runner.Run(solution, dayToRun, partToRun, input);
                    }
                });

            if (result != null)
            {
                if (result.ResultType == ExecutionResultType.Success)
                {
                    this.solutionChecker.SaveDayResult(dayToRun, partToRun, result.Result);
                    AnsiConsole.WriteLine();

                    this.logger.Log($"Day #{dayToRun} - Part {partToRun} successfully run!", LogSeverity.Runner);
                    this.logger.Log($"Result: {result.Result}", LogSeverity.Runner);
                    this.logger.Log($"Elapsed time: {result.ElapsedTimeInMs} ms", LogSeverity.Runner);
                }
                else
                {
                    AnsiConsole.WriteLine();
                    this.logger.Log($"Error runnning Day #{dayToRun} - Part {partToRun}:", LogSeverity.Error);
                    this.logger.Log(result.Result, LogSeverity.Other);
                }
            }
            else
            {
                this.logger.Log($"There was a problem running Day #{dayToRun} - Part {partToRun}",
                    LogSeverity.Error);
            }

            AnsiConsole.WriteLine();
            this.logger.Log("Press any key to continue", LogSeverity.Other);
            Console.ReadKey();
        }

        private void SelectAndRunTest()
        {
            int dayToRun = ChoseAvailableTestDay();
            List<TestCase> testCases = this.testManager.Parse(dayToRun);

            if (testCases.Count == 0)
            {
                this.logger.Log($"Day #{dayToRun} has no valid tests. Press any key to continue.", LogSeverity.Error);
                Console.ReadKey();
                return;
            }

            int[] availableTestCases = testCases.Select(t => t.TestNumber).Order().ToArray();

            int testCaseToRun = ChoseAvailableTestCase(availableTestCases);

            TestCase testCase = testCases.First(t => t.TestNumber == testCaseToRun);

            Part partToRun = SelectAvailableTestPart(testCase);

            Console.Clear();

            PrintHeader();

            this.lastExecutionManager.WriteLastChoice(dayToRun, Mode.Test, partToRun, testCaseToRun);

            RunTest(dayToRun, partToRun, testCase);
        }

        private void RunTest(int dayToRun, Part partToRun, TestCase testCaseToRun)
        {
            AnsiConsole.WriteLine();

            ExecutionResult? result = null;
            string? expectedResult = partToRun == Part.A ? testCaseToRun.AnswerA : testCaseToRun.AnswerB;

            AnsiConsole.Status()
                .Start($"Running Test #{testCaseToRun.TestNumber} for Day #{dayToRun} - Part {partToRun}...",
                ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    ISolution? solution = this.solutionManager.CreateSolutionInstance(dayToRun);
                    string input = testCaseToRun.Input;

                    if (solution != null && !string.IsNullOrEmpty(input))
                    {
                        result = this.runner.Run(solution, dayToRun, partToRun, input);
                    }
                });

            if (result != null && !string.IsNullOrEmpty(expectedResult))
            {
                if (result.ResultType == ExecutionResultType.Success)
                {
                    this.logger.Log($"Test #{testCaseToRun.TestNumber} for Day #{dayToRun} - Part {partToRun} successfully run!", LogSeverity.Runner);
                    this.logger.Log($"Result: {result.Result}", LogSeverity.Runner);
                    this.logger.Log($"Expected result: {expectedResult}", LogSeverity.Runner);
                    this.logger.Log($"Elapsed time: {result.ElapsedTimeInMs} ms", LogSeverity.Runner);

                    if (result.Result == expectedResult)
                    {
                        this.logger.Log($"Your test [green]passed[/]!", LogSeverity.Runner);
                    }
                    else
                    {
                        this.logger.Log($"Your test [red]failed[/]!", LogSeverity.Runner);
                    }
                }
                else
                {
                    this.logger.Log($"Error runnning Day #{dayToRun} - Part {partToRun}:", LogSeverity.Error);
                    this.logger.Log(result.Result, LogSeverity.Other);
                }

            }
            else
            {
                this.logger.Log($"There was a problem running Day #{dayToRun} - Part {partToRun}",
                    LogSeverity.Error);
            }

            AnsiConsole.WriteLine();
            this.logger.Log("Press any key to continue", LogSeverity.Other);
            Console.ReadKey();
        }

        private void InitializeDay()
        {
            int dayToInitialize = AnsiConsole.Prompt(
                new TextPrompt<int>("Day: "));

            Console.Clear();
            PrintHeader();

            AnsiConsole.Status()
                .Start($"Initializing day #{dayToInitialize}...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star2);

                    if (this.solutionManager.IsDayAlreadyInitialized(dayToInitialize))
                    {
                        this.logger.Log($"Day #{dayToInitialize} has already a solution. Only the input will be fetched.", LogSeverity.Log);
                    }

                    this.logger.Log("Getting puzzle input from AoC...", LogSeverity.Log);
                    ClientResponse input = this.aocClient.GetPuzzleInput(dayToInitialize).Result;
                    if (input.ResponseType == ClientResponseType.Success)
                    {
                        this.logger.Log("Puzzle input fetched with success!", LogSeverity.Log);
                        ctx.Spinner(Spinner.Known.Christmas);
                        this.logger.Log("Creating files...", LogSeverity.Log);
                        this.solutionManager.CreateInitialFiles(dayToInitialize, input.Content);
                        this.logger.Log($"Files created for Day #{dayToInitialize}", LogSeverity.Log);
                    }
                    else
                    {
                        this.logger.Log($"Error getting puzzle input:\r\n: {input.Content}", LogSeverity.Error);
                    }

                    this.logger.Log("Press any key to return to main menu", LogSeverity.Other);
                });

            Console.ReadKey();
        }

        private int ChoseAvailableSolution()
        {
            int[] availableSolutions = this.solutionManager.GetAvailableSolutions();

            if (availableSolutions.Length == 0)
            {
                return 0;
            }

            int choice = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("[bold]Select day[/]")
                .PageSize(5)
                .AddChoices(availableSolutions)
                );

            return choice;
        }

        private static int ChoseAvailableTestCase(int[] availableTestCases)
        {
            int choice = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("[bold]Select test case[/]")
                .PageSize(5)
                .AddChoices(availableTestCases)
                );

            return choice;
        }

        private int ChoseAvailableTestDay()
        {
            int[] availableSolutions = this.solutionManager.GetAvailableSolutions();
            int[] availableTests = this.testManager.GetAvailableTests();

            int[] intersection = availableSolutions.Intersect(availableTests).ToArray();

            if (intersection.Length == 0)
            {
                return 0;
            }

            int choice = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("[bold]Select day[/]")
                .PageSize(5)
                .AddChoices(intersection)
                );

            return choice;
        }

        private string GetLastExecutionString()
        {
            if (!this.lastExecutionManager.HasLastExecution())
            {
                return string.Empty;
            }

            ExecutionSettings execution = this.lastExecutionManager.GetLastExecution();

            if (execution.mode == Mode.Test)
            {
                return $"Run Test #{execution.testNumber}" +
                    $" for Day #{execution.day} - Part {execution.part}";
            }

            return $"Run Day #{execution.day} - Part {execution.part}";
        }
    }
}
