namespace Aoc2021.Day5 {
    public class VentSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            List<Line> straightLines = Parse(data).Where(l => l.IsStraight()).ToList();

            Point max = FindBounds(straightLines);
            Console.WriteLine($"X: {max.X}, Y: {max.Y}");

            var grid = new int[max.X+1, max.Y+1];
            foreach (Line line in straightLines) {
                foreach (Point point in line.Points()) {
                    grid[point.X, point.Y]++;
                }
            }

            // Can't really print anything except the example prompt to console
            // PrintGrid(grid);

            var count = 0;
            for (var i = 0; i < max.X + 1; i++) {
                for (var j = 0; j < max.Y + 1; j++) {
                    if (grid[i,j] >= 2) {
                        Console.WriteLine($"Found at {i},{j}");
                        count++;
                    }
                }
            }

            Console.WriteLine(count);
        }

        private static Point FindBounds(in List<Line> lines)
        {
            return new Point() {
                X = (from line in lines
                     select Math.Max(line.Start.X, line.End.X)).Max(),
                Y = (from line in lines
                     select Math.Max(line.Start.Y, line.End.Y)).Max()
            };
        }

        private static List<Line> Parse(in List<string> data) {
            var result = new List<Line>();
            foreach (string line in data) {
                var splitLine = line.Split("->");
                result.Add(new Line() { Start = ReadPoint(splitLine[0]), End = ReadPoint(splitLine[1]) });
            }
            return result;
        }

        private static Point ReadPoint(string input) {
            string[] splitInput = input.Trim().Split(',');
            return new Point() { X = int.Parse(splitInput[0]), Y = int.Parse(splitInput[1])};
        }

        private static void PrintGrid(in int[,] grid) {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++) {
                    Console.Write("{0} ", grid[i, j]);
                }
                Console.WriteLine();
            }
        }
        public void SolvePartTwo(in List<string> data)
        {
            List<Line> inputLines = Parse(data).ToList();
            Point max = FindBounds(inputLines);
            Console.WriteLine($"X: {max.X}, Y: {max.Y}");

            var grid = new int[max.X+1, max.Y+1];
            foreach (Line line in inputLines) {
                foreach (Point point in line.Points()) {
                    grid[point.X, point.Y]++;
                }
            }

            // Can't really print anything except the example prompt to console
            // PrintGrid(grid);

            var count = 0;
            for (int i = 0; i < max.X + 1; i++) {
                for (int j = 0; j < max.Y + 1; j++) {
                    if (grid[i,j] >= 2) {
                        Console.WriteLine($"Found at {i},{j}");
                        count++;
                    }
                }
            }

            Console.WriteLine(count);
        }
    }

    public struct Point {
        public int X;
        public int Y;
    }

    public struct Line {
        public Point Start;
        public Point End;

        public bool IsStraight() {
            return Start.X == End.X || Start.Y == End.Y;
        }

        public List<Point> Points() {
            int xDir = End.X - Start.X;
            int yDir = End.Y - Start.Y;

            var ret = new List<Point>();

            // Part 1 Simplification
            if (xDir != 0 && yDir == 0) {
                var inc = xDir < 0 ? -1 : 1;
                var curX = Start.X;
                while (curX != End.X) {
                    ret.Add(new Point() {X = curX, Y = Start.Y});
                    curX += inc;
                }
                ret.Add(End);
            } else if (xDir == 0 && yDir != 0) {
                var inc = yDir < 0 ? -1 : 1;
                var curY = Start.Y;
                while (curY != End.Y) {
                    ret.Add(new Point() {X = Start.X, Y = curY});
                    curY += inc;
                }
                ret.Add(End);
            } else {
                var xInc = xDir < 0 ? -1 : 1;
                var yInc = yDir < 0 ? -1 : 1;

                var curPoint = new Point() { X = Start.X, Y = Start.Y };
                while (!curPoint.Equals(End)) {
                    ret.Add(curPoint);

                    curPoint.X += xInc;
                    curPoint.Y += yInc;
                }
                ret.Add(End);
            }

            return ret;
        }
    }
}