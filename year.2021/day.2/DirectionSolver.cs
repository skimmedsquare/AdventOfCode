namespace AdventOfCode.Year2021.Day2
{
    public class DirectionSolver : ISolver
    {
        public void SolvePartOne(List<string> data)
        {
            var depth = 0;
            var pos = 0;

            foreach (Instruction instr in Parse(data))
            {
                if (instr.Direction == "forward")
                {
                    pos += instr.Value;
                }
                else
                {
                    if (instr.Direction == "up")
                    {
                        depth += instr.Value;
                    }
                    else
                    {
                        depth -= instr.Value;
                    }
                }
            }

            Console.WriteLine($"Solution: {depth * pos}");
        }

        public void SolvePartTwo(List<string> data)
        {
            var depth = 0;
            var pos = 0;
            var aim = 0;

            foreach (Instruction instr in Parse(data))
            {
                if (instr.Direction == "forward")
                {
                    pos += instr.Value;
                    depth += aim * instr.Value;
                }
                else
                {
                    if (instr.Direction == "down")
                    {
                        aim += instr.Value;
                    }
                    else
                    {
                        aim -= instr.Value;
                    }
                }
            }

            Console.WriteLine($"Solution: {depth * pos}");
        }

        private static List<Instruction> Parse(List<string> data)
        {
            return data.Select(line => line.Split(' '))
                .Select(arr => new Instruction(arr[0], int.Parse(arr[1])))
                .ToList();
        }
    }

    internal record Instruction(string Direction, int Value);
}