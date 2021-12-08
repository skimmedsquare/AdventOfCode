using Aoc2021.Day1;
using Aoc2021.Day2;
using Aoc2021.Day3;
using Aoc2021.Day4;
using Aoc2021.Day5;
using Aoc2021.Day6;
using Aoc2021.Day7;

namespace Aoc2021
{
    class Program
    {
        static readonly Dictionary<int, ISolver> Solvers = new Dictionary<int, ISolver>() {
            { 1, new DepthSolver() },
            { 2, new DirectionSolver() },
            { 3, new BinarySolver() },
            { 4, new BingoSolver() },
            { 5, new VentSolver() },
            { 6, new FishSolver() },
            { 7, new CrabSolver() }
        };

        static int Main(string[] args)
        {
            if (args.Count() != 1) {
                return -1;
            }

            var choice = args[0];
            var splitChoice = choice.Split('.');
            
            var day = int.Parse(splitChoice[0]);
            var part = int.Parse(splitChoice[1]);

            Console.WriteLine($"Executing Day {day}, Part {part}");

            ISolver? solver;
            if (!Solvers.TryGetValue(day, out solver)) {
                Console.WriteLine("Invalid day provided!");
                return -1;
            }

            var watch = new System.Diagnostics.Stopwatch();
            if (part == 1) {
                watch.Start();
                solver.SolvePartOne();
                watch.Stop();
            } else if (part == 2) {
                watch.Start();
                solver.SolvePartTwo();
                watch.Stop();
            } else {
                Console.WriteLine("Invalid part provided!");
                return -1;
            }
            Console.WriteLine();
            Console.WriteLine("===============================================");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("===============================================");
            return 0;
        }
    }

    interface ISolver {
        void SolvePartOne();
        void SolvePartTwo();
    }
}