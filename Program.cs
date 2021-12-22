using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;

namespace AdventOfCode
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Argument<string>(
                    "selection",
                    getDefaultValue: () => "2021.1.1",
                    description: "The year, day, and part to run, formatted as <year.day.part> (e.g. 2021.1.1)"),
                new Option<bool>(
                    new[] { "--use-real-input", "-r" },
                    getDefaultValue: () => false,
                    description: "Whether to use the real (read: large) input, or the example input"),
            };

            rootCommand.Description = "Advent of Code Runner";
            rootCommand.Handler = CommandHandler.Create<string, bool>(Execute);
            rootCommand.Invoke(args);
        }

        private static void GetSolvers(out Dictionary<SolverKey, ISolver> solvers)
        {
            solvers = new Dictionary<SolverKey, ISolver>();
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(ISolver).IsAssignableFrom(x) && !x.IsInterface).ToList();
            foreach (Type type in types)
            {
                ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);
                string dayString = type.Namespace!.Split('.')[^1].TrimStart("Day".ToCharArray());
                string yearString = type.Namespace!.Split('.')[^2].TrimStart("Year".ToCharArray());
                var day = int.Parse(dayString);
                var year = int.Parse(yearString);
                solvers[new SolverKey(year, day)] = (ISolver)ctor!.Invoke(null);
            }
        }

        private static List<string> ReadFile(int year, int day, bool useRealInput)
        {
            var path = $"year.{year}/day.{day}/{(useRealInput ? "input" : "example")}.txt";
            Console.WriteLine($"Using file at: {System.IO.Directory.GetCurrentDirectory()}/{path}");
            return File.ReadLines(path).ToList();
        }

        private static void Execute(string selection, bool useRealInput)
        {
            string[] splitChoice = selection.Split('.');

            var year = int.Parse(splitChoice[0]);
            var day = int.Parse(splitChoice[1]);
            var part = int.Parse(splitChoice[2]);

            Console.WriteLine($"Executing Year {year}, Day {day}, Part {part}");

            var solvers = new Dictionary<SolverKey, ISolver>();
            GetSolvers(out solvers);

            if (!solvers.TryGetValue(new SolverKey(year, day), out ISolver? solver) || !(part == 1 || part == 2))
            {
                Console.WriteLine("Invalid year, day, or part provided!");
                return;
            }

            var watch = new System.Diagnostics.Stopwatch();
            Solve method = part switch {
                1 => solver.SolvePartOne,
                2 => solver.SolvePartTwo,
                _ => throw new InvalidOperationException()
            };

            watch.Start();
            method.Invoke(ReadFile(year, day, useRealInput));
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("===============================================");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("===============================================");
        }
    }

    public delegate void Solve(List<string> data);

    public struct SolverKey
    {
        public SolverKey(int year, int day)
        {
            Year = year;
            Day = day;
        }

        public readonly int Year { get; }
        public readonly int Day { get; }
    }

    public interface ISolver
    {
        void SolvePartOne(List<string> data);
        void SolvePartTwo(List<string> data);
    }
}