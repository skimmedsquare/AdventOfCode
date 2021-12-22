namespace AdventOfCode.Year2021.Day1
{
    public class DepthSolver : ISolver
    {
        public void SolvePartOne(List<string> data)
        {
            List<int> depths = Parse(data);
            Console.WriteLine($"Solution: {NumIncreasing(depths)}");
        }

        public void SolvePartTwo(List<string> data)
        {
            List<int> depths = Parse(data);

            List<int> windows = new(depths.Count - 3);
            var currWindow = 0;
            for (var i = 0; i < depths.Count - 2; i++)
            {
                if (i == 0)
                {
                    currWindow = depths[i] + depths[i + 1] + depths[i + 2];
                    windows.Add(currWindow);
                }
                else
                {
                    currWindow -= depths[i - 1];
                    currWindow += depths[i + 2];
                    windows.Add(currWindow);
                }
            }

            Console.WriteLine($"Solution: {NumIncreasing(windows)}");
        }

        private static List<int> Parse(List<string> data)
        {
            return (from line in data select int.Parse(line)).ToList();
        }

        private static int NumIncreasing(List<int> vals)
        {
            var count = 0;
            for (var i = 0; i < vals.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                if (vals[i] > vals[i - 1])
                {
                    count++;
                }
            }

            return count;
        }
    }
}