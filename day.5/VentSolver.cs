namespace Aoc2021.Day5 {
    class VentSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            var straightLines = Parse(data).Where(l => l.IsStraight()).ToList();
            var x = 0;
            var y = 0;

            FindBounds(straightLines, out x, out y);
            Console.WriteLine($"X: {x}, Y: {y}");

            var grid = new int[x+1, y+1];
            foreach (var line in straightLines) {
                foreach (var point in line.Points()) {
                    grid[point.X, point.Y] += 1;
                }
            }

            // Can't really print anything except the example prompt to console
            // PrintGrid(grid);

            var count = 0;
            for (int i = 0; i < x + 1; i++) {
                for (int j = 0; j < y + 1; j++) {
                    if (grid[i,j] >= 2) {
                        Console.WriteLine($"Found at {i},{j}");
                        count++;
                    }
                }
            }

            Console.WriteLine(count);
        }

        void FindBounds(in List<Line> lines, out int x, out int y)
        {
            x = (from line in lines
                select Math.Max(line.Start.X, line.End.X)).Max();
                
            y = (from line in lines
                select Math.Max(line.Start.Y, line.End.Y)).Max();
        }

        List<Line> Parse(in List<string> data) {
            var result = new List<Line>();
            foreach (var line in data) {
                var splitLine = line.Split("->");
                result.Add(new Line() { Start = readPoint(splitLine[0]), End = readPoint(splitLine[1]) });
            }
            return result;
        }

        Point readPoint(string input) {
            var splitInput = input.Trim().Split(',');
            return new Point() { X = int.Parse(splitInput[0]), Y = int.Parse(splitInput[1])};
        }

        void PrintGrid(in int[,] grid) {
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
            var inputLines = Parse(data).ToList();
            var x = 0;
            var y = 0;

            FindBounds(inputLines, out x, out y);
            Console.WriteLine($"X: {x}, Y: {y}");

            var grid = new int[x+1, y+1];
            foreach (var line in inputLines) {
                foreach (var point in line.Points()) {
                    grid[point.X, point.Y] += 1;
                }
            }

            // Can't really print anything except the example prompt to console
            // PrintGrid(grid);

            var count = 0;
            for (int i = 0; i < x + 1; i++) {
                for (int j = 0; j < y + 1; j++) {
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
    }

    public static class ExtensionMethods {
        public static bool IsStraight (this Line line) {
            return line.Start.X == line.End.X || line.Start.Y == line.End.Y;
        }

        public static List<Point> Points(this Line line) {
            var xDir = line.End.X - line.Start.X;
            var yDir = line.End.Y - line.Start.Y;

            var ret = new List<Point>();

            // Part 1 Simplification
            if (xDir != 0 && yDir == 0) {
                var inc = xDir < 0 ? -1 : 1;                
                var curX = line.Start.X;
                while (curX != line.End.X) {
                    ret.Add(new Point() {X = curX, Y = line.Start.Y});
                    curX += inc;
                }
                ret.Add(line.End);
            } else if (xDir == 0 && yDir != 0) {
                var inc = yDir < 0 ? -1 : 1;                
                var curY = line.Start.Y;
                while (curY != line.End.Y) {
                    ret.Add(new Point() {X = line.Start.X, Y = curY});
                    curY += inc;
                }
                ret.Add(line.End);
            } else {
                var xInc = xDir < 0 ? -1 : 1;
                var yInc = yDir < 0 ? -1 : 1;

                var curPoint = new Point() { X = line.Start.X, Y = line.Start.Y };
                while (!curPoint.Equals(line.End)) {
                    ret.Add(curPoint);

                    curPoint.X += xInc;
                    curPoint.Y += yInc;
                }
                ret.Add(line.End);
            }

            return ret;
        }
    }
}