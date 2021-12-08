namespace Aoc2021.Day7 {
    class CrabSolver : ISolver
    {
        public void SolvePartOne()
        {
            var crabs = ReadFile();
            crabs.Sort();
            Console.WriteLine((from crab in crabs select crab).Sum() / crabs.Count());
            var median = CalculateMedian(crabs);

            var sumOfDistances = (from crab in crabs
                                  select Math.Abs(median - crab)).Sum();

            Console.WriteLine(sumOfDistances);
        }

        List<int> ReadFile() {
            return File.ReadAllLines("day.seven/input.txt")[0].Split(',').Select(n => int.Parse(n)).ToList();
        }

        int CalculateMedian(in List<int> values) {
            if (values.Count() % 2 == 0) {
                var middle = values.Count() / 2;
                return (values[middle] + values[middle-1]) / 2;
            } else {
                // Truncation means we get the value we want, e.g. ([0,1,2] => idx 1)
                return values[values.Count() / 2];
            }
        }

        public void SolvePartTwo()
        {
            var crabs = ReadFile();

            // memoize
            var max = crabs.Max();
            var distList = new List<int>(max + 1);
            for (var i = 0; i < max + 1; i++) {
                distList.Add((i * (i+1)) / 2);
            }

            var initialPositions = new Dictionary<int, int>(); // position : count
            foreach(var crab in crabs) {
                int val;
                if (initialPositions.TryGetValue(crab, out val)) {
                    initialPositions[crab]++;
                } else {
                    initialPositions[crab] = 1;
                }
            }

            var fuelCosts = new List<int>(max + 1);
            for (var i = 0; i < max + 1; i++) {
                var sum = 0;
                foreach(var entry in initialPositions) {
                    var dist = Math.Abs(entry.Key - i);
                    sum += distList[dist] * entry.Value;
                }
                fuelCosts.Add(sum);
            }
            Console.WriteLine(fuelCosts.Min());
        }
    }
}