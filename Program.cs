using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;

namespace Aoc2021
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Argument<string>(
                    "selection",
                    getDefaultValue: () => "1.1",
                    description: "The day and part to run, formatted as <day.part> (e.g. 1.1)"),
                new Option<bool>(
                    new[] { "--use-real-input", "-r" },
                    getDefaultValue: () => false,
                    description: "Whether to use the real (read: large) input, or the example input"),
            };

            rootCommand.Description = "Advent of Code Runner";
            rootCommand.Handler = CommandHandler.Create<string, bool>(Execute);
            rootCommand.Invoke(args);
        }

        private static void GetSolvers(out Dictionary<int, ISolver> solvers)
        {
            solvers = new Dictionary<int, ISolver>();
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(ISolver).IsAssignableFrom(x) && !x.IsInterface).ToList();
            foreach (Type type in types)
            {
                ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);
                char lastChar = type.Namespace![^1];
                int day = int.Parse(lastChar.ToString());
                solvers[day] = (ISolver)ctor!.Invoke(null);
            }
        }

        private static List<string> ReadFile(int day, bool useRealInput)
        {
            var path = $"day.{day}/{(useRealInput ? "input" : "example")}.txt";
            Console.WriteLine($"Using file at: {path}");
            return File.ReadLines(path).ToList();
        }

        private static void Execute(string selection, bool useRealInput)
        {
            string[] splitChoice = selection.Split('.');

            int day = int.Parse(splitChoice[0]);
            int part = int.Parse(splitChoice[1]);

            Console.WriteLine($"Executing Day {day}, Part {part}");

            var solvers = new Dictionary<int, ISolver>();
            GetSolvers(out solvers);

            if (!solvers.TryGetValue(day, out ISolver? solver) || !(part == 1 || part == 2))
            {
                Console.WriteLine("Invalid day provided!");
                return;
            }

            var watch = new System.Diagnostics.Stopwatch();
            Solve method = part switch {
                1 => solver.SolvePartOne,
                2 => solver.SolvePartTwo,
                _ => throw new InvalidOperationException()
            };

            watch.Start();
            method.Invoke(ReadFile(day, useRealInput));
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("===============================================");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("===============================================");
        }
    }

    public delegate void Solve(in List<string> data);

    public interface ISolver
    {
        void SolvePartOne(in List<string> data);
        void SolvePartTwo(in List<string> data);
    }
}