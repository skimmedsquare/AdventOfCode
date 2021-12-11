namespace AdventOfCode.Year2021.Day6
{
    public class FishSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            const int days = 80;
            List<int> fishes = Parse(data);
            for (var i = 0; i < days; i++)
            {
                var numToAdd = 0;
                for (var j = 0; j < fishes.Count; j++)
                {
                    if (fishes[j] == 0)
                    {
                        numToAdd++;
                        fishes[j] = 6;
                    }
                    else
                    {
                        fishes[j]--;
                    }
                }

                for (var f = 0; f < numToAdd; f++)
                {
                    fishes.Add(8);
                }

                // Console.WriteLine($"After {i + 1} day(s): {string.Join(',', fishes)}");
            }

            Console.WriteLine(fishes.Count);
        }

        private static List<int> Parse(in List<string> data)
        {
            return (from num in data[0].Split(',')
                    select int.Parse(num)).ToList();
        }

        public void SolvePartTwo(in List<string> data)
        {
            const int days = 256;
            List<int> fishes = Parse(data);

            var fishLog = new Dictionary<int, long>()
            {
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 },
                { 6, 0 },
                { 7, 0 },
                { 8, 0 },
            };

            foreach (int fish in fishes)
            {
                fishLog[fish]++;
            }

            for (var _ = 0; _ < days; _++)
            {
                var numBirthing = 0L;
                for (var j = 0; j < 9; j++)
                {
                    if (j == 0)
                    {
                        numBirthing = fishLog[j];
                    }
                    else
                    {
                        fishLog[j - 1] = fishLog[j];
                        if (j == 7)
                        {
                            fishLog[j - 1] += numBirthing;
                        }
                    }
                }

                fishLog[8] = numBirthing;
            }

            foreach (KeyValuePair<int, long> entry in fishLog)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }

            Console.WriteLine((from e in fishLog select e.Value).Sum());
        }
    }
}