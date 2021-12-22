using System.Text;

namespace AdventOfCode.Year2021.Day9
{
    public class HeightSolver : ISolver
    {
        public void SolvePartOne(List<string> data)
        {
            Node[,] nodes = Parse(data);
            var lowPoints = new List<Node>();
            foreach (Node node in nodes)
            {
                if (!node.Visited)
                {
                    lowPoints.AddRange(node.FindLowPoints());
                }
            }

            Console.WriteLine((from node in lowPoints
                               select node.Risk).Sum());

            Console.WriteLine(FilledMap(nodes, lowPoints));
        }

        public void SolvePartTwo(List<string> data)
        {
            Node[,] nodes = Parse(data);
            var lowPoints = new List<Node>();
            foreach (Node node in nodes)
            {
                if (!node.Visited)
                {
                    lowPoints.AddRange(node.FindLowPoints());
                }
            }

            foreach (Node node in nodes)
            {
                node.Visited = false;
            }

            Console.WriteLine(lowPoints.Count);
            List<int> sizes = new();
            foreach (Node node in lowPoints)
            {
                int size = node.GetBasinSize();
                Console.WriteLine($"Basin rooted at ({node.X}, {node.Y}) has size {size}");
                sizes.Add(size);
            }

            sizes.Sort();
            sizes.Reverse();

            Console.WriteLine($"Solution: {sizes[0] * sizes[1] * sizes[2]}");
        }

        private static Node[,] Parse(List<string> data)
        {
            Node[,] nodes = PopulateNodes(data);
            return LinkNodes(nodes);
        }

        private static Node[,] PopulateNodes(List<string> data)
        {
            int x = data.Count;
            int y = data[0].Length;

            var ret = new Node[x, y];

            var i = 0;
            foreach (string line in data)
            {
                var j = 0;
                foreach (char c in line)
                {
                    ret[i, j] = new Node(height: int.Parse(c.ToString()), x: i, y: j);
                    j++;
                }

                i++;
            }

            return ret;
        }

        private static Node[,] LinkNodes(Node[,] nodes)
        {
            int maxX = nodes.GetLength(0);
            int maxY = nodes.GetLength(1);

            for (var x = 0; x < maxX; x++)
            {
                for (var y = 0; y < maxY; y++)
                {
                    Node currentNode = nodes[x, y];
                    if (x != 0)
                    {
                        currentNode.Top = nodes[x - 1, y];
                    }

                    if (x != maxX - 1)
                    {
                        currentNode.Bottom = nodes[x + 1, y];
                    }

                    if (y != 0)
                    {
                        currentNode.Left = nodes[x, y - 1];
                    }

                    if (y != maxY - 1)
                    {
                        currentNode.Right = nodes[x, y + 1];
                    }
                }
            }

            return nodes;
        }

        private static string FilledMap(Node[,] nodes, List<Node> lowPoints)
        {
            HashSet<string> points = (from point in lowPoints
                                      select point.Position).ToHashSet();

            var builder = new StringBuilder();
            for (var i = 0; i < nodes.GetLength(0); i++)
            {
                for (var j = 0; j < nodes.GetLength(1); j++)
                {
                    Node currentPoint = nodes[i, j];
                    if (points.Contains(currentPoint.Position))
                    {
                        builder.Append('*').Append(currentPoint.Height).Append("* ");
                    }
                    else
                    {
                        builder.Append(' ').Append(currentPoint.Height).Append("  ");
                    }
                }

                builder.Append('\n');
            }

            return builder.ToString();
        }
    }

    internal class Node
    {
        public static readonly Node Empty = new(height: -1, x: -1, y: -1) { Visited = true };
        private readonly Dictionary<Side, Node> sides;

        public int X { get; init; }
        public int Y { get; init; }

        public Node Left
        {
            get { return sides[Side.Left]; }
            set { sides[Side.Left] = value; }
        }

        public Node Right
        {
            get { return sides[Side.Right]; }
            set { sides[Side.Right] = value; }
        }

        public Node Top
        {
            get { return sides[Side.Top]; }
            set { sides[Side.Top] = value; }
        }

        public Node Bottom
        {
            get { return sides[Side.Bottom]; }
            set { sides[Side.Bottom] = value; }
        }

        public int Height { get; }

        public int Risk
        {
            get { return Height + 1; }
        }

        public bool Visited { get; set; }

        public static bool operator >=(Node a, Node b)
        {
            return a.Height >= b.Height;
        }

        public static bool operator <=(Node a, Node b)
        {
            return a.Height <= b.Height;
        }

        public Node(int height, int x, int y)
        {
            Height = height;
            X = x;
            Y = y;
            Visited = false;
            sides = new()
            {
                { Side.Left, Empty },
                { Side.Right, Empty },
                { Side.Top, Empty },
                { Side.Bottom, Empty }
            };
        }

        public string Position
        {
            get
            {
                var xStr = X < 10 ? "0" + X.ToString() : X.ToString();
                var yStr = Y < 10 ? "0" + Y.ToString() : Y.ToString();
                return $"{xStr},{yStr}";
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder($"Node: (({X}, {Y}):{Height}-{Visited})");

            return builder.ToString();
        }

        // This is WAY more complex than it needs to be, because I misunderstood the idea of a
        // low point / basin. There cannot be multiple "low points" in a single basin.
        public List<Node> FindLowPoints(int lowestSubtreeHeight = int.MaxValue)
        {
            // Console.WriteLine("\n///////////////////");
            // Console.WriteLine($"Visiting: ({X}, {Y})");
            Visited = true;

            List<Node> validNeighbors = (from entry in sides
                                         where entry.Value != Node.Empty && entry.Value.Height <= Height
                                         select entry.Value).ToList();

            // Base Case
            // If all neighbors are either empty (i.e. the edge) or higher then this is a low point
            if (validNeighbors.Count == 0)
            {
                // Console.WriteLine("Terminated, Found new low point\n");
                return new() { this };
            }

            // Alt Base Case
            // If of the non-empty neighbors, all are visited, then this is NOT a low point
            List<Node> nonVisitedNeighbors = (from node in validNeighbors
                                              where !node.Visited
                                              select node).ToList();

            int lowestNeighbor = (from entry in sides
                                  where entry.Value != Node.Empty
                                  select entry.Value.Height).Min();

            int lowestSeenValue = Math.Min(lowestNeighbor, lowestSubtreeHeight);

            if (nonVisitedNeighbors.Count == 0)
            {
                List<Node> sameHeightNeighbors = (from node in validNeighbors
                                                  where node.Height == Height
                                                  select node).ToList();
                if (sameHeightNeighbors.Count == validNeighbors.Count && Height == lowestSeenValue)
                {
                    // Console.WriteLine("Terminated, Current point is equal with all neighbors\n");
                    return new() { this };
                }
                else
                {
                    // Console.WriteLine("Terminated, Found no low point \n");
                    return new();
                }
            }

            // Recursive case
            var results = new List<Node>();
            foreach (Node node in nonVisitedNeighbors)
            {
                // We do a second Visited check, because its possible that a path has already visited this node.
                if (!node.Visited)
                {
                    // Console.WriteLine($"Starting at: ({X}, {Y})");
                    // Console.WriteLine($"Going to: ({node.X}, {node.Y})");
                    results.AddRange(node.FindLowPoints(lowestSeenValue));
                }
            }

            if (results.Count != 0)
            {
                int lowestHeight = results.ConvertAll(node => node.Height).Min();
                if (lowestHeight == Height)
                {
                    // Console.WriteLine($"Current node ({X}, {Y}) is min ({lowestHeight}) of all nodes found in this subtree, also adding it!");
                    results.Add(this);
                }
            }

            return results;
        }

        public int GetBasinSize()
        {
            Visited = true;

            // If all adjacent points are visited or 9's, this node is the end case of a basin
            int numFilledNodes = (from node in sides.Values
                                  where node.Visited || node.Height == 9 || node == Node.Empty
                                  select node).Count();

            if (numFilledNodes == 4)
            {
                return 1;
            }

            var runningTotal = 1;
            foreach (Node neighbor in sides.Values)
            {
                if (!neighbor.Visited && neighbor.Height != 9 && neighbor != Node.Empty)
                {
                    runningTotal += neighbor.GetBasinSize();
                }
            }

            return runningTotal;
        }
    }

    internal enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }
}