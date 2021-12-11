using System.Text;

namespace AdventOfCode.Year2021.Day8
{
    public class DigitSolver : ISolver
    {
        private static readonly Dictionary<string, int> SignalToDigitMapping = new()
        {
            { "abcefg", 0 },
            { "cf", 1 },
            { "acdeg", 2 },
            { "acdfg", 3 },
            { "bcdf", 4 },
            { "abdfg", 5 },
            { "abdefg", 6 },
            { "acf", 7 },
            { "abcdefg", 8 },
            { "abcdfg", 9 }
        };

        public void SolvePartOne(in List<string> data)
        {
            var validLengths = new HashSet<int>() { 2, 3, 4, 7 };

            List<Display> displays = Parse(data);
            int validNums = displays.Select(d => d.Values)
                .Select(v => v.Digits.Count(digit => validLengths.Contains(digit.Length)))
                .Sum();

            Console.WriteLine(validNums);
        }

        private static List<Display> Parse(in List<string> data)
        {
            var output = new List<Display>();
            foreach (string line in data)
            {
                string[] parts = line.Split('|');
                List<string> signals = parts[0].Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

                List<string> digits = parts[1].Split(' ')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                output.Add(new Display(signals, new OutputValue(digits)));
            }

            return output;
        }

        public void SolvePartTwo(in List<string> data)
        {
            List<Display> displays = Parse(data);
            var sum = 0;
            foreach (Display display in displays)
            {
                Dictionary<char, char> mapping = FindMapping(display);
                int value = DecodeOutput(mapping, display.Values);
                Console.WriteLine(value);
                sum += value;
            }

            Console.WriteLine(sum);
        }

        private static int DecodeOutput(in Dictionary<char, char> mapping, OutputValue value)
        {
            var builder = new StringBuilder();
            foreach (string digit in value.Digits)
            {
                builder.Append(DecodeOutput(mapping, digit));
            }

            return int.Parse(builder.ToString());
        }

        private static int DecodeOutput(in Dictionary<char, char> mapping, string value)
        {
            var builder = new StringBuilder();
            foreach (char c in value)
            {
                builder.Append(mapping[c]);
            }

            return SignalToDigitMapping[builder.ToString().Sort()];
        }

        private static Dictionary<char, char> FindMapping(Display display)
        {
            var codeToActual = new Dictionary<char, char>();
            var actualToCode = new Dictionary<char, char>();

            ////////////////////////
            // Extract top value (a)
            var oneDisplay = (from signal in display.Signals
                              where signal.Length == 2
                              select signal).First();

            var sevenDisplay = (from signal in display.Signals
                                where signal.Length == 3
                                select signal).First();

            WriteToDictionary(sevenDisplay.Signals(), oneDisplay.Signals(), 'a', ref codeToActual, ref actualToCode);

            ///////////////////////////
            // Extract bottom value (g)
            var fourDisplay = (from signal in display.Signals
                               where signal.Length == 4
                               select signal).First();

            var mask = fourDisplay.Signals();
            mask.Add(actualToCode['a']);

            var nineDisplay = (from signal in display.Signals
                               where signal.Length == 6 && signal.Signals().IsProperSupersetOf(mask)
                               select signal).First();

            WriteToDictionary(nineDisplay.Signals(), mask, 'g', ref codeToActual, ref actualToCode);

            ///////////////////////////
            // Extract middle value (d)
            mask = oneDisplay.Signals();
            mask.Add(actualToCode['a']);
            mask.Add(actualToCode['g']);

            var threeDisplay = (from signal in display.Signals
                                where signal.Length == 5 && signal.Signals().IsProperSupersetOf(mask)
                                select signal).First();

            WriteToDictionary(threeDisplay.Signals(), mask, 'd', ref codeToActual, ref actualToCode);

            /////////////////////////////
            // Extract top-left value (b)
            mask = oneDisplay.Signals();
            mask.Add(actualToCode['d']);

            WriteToDictionary(fourDisplay.Signals(), mask, 'b', ref codeToActual, ref actualToCode);

            ////////////////////////////////
            // Extract bottom-left value (e)
            var eightDisplay = (from signal in display.Signals
                                where signal.Length == 7
                                select signal).First();

            WriteToDictionary(eightDisplay.Signals(), nineDisplay.Signals(), 'e', ref codeToActual, ref actualToCode);

            /////////////////////////////////
            // Extract bottom-right value (f)
            mask = new HashSet<char>()
            {
                actualToCode['a'],
                actualToCode['g'],
                actualToCode['d'],
                actualToCode['b'],
                actualToCode['e']
            };

            var sixDisplay = (from signal in display.Signals
                              where signal.Length == 6 && signal.Signals().IsProperSupersetOf(mask)
                              select signal).First();

            WriteToDictionary(sixDisplay.Signals(), mask, 'f', ref codeToActual, ref actualToCode);

            //////////////////////////////
            // Extract top-right value (c)
            WriteToDictionary(oneDisplay.Signals(), new HashSet<char>() { actualToCode['f'] }, 'c', ref codeToActual, ref actualToCode);

            return codeToActual;
        }

        private static void WriteToDictionary(
            in HashSet<char> source,
            in HashSet<char> mask,
            char actualCharacter,
            ref Dictionary<char, char> codeToActual,
            ref Dictionary<char, char> actualToCode)
        {
            char encodedValue = source.Except(mask).First();
            codeToActual.Add(encodedValue, actualCharacter);
            actualToCode.Add(actualCharacter, encodedValue);

            // DisplayDictionary(codeToActual);
        }

        private record Display(List<string> Signals, OutputValue Values);
        private record OutputValue(List<string> Digits);
    }

    public static class ExtensionMethods
    {
        public static HashSet<char> Signals(this string aString)
        {
            return aString.Distinct().ToHashSet();
        }

        public static string Sort(this string input)
        {
            char[] characters = input.ToArray();
            Array.Sort(characters);
            return new string(characters);
        }
    }
}