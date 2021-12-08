using System.Text;

namespace Aoc2021.Day4
{
    public class BingoSolver : ISolver
    {
        public void SolvePartOne(in List<string> data)
        {
            ReadFile(data, out var numbers, out var cards);

            BingoCard winner = FindWinner(numbers, cards, out var winningNumber);
            Console.WriteLine($"Solution: {winner.UnmarkedSum() * winningNumber}");
        }

        public void SolvePartTwo(in List<string> data)
        {
            ReadFile(data, out var numbers, out var cards);
            BingoCard loser = FindLoser(numbers, cards, out var winningNumber);
            Console.WriteLine($"Solution: {loser.UnmarkedSum() * winningNumber}");
        }

        private static BingoCard FindWinner(in List<int> numbers, in List<BingoCard> cards, out int winningNumber)
        {
            Console.WriteLine($"Total Numbers: {numbers.Count}, Total Cards: {cards.Count}");

            foreach (int num in numbers)
            {
                Console.WriteLine($"Marking {num}!");
                foreach (BingoCard card in cards)
                {
                    card.Mark(num);
                }

                BingoCard? found = cards.Find(c => c.IsSolved());
                if (found != null)
                {
                    winningNumber = num;
                    return found;
                }
            }

            throw new InvalidDataException();
        }

        private static BingoCard FindLoser(in List<int> numbers, in List<BingoCard> cards, out int winningNumber)
        {
            Console.WriteLine($"Total Numbers: {numbers.Count}, Total Cards: {cards.Count}");
            var cardsCopy = new List<BingoCard>(cards);

            foreach (int num in numbers)
            {
                Console.WriteLine($"Marking {num}!");
                for (var i = 0; i < cardsCopy.Count; i++)
                {
                    cardsCopy[i].Mark(num);
                }

                List<BingoCard> solved = cardsCopy.FindAll(c => c.IsSolved());
                if (solved.Count > 0)
                {
                    winningNumber = num;

                    if (cardsCopy.Count == 1)
                    {
                        return cardsCopy[0];
                    }

                    cardsCopy.RemoveAll(c => c.IsSolved());
                }
            }

            // Should never happen...
            throw new InvalidDataException();
        }

        private static void ReadFile(in List<string> data, out List<int> numbers, out List<BingoCard> cards)
        {
            string[] lines = data.ToArray();
            numbers = lines[0].Split(',').Select(v => int.Parse(v)).ToList();

            var readCards = new List<BingoCard>();
            var startIdx = 2;
            do
            {
                int endIdx = startIdx + BingoCard.Dims;
                readCards.Add(new(lines[startIdx..endIdx]));
                startIdx += BingoCard.Dims + 1;
            }
            while (startIdx < lines.Length);
            Console.WriteLine(readCards.Count);
            cards = readCards;
        }
    }

    internal class BingoCard
    {
        public const int Dims = 5;

        private readonly int[] rowsStatus;

        private readonly int[] colsStatus;

        private readonly Entry[,] board;

        public BingoCard(string[] boardLines)
        {
            rowsStatus = new int[Dims];
            colsStatus = new int[Dims];
            board = new Entry[Dims, Dims];

            var i = 0;
            foreach (string line in boardLines)
            {
                var j = 0;
                foreach (int num in line.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(v => int.Parse(v)).ToList())
                {
                    board[i, j].Value = num;
                    j++;
                }

                i++;
            }
        }

        public int UnmarkedSum()
        {
            var sum = 0;
            for (var i = 0; i < Dims; i++)
            {
                for (var j = 0; j < Dims; j++)
                {
                    Entry entry = board[i, j];
                    if (!entry.Marked)
                    {
                        sum += entry.Value;
                    }
                }
            }

            return sum;
        }

        public bool IsSolved()
        {
            return colsStatus.Any(v => v == Dims) || rowsStatus.Any(v => v == Dims);
        }

        public void Mark(int num)
        {
            for (var i = 0; i < Dims; i++)
            {
                for (var j = 0; j < Dims; j++)
                {
                    if (board[i, j].Value == num)
                    {
                        board[i, j].Marked = true;

                        rowsStatus[i]++;
                        colsStatus[j]++;
                    }
                }
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("Row Status: ").AppendJoin(", ", rowsStatus).Append('\n')
                .Append("Col Status: ").AppendJoin(", ", colsStatus).Append("\n\n");
            for (var i = 0; i < Dims; i++)
            {
                for (var j = 0; j < Dims; j++)
                {
                    output.Append(board[i, j])
                        .Append(' ');
                }

                output.Append('\n');
            }

            return output.ToString();
        }
    }

    internal struct Entry
    {
        public int Value;
        public bool Marked;

        public override string ToString()
        {
            return Marked ? $"*{Value}*" : Value.ToString();
        }
    }
}