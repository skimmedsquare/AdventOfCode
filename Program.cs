using Aoc2021.DayOne;
using Aoc2021.DayTwo;

namespace Aoc2021
{
    class Program
    {
        static readonly Dictionary<int, ISolver> Solvers = new Dictionary<int, ISolver>() {
            { 1, new DepthSolver() },
            { 2, new DirectionSolver() }
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

            if (part == 1) {
                solver.SolvePartOne();
            } else if (part == 2) {
                solver.SolvePartTwo();
            } else {
                Console.WriteLine("Invalid part provided!");
                return -1;
            }

            return 0;
        }
    }

    interface ISolver {
        void SolvePartOne();
        void SolvePartTwo();
    }
}