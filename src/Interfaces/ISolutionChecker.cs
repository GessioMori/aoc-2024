namespace aoc_2024.Interfaces
{
    public struct SolutionResult
    {
        public int DayNumber;
        public Part Part;
        public string Result;
    }

    public interface ISolutionChecker
    {
        void SaveDayResult(int dayNumber, Part part, string result);
        List<SolutionResult> GetSolutionResults();
    }
}
