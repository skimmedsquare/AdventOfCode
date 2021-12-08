using System.Text;

namespace Aoc2021.Day4 {
    public class BingoSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            ReadFile(data, out List<int> numbers, out List<BingoCard> cards);

            BingoCard winner = FindWinner(numbers, cards, out int winningNumber);
            Console.WriteLine($"Solution: {winner.UnmarkedSum() * winningNumber}");
        }

        public void SolvePartTwo(in List<string> data)
        {
            ReadFile(data, out List<int> numbers, out List<BingoCard> cards);
            BingoCard loser = FindLoser(numbers, cards, out int winningNumber);
            Console.WriteLine($"Solution: {loser.UnmarkedSum() * winningNumber}");
        }

        private static BingoCard FindWinner(in List<int> numbers, in List<BingoCard> cards, out int winningNumber) {
            Console.WriteLine($"Total Numbers: {numbers.Count}, Total Cards: {cards.Count}");

            foreach (int num in numbers) {
                Console.WriteLine($"Marking {num}!");
                foreach (BingoCard card in cards) {
                    card.Mark(num);
                }

                var found = cards.Find(c => c.IsSolved());
                if (found?.Populated == true) {
                    winningNumber = num;
                    return found;
                }
            }

            throw new InvalidDataException();
        }
        private static BingoCard FindLoser(in List<int> numbers, in List<BingoCard> cards, out int winningNumber) {
            Console.WriteLine($"Total Numbers: {numbers.Count}, Total Cards: {cards.Count}");
            var cardsCopy = new List<BingoCard>(cards);

            foreach (int num in numbers) {
                Console.WriteLine($"Marking {num}!");
                for (var i = 0; i < cardsCopy.Count; i++ ) {
                    cardsCopy[i].Mark(num);
                }

                var solved = cardsCopy.FindAll(c => c.IsSolved());
                if (solved.Count > 0) {
                    winningNumber = num;

                    if (cardsCopy.Count == 1) {
                        return cardsCopy[0];
                    }

                    cardsCopy.RemoveAll(c => c.IsSolved());
                }
            }

            // Should never happen...
            throw new InvalidDataException();
        }

        private static void ReadFile(in List<string> data, out List<int> numbers, out List<BingoCard> cards) {
            var lines = data.ToArray();
            numbers = lines[0].Split(',').Select(v => int.Parse(v)).ToList();

            var readCards = new List<BingoCard>();
            var startIdx = 2;
            do
            {
                readCards.Add(new(lines[startIdx..(startIdx + BingoCard.Dims)]));
                startIdx += BingoCard.Dims + 1;
            }
            while (startIdx < lines.Length);
            Console.WriteLine(readCards.Count);
            cards = readCards;
        }
    }

    internal class BingoCard {
        public const int Dims = 5;

        public bool Populated;

        public bool Solved;

        public int[] RowsStatus;

        public int[] ColsStatus;

        public Entry[,] Board;

        public BingoCard(string[] boardLines) {
            RowsStatus = new int[Dims];
            ColsStatus = new int[Dims];
            Board = new Entry[Dims, Dims];
            Populated = true;
            Solved = false;

            var i = 0;
            foreach(string line in boardLines) {
                var j = 0;
                foreach (int num in line.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(v => int.Parse(v)).ToList()) {
                    Board[i, j].Value = num;
                    j++;
                }
                i++;
            }
        }

        public int UnmarkedSum()
        {
            int sum = 0;
            for (var i = 0; i < Dims; i++) {
                for (var j = 0; j < Dims; j++) {
                    var entry = this.Board[i,j];
                    if (!entry.Marked) {
                        sum += entry.Value;
                    }
                }
            }
            return sum;
        }

        public bool IsSolved() {
            return ColsStatus.Any(v => v == Dims) || RowsStatus.Any(v => v == Dims);
        }

        public void Mark(int num) {
            for (var i = 0; i < Dims; i++) {
                for (var j = 0; j < Dims; j++) {
                    if (Board[i,j].Value == num) {
                        Board[i,j].Marked = true;

                        RowsStatus[i]++;
                        ColsStatus[j]++;
                    }
                }
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("Row Status: ").AppendJoin(", ", RowsStatus).Append('\n')
                .Append("Col Status: ").AppendJoin(", ", ColsStatus).Append("\n\n");
            for (var i = 0; i < Dims; i++) {
                for (var j = 0; j < Dims; j++) {
                    output.Append(Board[i,j]);
                    output.Append(' ');
                }
                output.Append('\n');
            }
            return output.ToString();
        }
    }

    internal struct Entry {
        public int Value;
        public bool Marked;

        public override string ToString()
        {
            if (Marked) {
                return $"*{Value}*";
            }
            return Value.ToString();
        }
    }
}