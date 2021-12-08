using System.Text;

namespace Aoc2021.Day3 {
    public class BinarySolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            List<uint> nums = data.ConvertAll(bin_string => Convert.ToUInt32(bin_string, 2));
            int digits = data[0].Length;
            var builder = new StringBuilder();

            for (var bits = digits; bits > 0; bits--) {
                var sum = 0L;
                for (var i = 0; i < nums.Count ; i++ ) {
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
                if (sum > nums.Count / 2) {
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

        public void SolvePartTwo(in List<string> data)
        {
            List<uint> nums = data.ConvertAll(bin_string => Convert.ToUInt32(bin_string, 2))
;

            Console.WriteLine(FindRating(data[0].Length, nums, true) * FindRating(data[0].Length, nums, false));
        }

        private static uint FindRating(int digits, in List<uint> nums, bool useMajority) {
            var possibleVals = nums;
            for (var bits = digits; bits > 0; bits--) {
                var numOnes = 0L;
                var ones = new List<uint>();
                var zeroes = new List<uint>();

                if (possibleVals.Count == 1) {
                    break;
                }

                for (var i = 0; i < possibleVals.Count; i++ ) {
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

                var numZeroes = possibleVals.Count - numOnes;

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