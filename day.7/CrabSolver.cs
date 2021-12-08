namespace Aoc2021.Day7
{
    public class CrabSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            List<int> crabs = Parse(data);
            crabs.Sort();
            Console.WriteLine((from crab in crabs select crab).Sum() / crabs.Count);
            int median = CalculateMedian(crabs);

            int sumOfDistances = (from crab in crabs
                                  select Math.Abs(median - crab)).Sum();

            Console.WriteLine(sumOfDistances);
        }

        private static List<int> Parse(in List<string> data)
        {
            return data[0].Split(',').Select(n => int.Parse(n)).ToList();
        }

        private static int CalculateMedian(in List<int> values)
        {
            if (values.Count % 2 == 0)
            {
                // Truncation means we get the value we want, e.g. ([0,1,2] => idx 1)
                int middle = values.Count / 2;
                return (values[middle] + values[middle - 1]) / 2;
            }

            return values[values.Count / 2];
        }

        public void SolvePartTwo(in List<string> data)
        {
            List<int> crabs = Parse(data);

            // memoize
            int max = crabs.Max();
            var distList = new List<int>(max + 1);
            for (var i = 0; i < max + 1; i++)
            {
                distList.Add(i * (i + 1) / 2);
            }

            var initialPositions = new Dictionary<int, int>(); // position : count
            foreach (int crab in crabs)
            {
                if (initialPositions.TryGetValue(crab, out _))
                {
                    initialPositions[crab]++;
                }
                else
                {
                    initialPositions[crab] = 1;
                }
            }

            var fuelCosts = new List<int>(max + 1);
            for (var i = 0; i < max + 1; i++)
            {
                var sum = 0;
                foreach (KeyValuePair<int, int> entry in initialPositions)
                {
                    var dist = Math.Abs(entry.Key - i);
                    sum += distList[dist] * entry.Value;
                }

                fuelCosts.Add(sum);
            }

            Console.WriteLine(fuelCosts.Min());
        }
    }
}