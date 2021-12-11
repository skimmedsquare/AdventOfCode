namespace AdventOfCode.Year2021.Day10
{
    public class SyntaxSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            List<Line> lines = Parse(data);
            List<char> illegalChars = (from line in lines
                                       select line.FirstIllegalCharacter() into illegalChar
                                       where illegalChar != '0'
                                       select illegalChar).ToList();

            int solution = (from illegalChar in illegalChars
                            select Line.IllegalCharValue(illegalChar)).Sum();

            Console.WriteLine(solution);
        }

        public void SolvePartTwo(in List<string> data)
        {
            List<Line> lines = Parse(data);
            List<Line> incompleteLines = (from line in lines
                                          where line.FirstIllegalCharacter() == '0'
                                          select line).ToList();

            List<long> scores = (from line in incompleteLines
                                select Line.AutoCompleteCharsValue(line.AutoCompleteChars())).ToList();

            scores.Sort();

            Console.WriteLine(string.Join('\n', scores));
            Console.WriteLine($"Solution: {scores[scores.Count / 2]}");
        }

        private static List<Line> Parse(in List<string> data)
        {
            return (from line in data
                    select new Line(line)).ToList();
        }
    }

    internal class Line
    {
        private static readonly Dictionary<char, char> OpenToCloseCharacter = new()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' }
        };

        private static readonly Dictionary<char, int> IllegalCharToValue = new()
        {
            { ')', 3 },
            { ']', 57 },
            { '}', 1197 },
            { '>', 25137 }
        };

        private static readonly Dictionary<char, int> AutoCompleteCharToValue = new()
        {
            { ')', 1 },
            { ']', 2 },
            { '}', 3 },
            { '>', 4 }
        };
        private readonly char[] data;

        public Line(string data)
        {
            this.data = data.ToCharArray();
        }

        public char FirstIllegalCharacter()
        {
            Stack<char> validator = new();
            for (var i = 0; i < data.Length; i++)
            {
                if (OpenToCloseCharacter.ContainsKey(data[i]))
                {
                    validator.Push(OpenToCloseCharacter[data[i]]);
                }
                else
                {
                    char expectedChar = validator.Pop();
                    if (expectedChar != data[i])
                    {
                        // Console.WriteLine($"Expected {expectedChar}, but found {data[i]} instead");
                        return data[i];
                    }
                }
            }

            return '0';
        }

        public List<char> AutoCompleteChars()
        {
            Stack<char> validator = new();
            for (var i = 0; i < data.Length; i++)
            {
                if (OpenToCloseCharacter.ContainsKey(data[i]))
                {
                    validator.Push(OpenToCloseCharacter[data[i]]);
                }
                else
                {
                    validator.Pop();
                }
            }

            List<char> charsNeeded = new();
            while (validator.TryPop(out char result))
            {
                charsNeeded.Add(result);
            }

            Console.WriteLine($"Incomplete String: {string.Join("", data)}, Needed Values: {string.Join("", charsNeeded)}");

            return charsNeeded;
        }

        public static int IllegalCharValue(char illegalChar)
        {
            return IllegalCharToValue[illegalChar];
        }

        public static long AutoCompleteCharsValue(in List<char> autoCompleteChars)
        {
            long sum = 0;
            foreach (char c in autoCompleteChars)
            {
                sum *= 5;
                sum += AutoCompleteCharToValue[c];
            }

            Console.WriteLine($"{string.Join("", autoCompleteChars)}, {sum}");

            return sum;
        }
    }
}