using System.Text;

namespace Aoc2021.Day3 {
    class BinarySolver : ISolver
    {
        static string[] TEST = {
            "00100",
            "11110",
            "10110",
            "10111",
            "10101",
            "01111",
            "00111",
            "11100",
            "10000",
            "11001",
            "00010",
            "01010"
        };

        public void SolvePartOne()
        {
            List<string> data = ReadFile();
            List<uint> nums = data.Select(bin_string => Convert.ToUInt32(bin_string, 2))
                .ToList();

            var digits = data[0].Count();
            var builder = new StringBuilder();

            for (var bits = digits; bits > 0; bits--) {
                var sum = 0L;
                for (var i = 0; i < nums.Count() ; i++ ) {
                    var mask = (1 << bits) - 1;
                    // Console.WriteLine($"Mask     : {Convert.ToString(mask, toBase:2)}");

                    var extracted = mask & nums[i];
                    // Console.WriteLine($"Extracted: {Convert.ToString(mask & nums[i], toBase:2)}");

                    var digit = extracted >> (bits - 1);
                    // Console.WriteLine($"Digit    : {Convert.ToString(digit, toBase: 2)}");
                    // Console.WriteLine();

                    sum += digit;
                }

                // This only works because an odd # is guaranteed by problem constraints... See below for 
                // the correct way of not being hit by truncation bugs.
                if (sum > nums.Count() / 2) {
                    builder.Append(1);
                } else {
                    builder.Append(0);
                }
            }

            var gamma_str = builder.ToString();
            Console.WriteLine($"Gamma  : {gamma_str}");
            
            var epsilon_str = new string(builder.ToString().Select(c => c == '1' ? '0' : '1').ToArray());
            Console.WriteLine($"Epislon: {epsilon_str}");

            var gamma = Convert.ToUInt32(gamma_str, 2);
            var epsilon = Convert.ToUInt32(epsilon_str, 2);

            Console.WriteLine(gamma * epsilon);
        }

        static List<String> ReadFile()
        {
            return System.IO.File.ReadLines("day.three/data.txt").ToList();
        }

        public void SolvePartTwo()
        {
            List<string> data = ReadFile();
            List<uint> nums = data.Select(bin_string => Convert.ToUInt32(bin_string, 2))
                .ToList();

            Console.WriteLine(FindRating(data[0].Count(), nums, true) * FindRating(data[0].Count(), nums, false));
        }

        uint FindRating(int digits, in List<uint> nums, bool useMajority) {
            var possibleVals = nums;
            for (var bits = digits; bits > 0; bits--) {
                var numOnes = 0L;
                var ones = new List<uint>();
                var zeroes = new List<uint>();

                if (possibleVals.Count() == 1) {
                    break;
                }

                for (var i = 0; i < possibleVals.Count() ; i++ ) {
                    var mask = (1 << bits) - 1;
                    // Console.WriteLine($"Mask     : {Convert.ToString(mask, toBase:2)}");

                    var extracted = mask & possibleVals[i];
                    // Console.WriteLine($"Extracted: {Convert.ToString(mask & nums[i], toBase:2)}");

                    var digit = extracted >> (bits - 1);
                    // Console.WriteLine($"Digit    : {Convert.ToString(digit, toBase: 2)}");
                    // Console.WriteLine();

                    if (digit == 1) {
                        ones.Add(possibleVals[i]);
                    } else {
                        zeroes.Add(possibleVals[i]);
                    }

                    numOnes += digit;
                }

                var numZeroes = possibleVals.Count() - numOnes;

                if (numOnes > numZeroes) {
                    if (useMajority) {
                        possibleVals = ones;
                    } else {
                        possibleVals = zeroes;
                    }
                } else if (numOnes < numZeroes) {
                    if (useMajority) {
                        possibleVals = zeroes;
                    } else {
                        possibleVals = ones;
                    }
                } else {
                    if (useMajority) {
                        possibleVals = ones;
                    } else {
                        possibleVals = zeroes;
                    }
                }
            }
            return possibleVals[0];
        }
    }
}