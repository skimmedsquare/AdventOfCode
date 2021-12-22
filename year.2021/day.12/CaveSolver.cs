namespace AdventOfCode.Year2021.Day12
{
    public class CaveSolver : ISolver
    {
        public void SolvePartOne(List<string> data)
        {
            Dictionary<string, Node> nodes = Parse(data);
            Node start = nodes["start"];

            List<string>? paths = start.FindAllPaths(new HashSet<string>());
            Console.WriteLine(string.Join('\n', paths ?? new List<string>()));
            Console.WriteLine(paths!.Count);
        }

        public void SolvePartTwo(List<string> data)
        {
            Dictionary<string, Node> nodes = Parse(data);
            Node start = nodes["start"];

            List<string>? paths = start.FindAllPathsPartTwo(new HashSet<string>(), false, 0);
            Console.WriteLine(string.Join('\n', paths ?? new List<string>()));
            Console.WriteLine(paths!.Count);
        }

        private static Dictionary<string, Node> Parse(List<string> data)
        {
            Dictionary<string, Node> nodes = new();
            foreach (string line in data)
            {
                string[] splitLine = line.Split('-');
                if (!nodes.TryGetValue(splitLine[0], out Node? leftNode))
                {
                    leftNode = new Node(splitLine[0]);
                    nodes.Add(splitLine[0], leftNode);
                }

                if (!nodes.TryGetValue(splitLine[1], out Node? rightNode))
                {
                    rightNode = new Node(splitLine[1]);
                    nodes.Add(splitLine[1], rightNode);
                }

                leftNode.AddNeighbor(rightNode);
                rightNode.AddNeighbor(leftNode);
            }

            return nodes;
        }
    }

    internal class Node
    {
        public string Name { get; }
        public VisitType Type { get; }

        private readonly HashSet<Node> neighbors;

        public Node(string name)
        {
            Name = name;
            Type = char.IsUpper(name[0]) ? VisitType.Unlimited : VisitType.Single;

            neighbors = new();
        }

        public void AddNeighbor(Node node)
        {
            neighbors.Add(node);
        }

        public List<string>? FindAllPaths(HashSet<string> seenNodes)
        {
            // Base case, at end node - finish recursion
            if (Name == "end")
            {
                return new List<string> { "end" };
            }

            // Check all neighbors
            List<Node> validNeighbors = (from neighbor in neighbors
                                         where neighbor.Type == VisitType.Unlimited || (neighbor.Type == VisitType.Single && !seenNodes.Contains(neighbor.Name))
                                         select neighbor).ToList();

            if (validNeighbors.Count == 0)
            {
                // If we can't go anywhere, we return 'null' for this path
                return null;
            }

            HashSet<string> copy = new(seenNodes);
            copy.Add(Name);
            List<List<string>?> pathResult = new();
            foreach (Node neighbor in validNeighbors)
            {
                pathResult.Add(neighbor.FindAllPaths(copy));
            }

            List<string> nonEmptyResults = (from resultList in pathResult
                                            where resultList != null
                                            from result in resultList
                                            select result).ToList();

            if (nonEmptyResults.Count == 0)
            {
                return null;
            }

            return (from result in nonEmptyResults
                    select Name + ',' + result).ToList();
        }

        public List<string>? FindAllPathsPartTwo(HashSet<string> seenNodes, bool hasVisitedCaveTwice, int depth)
        {
            // Base case, at end node - finish recursion
            if (Name == "end")
            {
                return new List<string> { "end" };
            }

            // Check all neighbors... wow this conditional is awful.
            List<Node> validNeighbors = (from neighbor in neighbors
                                         where neighbor.Type == VisitType.Unlimited || (neighbor.Type == VisitType.Single && neighbor.Name != "start" && (!hasVisitedCaveTwice || !seenNodes.Contains(neighbor.Name)))
                                         select neighbor).ToList();

            if (validNeighbors.Count == 0)
            {
                // If we can't go anywhere, we return 'null' for this path
                return null;
            }

            HashSet<string> copy = new(seenNodes);
            copy.Add(Name);
            List<List<string>?> pathResult = new();
            foreach (Node neighbor in validNeighbors)
            {
                if (neighbor.Type == VisitType.Single && seenNodes.Contains(neighbor.Name))
                {
                    pathResult.Add(neighbor.FindAllPathsPartTwo(copy, true, depth + 1));
                }
                else
                {
                    pathResult.Add(neighbor.FindAllPathsPartTwo(copy, hasVisitedCaveTwice, depth + 1));
                }
            }

            List<string> nonEmptyResults = (from resultList in pathResult
                                            where resultList != null
                                            from result in resultList
                                            select result).ToList();

            if (nonEmptyResults.Count == 0)
            {
                return null;
            }

            return (from result in nonEmptyResults
                    select Name + ',' + result).ToList();
        }
    }

    internal enum VisitType
    {
        Single,
        Unlimited
    }
}