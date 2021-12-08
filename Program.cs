using System.CommandLine;
using System.CommandLine.Invocation;

namespace Aoc2021
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Argument<string>(
                    "selection",
                    getDefaultValue: () => "1.1",
                    description: "The day and part to run, formatted as <day.part> (e.g. 1.1)"),
                new Option<bool>(
                    new string[] {"--use-real-input", "-r"},
                    getDefaultValue: () => false,
                    description: "Whether to use the real (read: large) input, or the example input"),
            };

            rootCommand.Description = "Advent of Code Runner";
            rootCommand.Handler = CommandHandler.Create<string, bool>(execute);
            rootCommand.Invoke(args);
        }

        static void GetSolvers(out Dictionary<int, ISolver> solvers) {
            solvers = new Dictionary<int, ISolver>();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(ISolver).IsAssignableFrom(x) && !x.IsInterface).ToList();
            foreach (var type in types) {
                var ctor = type.GetConstructor(System.Type.EmptyTypes);
                var lastChar = type.Namespace![type.Namespace.Count()-1];
                var day = int.Parse(lastChar.ToString());
                solvers[day] = (ISolver)ctor!.Invoke(null);
            }
        }

        static List<string> ReadFile(int day, bool useRealInput) {
            var path = $"day.{day}/{(useRealInput ? "input" : "example")}.txt";
            Console.WriteLine($"Using file at: {path}");
            return File.ReadLines(path).ToList();
        }

        static void execute(string selection, bool useRealInput) {
            var splitChoice = selection.Split('.');
            
            var day = int.Parse(splitChoice[0]);
            var part = int.Parse(splitChoice[1]);

            Console.WriteLine($"Executing Day {day}, Part {part}");

            var solvers = new Dictionary<int, ISolver>();
            GetSolvers(out solvers);

            ISolver? solver;
            if (!solvers.TryGetValue(day, out solver) || !(part == 1 || part == 2)) {
                Console.WriteLine("Invalid day provided!");
                return;
            }

            var watch = new System.Diagnostics.Stopwatch();
            var lines = ReadFile(day, useRealInput);
            Solve method = part switch {
                1 => solver.SolvePartOne,
                2 => solver.SolvePartTwo,
                _ => throw new InvalidOperationException()
            };

            watch.Start();
            method.Invoke(lines);
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("===============================================");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("===============================================");
        }
    }

    public delegate void Solve(in List<string> data);

    interface ISolver {
        void SolvePartOne(in List<string> data);
        void SolvePartTwo(in List<string> data);
    }
}