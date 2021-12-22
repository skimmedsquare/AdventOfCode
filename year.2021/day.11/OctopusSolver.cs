using System.Text;

namespace AdventOfCode.Year2021.Day11
{
    public class OctopusSolver : ISolver
    {
        private static readonly List<PointOffset> Offsets = new()
        {
            new(-1, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, -1),
            new(0, 1),
            new(1, -1),
            new(1, 0),
            new(1, 1)
        };

        private const int Iterations = 195;

        public void SolvePartOne(List<string> data)
        {
            Octopus[,] octopi = Parse(data);
            var counter = new Counter();

            for (var i = 0; i < octopi.GetLength(0); i++)
            {
                for (var j = 0; j < octopi.GetLength(1); j++)
                {
                    octopi[i, j].Flashed += counter.OnFlash;
                    Link(octopi, i, j);
                }
            }

            PrintOctopi(octopi);
            for (var i = 0; i < Iterations; i++)
            {
                foreach (Octopus octopus in octopi)
                {
                    octopus.Increment();
                }

                foreach (Octopus octopus in octopi)
                {
                    octopus.Reset();
                }

                PrintOctopi(octopi);
                Console.WriteLine($"Total Step Flashes: {counter.StepCount}");
                counter.ResetStepCount();
            }

            Console.WriteLine(counter.Count);
        }

        private static void Link(Octopus[,] octopi, int x, int y)
        {
            foreach (PointOffset offset in Offsets)
            {
                int newX = x + offset.X;
                int newY = y + offset.Y;

                if (newX >= 0 && newX < octopi.GetLength(0) && newY >= 0 && newY < octopi.GetLength(1))
                {
                    octopi[x, y].Flashed += octopi[newX, newY].OnFlash;
                }
            }
        }

        private static void PrintOctopi(Octopus[,] octopi)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < octopi.GetLength(0); i++)
            {
                for (int j = 0; j < octopi.GetLength(1); j++)
                {
                    builder.Append(octopi[i, j].Energy).Append(' ');
                }

                builder.Append('\n');
            }

            Console.WriteLine(builder);
        }

        public void SolvePartTwo(List<string> data)
        {
            Octopus[,] octopi = Parse(data);
            var counter = new Counter();

            for (var i = 0; i < octopi.GetLength(0); i++)
            {
                for (var j = 0; j < octopi.GetLength(1); j++)
                {
                    octopi[i, j].Flashed += counter.OnFlash;
                    Link(octopi, i, j);
                }
            }

            PrintOctopi(octopi);
            var done = false;
            var steps = 0;
            while (!done)
            {
                foreach (Octopus octopus in octopi)
                {
                    octopus.Increment();
                }

                foreach (Octopus octopus in octopi)
                {
                    octopus.Reset();
                }

                PrintOctopi(octopi);
                steps++;
                Console.WriteLine($"Total Step Flashes: {counter.StepCount}");
                if (counter.StepCount == octopi.Length)
                {
                    done = true;
                }

                counter.ResetStepCount();
            }

            Console.WriteLine(counter.Count);
            Console.WriteLine($"Steps needed: {steps}");
        }

        private static Octopus[,] Parse(List<string> data)
        {
            var result = new Octopus[data.Count, data.Count];

            var i = 0;
            foreach (string line in data)
            {
                var j = 0;
                foreach (char c in line)
                {
                    result[i, j] = new Octopus(c - '0'); // Rather odd way of converting but... it works?
                    j++;
                }

                i++;
            }

            return result;
        }
    }

    internal class Octopus
    {
        public int Energy { get; private set; }
        public bool HasFlashed { get; private set; }

        public Octopus(int energy)
        {
            Energy = energy;
            HasFlashed = false;
        }

        private void Flash()
        {
            Energy = 0;
            HasFlashed = true;
            OnFlash();
        }

        protected virtual void OnFlash()
        {
            Flashed?.Invoke(this, EventArgs.Empty);
        }

        public void Increment()
        {
            if (!HasFlashed)
            {
                Energy++;

                if (Energy > 9)
                {
                    Flash();
                }
            }
        }

        public void OnFlash(object? source, EventArgs args)
        {
            Increment();
        }

        public void Reset()
        {
            HasFlashed = false;
        }

        public event EventHandler? Flashed;
    }

    internal class Counter
    {
        public int Count { get; private set; }
        public int StepCount { get; private set; }

        public Counter()
        {
            Count = 0;
            StepCount = 0;
        }

        public void OnFlash(object? source, EventArgs args)
        {
            Count++;
            StepCount++;
        }

        public void ResetStepCount()
        {
            StepCount = 0;
        }
    }

    internal record PointOffset(int X, int Y);
}