namespace aoc_2024.SolutionUtils
{
    public class MatrixAxis
    {
        private readonly string axisString;
        private readonly (int, int)[] axisCoordinates;

        public string AxisString { get => axisString; }

        public MatrixAxis(string axisString, (int, int)[] axisCoordinates)
        {
            this.axisString = axisString;
            this.axisCoordinates = axisCoordinates;
        }

        public (int, int) GetCoordinatesByAxisPosition(int position)
        {
            if (position < 0 || position > this.axisString.Length - 1)
            {
                return (-1, -1);
            }

            return this.axisCoordinates[position];
        }
    }
}
