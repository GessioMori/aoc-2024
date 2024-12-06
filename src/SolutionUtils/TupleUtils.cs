using System.Numerics;

namespace aoc_2024.SolutionUtils
{
    public static class TupleExtensions
    {
        public static (T, T) Add<T>(this (T, T) tuple1, (T, T) tuple2) where T : INumber<T>
        {
            return (tuple1.Item1 + tuple2.Item1, tuple1.Item2 + tuple2.Item2);
        }
    }
}
