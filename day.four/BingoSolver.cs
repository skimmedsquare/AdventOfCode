using System.Text;

namespace Aoc2021.DayFour {
    class BingoSolver : ISolver
    {
        public void SolvePartOne()
        {
            var numbers = new List<int>();
            var cards = new List<BingoCard>();
            ReadFile("day.four/input.txt", out numbers, out cards);
            var winningNumber = 0;
            BingoCard winner = FindWinner(numbers, cards, out winningNumber);
            Console.WriteLine($"Solution: {UnmarkedSum(winner) * winningNumber}");
        }

        public void SolvePartTwo()
        {
            var numbers = new List<int>();
            var cards = new List<BingoCard>();
            ReadFile("day.four/example.txt", out numbers, out cards);
            var winningNumber = 0;
            BingoCard loser = FindLoser(numbers, cards, out winningNumber);
            Console.WriteLine($"Solution: {UnmarkedSum(loser) * winningNumber}");
        }

        int UnmarkedSum(in BingoCard winner) {
            int sum = 0;
            for (var i = 0; i < BingoCard.Dims; i++) {
                for (var j = 0; j < BingoCard.Dims; j++) {
                    var entry = winner.Board[i,j];
                    if (!entry.Marked) {
                        sum += entry.Value;
                    }
                }
            }
            return sum;
        }

        BingoCard FindWinner(in List<int> numbers, in List<BingoCard> cards, out int winningNumber) {
            Console.WriteLine($"Total Numbers: {numbers.Count()}, Total Cards: {cards.Count()}");

            foreach (var num in numbers) {
                Console.WriteLine($"Marking {num}!");
                for (var i = 0; i < cards.Count(); i++ ) {
                    var card = cards[i];
                    MarkCard(ref card, num);
                    cards[i] = card;
                    Console.WriteLine(card);
                }

                var found = cards.FirstOrDefault(c => IsSolved(c));
                if (found.Populated) {
                    winningNumber = num;
                    return found;
                }
            }

            // Should never happen...
            winningNumber = -1;
            return new BingoCard();
        }
        BingoCard FindLoser(in List<int> numbers, in List<BingoCard> cards, out int winningNumber) {
            Console.WriteLine($"Total Numbers: {numbers.Count()}, Total Cards: {cards.Count()}");
            var cardsCopy = new List<BingoCard>(cards);

            foreach (var num in numbers) {
                Console.WriteLine($"Marking {num}!");
                for (var i = 0; i < cardsCopy.Count(); i++ ) {
                    var card = cardsCopy[i];
                    MarkCard(ref card, num);
                    cardsCopy[i] = card;
                }

                var solved = cardsCopy.FindAll(c => IsSolved(c));
                if (solved.Count() > 0) {
                    winningNumber = num;

                    if (cardsCopy.Count() == 1) {
                        return cardsCopy[0];
                    }

                    cardsCopy.RemoveAll(c => IsSolved(c));
                }
            }

            // Should never happen...
            winningNumber = -1;
            return new BingoCard();
        }

        void ReadFile(string fileName, out List<int> numbers, out List<BingoCard> cards) {
            var lines = File.ReadLines(fileName).ToArray();
            numbers = lines[0].Split(',').Select(v => int.Parse(v)).ToList();

            var readCards = new List<BingoCard>();
            var startIdx = 2;
            while (true) {
                var card = ReadCard(lines, startIdx, startIdx + BingoCard.Dims);
                readCards.Add(card);

                startIdx += BingoCard.Dims + 1;

                if (startIdx >= lines.Count()) {
                    break;
                }
            }
            Console.WriteLine(readCards.Count());
            cards = readCards;
        }

        BingoCard ReadCard(string[] lines, int startIdx, int endIdx) {
            var boardLines = lines[startIdx..endIdx];
            var i = 0;

            BingoCard bingo = CreateBingoBoard();

            foreach(var line in boardLines) {
                var j = 0;
                var nums = line.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(v => int.Parse(v)).ToList();

                foreach (var num in nums) {
                    bingo.Board[i, j].Value = num;
                    j++;
                }
                i++;
            }

            return bingo;
        }

        BingoCard CreateBingoBoard() {
            return new BingoCard() {
                RowsStatus = new int[BingoCard.Dims],
                ColsStatus = new int[BingoCard.Dims],
                Board = new Entry[BingoCard.Dims, BingoCard.Dims],
                Populated = true,
                Solved = false
            };
        }

        bool IsSolved(BingoCard card) {
            return card.ColsStatus.Any(v => v == BingoCard.Dims) || card.RowsStatus.Any(v => v == BingoCard.Dims);
        }

        void MarkCard(ref BingoCard card, int num) {
            for (var i = 0; i < BingoCard.Dims; i++) {
                for (var j = 0; j < BingoCard.Dims; j++) {
                    if (card.Board[i,j].Value == num) {
                        card.Board[i,j].Marked = true;

                        card.RowsStatus[i]++;
                        card.ColsStatus[j]++;
                    }
                }
            }
        }
    }

    struct BingoCard {
        public const int Dims = 5;

        public bool Populated;

        public bool Solved;

        public int[] RowsStatus;

        public int[] ColsStatus;

        public Entry[,] Board;

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append($"Row Status: {string.Join(", ", RowsStatus)}\n");
            output.Append($"Col Status: {string.Join(", ", ColsStatus)}\n\n");
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

    struct Entry {
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