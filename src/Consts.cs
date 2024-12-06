namespace aoc_2024
{
    public static class Consts
    {
        public const int year = 2024;
        public const string baseUri = "https://adventofcode.com";
    }

    public enum Mode
    {
        Repeat,
        Run,
        Test,
        Init,
        Check,
        Exit
    }

    public enum Part
    {
        A,
        B
    }

    public enum ClientResponseType
    {
        Success,
        Failure,
    }

    public enum LogSeverity
    {
        Log,
        Error,
        Runner,
        Other
    }
}
