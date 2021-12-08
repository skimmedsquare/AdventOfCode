namespace Aoc2021.Day2
{
    class DirectionSolver : ISolver
    {
        public void SolvePartOne()
        {
            var depth = 0;
            var pos = 0;

            List<Instruction> instructions = GetInstructions();

            foreach (Instruction instr in instructions)
            {
                if ("forward" == instr.Direction)
                {
                    pos += instr.Value;
                }
                else
                {
                    if ("down" == instr.Direction)
                    {
                        depth += instr.Value;
                    }
                    else /* "up" */
                    {
                        depth -= instr.Value;
                    }
                }
            }

            Console.WriteLine($"Solution: {depth * pos}");
        }

        public void SolvePartTwo()
        {
            var depth = 0;
            var pos = 0;
            var aim = 0;

            List<Instruction> instructions = GetInstructions();

            foreach (Instruction instr in instructions)
            {
                if ("forward" == instr.Direction)
                {
                    pos += instr.Value;
                    depth += aim * instr.Value;
                }
                else
                {
                    if ("down" == instr.Direction)
                    {
                        aim += instr.Value;
                    }
                    else /* "up" */
                    {
                        aim -= instr.Value;
                    }
                }
            }

            Console.WriteLine($"Solution: {depth * pos}");
        }

        static List<Instruction> GetInstructions()
        {
            return ReadFile().Select(line => line.Split(' '))
                             .Select(arr => new Instruction(arr[0], int.Parse(arr[1])))
                             .ToList();
        }

        static List<String> ReadFile()
        {
            return System.IO.File.ReadLines("day.two/directions.txt").ToList();
        }

        static List<String> ReadStatic()
        {
            string[] hc = {
                "forward 5",
                "down 5",
                "forward 8",
                "up 3",
                "down 8",
                "forward 2"
            };
            return hc.ToList();
        }
    }


    record Instruction(string Direction, int Value);
}