using System.Collections;
using System.Text;

namespace AdventOfCode.Year2021.Day13
{
    public class FoldSolver : ISolver
    {
        public void SolvePartOne(List<string> data)
        {
            Parse(data, out List<Point> initialPoints, out List<Instruction> instructions);
            Point bounds = FindMaxBounds(initialPoints);
            BitArray[] paper = FillArray(initialPoints, bounds);
            PrintPaper(paper);
            PrintPaperWithLine(paper, instructions[0]);
            FoldPaper(ref paper, instructions, 1);

            Console.WriteLine(paper.Sum(array => array.NumTrue()));
        }

        private static void FoldPaper(ref BitArray[] paper, List<Instruction> instructions, int numInstructions = -1)
        {
            var instructionCount = 0;
            foreach (Instruction instruction in instructions)
            {
                if (instructionCount == numInstructions)
                {
                    break;
                }

                if (instruction.Type == FoldType.Y)
                {
                    var newYMax = 0;
                    if (paper.Length % 2 == 1)
                    {
                        newYMax = (paper.Length - 1) / 2; // 7
                    }
                    else
                    {
                        newYMax = paper.Length / 2;
                    }

                    var foldedPaper = new BitArray[newYMax];
                    foldedPaper[0] = paper[0];

                    for (var i = 1; (instruction.Value - i) >= 0 && (instruction.Value + i) < paper.Length; i++)
                    {
                        foldedPaper[newYMax - i] = paper[instruction.Value - i].Or(paper[instruction.Value + i]);
                    }

                    paper = foldedPaper;
                }
                else
                {
                    var newXMax = (paper[0].Length - 1) / 2;
                    var foldedPaper = new BitArray[paper.Length];
                    for (int i = 0; i < foldedPaper.Length; i++)
                    {
                        foldedPaper[i] = new BitArray(newXMax);
                        for (int j = 0; j < newXMax; j++)
                        {
                            foldedPaper[i][j] = paper[i][j] || paper[i][paper[i].Length - 1 - j];
                        }
                    }

                    paper = foldedPaper;
                }

                instructionCount++;
            }
        }

        private static void PrintPaperWithLine(BitArray[] paper, Instruction instruction)
        {
            var builder = new StringBuilder();
            var y = 0;
            foreach (BitArray array in paper)
            {
                var x = 0;
                foreach (bool bit in array)
                {
                    if (instruction.Type == FoldType.Y && y == instruction.Value)
                    {
                        builder.Append('-');
                    }
                    else if (instruction.Type == FoldType.X && x == instruction.Value)
                    {
                        builder.Append('|');
                    }
                    else
                    {
                        builder.Append(bit ? '#' : '.');
                    }

                    x++;
                }

                y++;
                builder.Append('\n');
            }

            Console.WriteLine(builder);
        }

        private static void PrintPaper(BitArray[] paper)
        {
            var builder = new StringBuilder();
            foreach (BitArray array in paper)
            {
                foreach (bool bit in array)
                {
                    builder.Append(bit ? '#' : '.');
                }

                builder.Append('\n');
            }

            Console.WriteLine(builder);
        }

        private static BitArray[] FillArray(List<Point> initialPoints, Point maxBounds)
        {
            BitArray[] paper = new BitArray[maxBounds.Y + 1];
            for (int i = 0; i < maxBounds.Y + 1; i++)
            {
                paper[i] = new BitArray(maxBounds.X + 1);
            }

            foreach (Point point in initialPoints)
            {
                paper[point.Y][point.X] = true;
            }

            return paper;
        }

        private static void Parse(List<string> data, out List<Point> initialPoints, out List<Instruction> instructions)
        {
            initialPoints = new List<Point>();
            instructions = new List<Instruction>();

            var hasEncounteredNewLine = false;
            foreach (string line in data)
            {
                if (!hasEncounteredNewLine)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        hasEncounteredNewLine = true;
                        continue;
                    }
                    else
                    {
                        string[] splitLine = line.Split(',');
                        initialPoints.Add(new Point(int.Parse(splitLine[0]), int.Parse(splitLine[1])));
                    }
                }
                else
                {
                    string[] splitPart = line.Split(' ')[2].Split('=');
                    instructions.Add(new Instruction(int.Parse(splitPart[1]), Enum.Parse<FoldType>(splitPart[0], true)));
                }
            }
        }

        private static Point FindMaxBounds(List<Point> points)
        {
            int maxX = points.MaxBy(p => p.X)!.X;
            int maxY = points.MaxBy(p => p.Y)!.Y;
            return new Point(maxX, maxY);
        }

        public void SolvePartTwo(List<string> data)
        {
            Parse(data, out List<Point> initialPoints, out List<Instruction> instructions);
            Point bounds = FindMaxBounds(initialPoints);
            BitArray[] paper = FillArray(initialPoints, bounds);
            Console.WriteLine(bounds);
            FoldPaper(ref paper, instructions);
            PrintPaper(paper);

            Console.WriteLine(paper.Sum(array => array.NumTrue()));
        }
    }

    internal record Point(int X, int Y)
    {
    }

    internal record Instruction(int Value, FoldType Type)
    {
    }

    internal enum FoldType
    {
        X,
        Y
    }

    public static class ExtensionMethods
    {
        public static int NumTrue(this BitArray arr)
        {
            var sum = 0;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                {
                    sum++;
                }
            }

            return sum;
        }
    }
}