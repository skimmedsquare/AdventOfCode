namespace Aoc2021 {
    namespace DayOne {
        class DepthSolver {
            public static int SolvePartOne() {
                List<int> depths = ReadInput();
                return NumIncreasing(depths);
            }    

            public static int SolvePartTwo() {
                List<int> depths = ReadInput();

                List<int> windows = new List<int>(depths.Count - 3);
                var currWindow = 0;
                for (var i = 0; i < depths.Count - 2; i++) {
                    if (i == 0) { 
                        currWindow = depths[i] + depths[i+1] + depths[i+2];
                        windows.Add(currWindow);
                    } else {
                        currWindow -= depths[i-1];
                        currWindow += depths[i+2];
                        windows.Add(currWindow);
                    }
                }

                return NumIncreasing(windows);
            }

            static List<int> ReadInput() {
                return System.IO.File.ReadLines("day.one/depths.txt")
                    .Select(line => int.Parse(line))
                    .ToList();
            }

            static int NumIncreasing(in List<int> vals ) {
                var count = 0;
                for (var i = 0; i < vals.Count; i++) {
                    if (i == 0) {
                        continue;
                    }
                    if (vals[i] > vals[i-1]) {
                        count++;
                    }
                }

                return count;
            }
        }
    }
}