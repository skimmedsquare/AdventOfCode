namespace AdventOfCode.Year2021.Day14
{
    public class PolymerSolver : ISolver
    {
        private const int PartTwoDepth = 40;

        public void SolvePartOne(in List<string> data)
        {
            Parse(data, out LinkedList<Pair> chain, out Dictionary<Pair, char> mapping);
            Solve(chain, mapping, 10);
        }

        private static void Solve(LinkedList<Pair> chain, Dictionary<Pair, char> mapping, int numIterations)
        {
            for (var i = 0; i < numIterations; i++)
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                for (LinkedListNode<Pair> node = chain.First!; node != null; node = node.Next!)
                {
                    if (node == chain.First)
                    {
                        continue;
                    }

                    LinkedListNode<Pair> prevNode = node.Previous!;

                    if (mapping.TryGetValue(prevNode.Value, out char newPolymer))
                    {
                        char existingR = prevNode.Value.R;
                        prevNode.ValueRef.R = newPolymer;

                        chain.AddAfter(prevNode, new Pair { L = newPolymer, R = existingR });
                    }
                }

                LinkedListNode<Pair> lastNode = chain.Last!;
                if (mapping.TryGetValue(lastNode.Value, out char lastPolymer))
                {
                    char existingR = lastNode.Value.R;
                    lastNode.ValueRef.R = lastPolymer;

                    chain.AddAfter(lastNode, new Pair { L = lastPolymer, R = existingR });
                }

                watch.Stop();
                Console.WriteLine($"Iteration {i + 1} took {watch.ElapsedMilliseconds}ms, size: {chain.Count}");
            }

            Dictionary<char, uint> counting = new();
            foreach (Pair pair in chain)
            {
                if (counting.TryGetValue(pair.L, out _))
                {
                    counting[pair.L]++;
                }
                else
                {
                    counting[pair.L] = 1;
                }
            }

            char lastChar = chain.Last!.Value.R;
            if (counting.TryGetValue(lastChar, out _))
            {
                counting[lastChar]++;
            }
            else
            {
                counting[lastChar] = 1;
            }

            List<uint> values = counting.Values.ToList();
            values.Sort();
            Console.WriteLine(values[^1] - values[0]);
        }

        private static void Parse(in List<string> data, out LinkedList<Pair> chain, out Dictionary<Pair, char> mapping)
        {
            chain = new LinkedList<Pair>();
            mapping = new Dictionary<Pair, char>();

            string template = data[0];
            for (var i = 1; i < template.Length; i++)
            {
                chain.AddLast(new Pair { L = template[i - 1], R = template[i] });
            }

            data.RemoveAt(0);
            data.RemoveAt(0);
            foreach (string line in data)
            {
                string[] formula = line.Split(" -> ");
                mapping.Add(new Pair { L = formula[0][0], R = formula[0][1] }, formula[1][0]);
            }
        }

        private static void ParseV2(in List<string> data, out List<Pair> chain, out Dictionary<Pair, ResultPair> mapping)
        {
            chain = new List<Pair>();
            mapping = new Dictionary<Pair, ResultPair>();

            string template = data[0];
            for (var i = 1; i < template.Length; i++)
            {
                chain.Add(new Pair { L = template[i - 1], R = template[i] });
            }

            data.RemoveAt(0);
            data.RemoveAt(0);
            foreach (string line in data)
            {
                string[] formula = line.Split(" -> ");

                char originalL = formula[0][0];
                char originalR = formula[0][1];
                char newPolymer = formula[1][0];

                mapping.Add(
                    new Pair { L = originalL, R = originalR },
                    new ResultPair { L = new Pair { L = originalL, R = newPolymer }, R = new Pair { L = newPolymer, R = originalR } }
                );
            }
        }

        public void SolvePartTwo(in List<string> data)
        {
            ParseV2(data, out List<Pair> chain, out Dictionary<Pair, ResultPair> mapping);
            Dictionary<PairDepthKey, Dictionary<char, ulong>> memoize = new();
            Dictionary<char, ulong> result = new();
            foreach (Pair pair in chain)
            {
                Dictionary<char, ulong> currentResult = SolveV2(pair, mapping, ref memoize, 0);
                result = Merge(result, currentResult);
            }

            if (result.TryGetValue(chain.Last().R, out ulong val))
            {
                result[chain.Last().R] = val + 1;
            }
            else
            {
                result[chain.Last().R] = 0;
            }

            List<ulong> values = result.Values.ToList();
            values.Sort();
            Console.WriteLine(values[^1] - values[0]);
        }

        private static Dictionary<char, ulong> SolveV2(Pair pair, in Dictionary<Pair, ResultPair> mapping, ref Dictionary<PairDepthKey, Dictionary<char, ulong>> memoize, int depth)
        {
            if (depth == PartTwoDepth)
            {
                return new Dictionary<char, ulong>() { { pair.L, 1 } };
            }

            if (!mapping.TryGetValue(pair, out ResultPair resultPair))
            {
                return new Dictionary<char, ulong>();
            }

            Dictionary<char, ulong> resultL = FetchOrCalculate(resultPair.L, mapping, ref memoize, depth);
            Dictionary<char, ulong> resultR = FetchOrCalculate(resultPair.R, mapping, ref memoize, depth);

            Dictionary<char, ulong> result = Merge(resultL, resultR);

            return result;
        }

        private static Dictionary<char, ulong> FetchOrCalculate(Pair pair, in Dictionary<Pair, ResultPair> mapping, ref Dictionary<PairDepthKey, Dictionary<char, ulong>> memoize, int depth)
        {
            Dictionary<char, ulong> result;
            PairDepthKey leftKey = new PairDepthKey() { Pair = pair, Depth = depth + 1 };
            if (memoize.TryGetValue(leftKey, out Dictionary<char, ulong>? stored))
            {
                result = stored;
            }
            else
            {
                result = SolveV2(pair, mapping, ref memoize, depth + 1);
                memoize[leftKey] = result;
            }

            return result;
        }

        private static Dictionary<char, ulong> Merge(in Dictionary<char, ulong> dictOne, in Dictionary<char, ulong> dictTwo)
        {
            Dictionary<char, ulong> result = new(dictOne);
            foreach (KeyValuePair<char, ulong> entry in dictTwo)
            {
                if (result.TryGetValue(entry.Key, out ulong existingValue))
                {
                    result[entry.Key] = existingValue + entry.Value;
                }
                else
                {
                    result[entry.Key] = entry.Value;
                }
            }

            return result;
        }

        private static void PrintResultDict(Dictionary<char, ulong> resultDict)
        {
            foreach (KeyValuePair<char, ulong> entry in resultDict)
            {
                Console.WriteLine($"{entry.Key}, {entry.Value}");
            }
        }
    }

    internal struct Pair
    {
        public char L;
        public char R;
    }

    internal struct ResultPair
    {
        public Pair L;
        public Pair R;
    }

    internal struct PairDepthKey
    {
        public Pair Pair;
        public int Depth;
    }
}