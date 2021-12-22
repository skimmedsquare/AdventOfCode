using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AdventOfCode.Year2021.Day15
{
    public class PathSolver : ISolver
    {
        private static readonly List<(int, int)> Offsets = new()
        {
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1)
        };

        private static readonly int[,] AdditionalRisk = new int[5, 5]
        {
            { 0, 1, 2, 3, 4 },
            { 1, 2, 3, 4, 5 },
            { 2, 3, 4, 5, 6 },
            { 3, 4, 5, 6, 7 },
            { 4, 5, 6, 7, 8 }
        };

        public void SolvePartOne(List<string> data)
        {
            Node[,] nodes = Parse(data);
            LinkNodes(nodes);
            FindShortestPath(nodes, nodes[0, 0]);

            Console.WriteLine(nodes[nodes.GetLength(0) - 1, nodes.GetLength(1) - 1].DistanceFromStart);
            Node? currentNode = nodes[nodes.GetLength(0) - 1, nodes.GetLength(1) - 1];
            HashSet<Node> path = new();
            while (currentNode != null)
            {
                path.Add(currentNode);
                Console.WriteLine($"{currentNode.X}, {currentNode.Y}: {currentNode.Value}");
                currentNode = currentNode.Previous;
            }

            PrintNodes(nodes, path);
        }

        private static Node[,] Parse(List<string> data)
        {
            var result = new Node[data.Count, data.Count];

            var i = 0;
            foreach (string line in data)
            {
                var j = 0;
                foreach (char c in line)
                {
                    result[i, j] = new Node(j, i, c - '0'); // Rather odd way of converting but... it works?
                    j++;
                }

                i++;
            }

            return result;
        }

        private static void PrintNodes(Node[,] nodes, HashSet<Node> path)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < nodes.GetLength(0); i++)
            {
                for (var j = 0; j < nodes.GetLength(1); j++)
                {
                    if (path.Contains(nodes[i, j]))
                    {
                        builder.Append('*').Append(nodes[i, j].Value).Append('*');
                    }
                    else
                    {
                        builder.Append(' ').Append(nodes[i, j].Value).Append(' ');
                    }
                }

                builder.Append('\n');
            }

            Console.WriteLine(builder.ToString());
        }

        private static void LinkNodes(Node[,] nodes)
        {
            for (var i = 0; i < nodes.GetLength(0); i++)
            {
                for (var j = 0; j < nodes.GetLength(1); j++)
                {
                    foreach ((int, int) offset in Offsets)
                    {
                        Node currentNode = nodes[i, j];
                        int newI = i + offset.Item1;
                        int newJ = j + offset.Item2;

                        if (newI >= 0 && newI < nodes.GetLength(0) && newJ >= 0 && newJ < nodes.GetLength(1))
                        {
                            currentNode.AddNeighbor(nodes[newI, newJ]);
                        }
                    }
                }
            }
        }

        private static void FindShortestPath(Node[,] nodes, Node start)
        {
            // Soo... the really odd thing about SortedSet is that it uses the comparison function
            // even for equality, not the equals method.
            SortedSet<Node> sortedSet = new();
            start.DistanceFromStart = 0;
            foreach (Node node in nodes)
            {
                if (!node.Equals(start))
                {
                    sortedSet.Add(node);
                }
            }

            bool firstIter = true;
            while (sortedSet.Count != 0)
            {
                Node workingNode;
                if (firstIter)
                {
                    workingNode = start;
                    firstIter = false;
                }
                else
                {
                    workingNode = sortedSet.Min!;
                    sortedSet.Remove(workingNode);
                }

                workingNode.Visited = true;

                var neighbors = from neighbor in workingNode.Neighbors
                                where !neighbor.Visited && sortedSet.Contains(neighbor)
                                select neighbor;

                foreach (Node neighbor in neighbors)
                {
                    int tempDistance = workingNode.DistanceFromStart + neighbor.Value;
                    if (tempDistance < neighbor.DistanceFromStart)
                    {
                        sortedSet.Remove(neighbor);

                        neighbor.DistanceFromStart = tempDistance;
                        neighbor.Previous = workingNode;

                        sortedSet.Add(neighbor);
                    }
                }
            }
        }

        public void SolvePartTwo(List<string> data)
        {
            Node[,] nodes = Parse(data);
            Node[,] expandedNodes = ExpandGrid(nodes);
            LinkNodes(expandedNodes);
            FindShortestPath(expandedNodes, expandedNodes[0, 0]);
            Console.WriteLine(expandedNodes[expandedNodes.GetLength(0) - 1, expandedNodes.GetLength(1) - 1].DistanceFromStart);
        }

        private static Node[,] ExpandGrid(Node[,] nodes)
        {
            Node[,] result = new Node[nodes.GetLength(0) * AdditionalRisk.GetLength(0), nodes.GetLength(1) * AdditionalRisk.GetLength(1)];
            for (var offsetY = 0; offsetY < AdditionalRisk.GetLength(0); offsetY++)
            {
                for (var offsetX = 0; offsetX < AdditionalRisk.GetLength(1); offsetX++)
                {
                    for (var origY = 0; origY < nodes.GetLength(0); origY++)
                    {
                        for (var origX = 0; origX < nodes.GetLength(1); origX++)
                        {
                            int newX = (offsetX * nodes.GetLength(1)) + origX;
                            int newY = (offsetY * nodes.GetLength(0)) + origY;

                            int newValue = nodes[origX, origY].Value + AdditionalRisk[offsetX, offsetY];
                            if (newValue > 9)
                            {
                                newValue -= 9;
                            }

                            result[newX, newY] = new Node(newX, newY, newValue);
                        }
                    }
                }
            }

            return result;
        }
    }

    internal class Node : IComparable<Node>
    {
        public int X { get; }
        public int Y { get; }
        public int Value { get; }
        public int DistanceFromStart { get; set; }

        public Node? Previous { get; set; }

        public bool Visited { get; set; }

        public HashSet<Node> Neighbors { get; }

        public Node(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
            DistanceFromStart = int.MaxValue;
            Previous = null;
            Visited = false;
            Neighbors = new();
        }

        public void AddNeighbor(Node node)
        {
            Neighbors.Add(node);
        }

        public int CompareTo(Node? other)
        {
            if (other == null)
            {
                return 1;
            }

            int distanceCompare = this.DistanceFromStart.CompareTo(other.DistanceFromStart);
            if (distanceCompare == 0)
            {
                int xCompare = this.X.CompareTo(other.X);
                if (xCompare == 0)
                {
                    return this.Y.CompareTo(other.Y);
                }
                else
                {
                    return xCompare;
                }
            }
            else
            {
                return distanceCompare;
            }
        }

        public virtual bool Equals(Node n)
        {
            return n != null && n.X == X && n.Y == Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}