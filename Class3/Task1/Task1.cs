// Колода

using System.ComponentModel.DataAnnotations.Schema;
using Deck = System.Collections.Generic.List<Card>;
// Набор карт у игрока
using Hand = System.Collections.Generic.List<Card>;
// Набор карт, выложенных на стол
using Table = System.Collections.Generic.List<Card>;

// Масть
internal enum Suit
{
    Diamonds,
    Clubs,
    Hearts,
    Spades
}

// Значение
internal enum Rank
{
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

// Карта
record Card(Suit Suit, Rank Rank)
{
    public override string ToString()
    {
        return $"{this.Suit} {this.Rank}";
    }
}

// Тип для обозначения игрока (первый, второй)
internal enum Player
{
    First,
    Second
}

internal class NoCardsException : Exception
{
    public Table Table { get; init; }

    public NoCardsException(Table table)
    {
        Table = table;
    }
}


namespace Task1
{
    public class Task1
    {
        /*
        * Реализуйте игру "Пьяница" (в простейшем варианте, на колоде в 36 карт)
        * https://ru.wikipedia.org/wiki/%D0%9F%D1%8C%D1%8F%D0%BD%D0%B8%D1%86%D0%B0_(%D0%BA%D0%B0%D1%80%D1%82%D0%BE%D1%87%D0%BD%D0%B0%D1%8F_%D0%B8%D0%B3%D1%80%D0%B0)
        * Рука — это набор карт игрока. Карты выкладываются на стол из начала "рук" и сравниваются
        * только по значениям (масть игнорируется). При равных значениях сравниваются следующие карты.
        * Набор карт со стола перекладывается в конец руки победителя. Шестерка туза не бьёт.
        *
        * Реализация должна сопровождаться тестами.
        */

        // Размер колоды
        internal const int DeckSize = 36;
        private static readonly Random Rng = new Random();

        // Возвращается null, если значения карт совпадают
        internal static Player? RoundWinner(Card card1, Card card2)
        {
            if (card1.Rank > card2.Rank)
                return Player.First;
            if (card1.Rank < card2.Rank)
                return Player.Second;
            return null;
        }

// Возвращает полную колоду (36 карт) в фиксированном порядке
        internal static Deck FullDeck()
        {
            var deck = new Deck();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank));
                }
            }

            return deck;
        }

// Раздача карт: случайное перемешивание (shuffle) и деление колоды пополам
        internal static Dictionary<Player, Hand> Deal(Deck deck)
        {
            var shuffledDeck = deck.OrderBy(_ => Rng.Next()).ToList();

            var dict = new Dictionary<Player, Hand>
            {
                [Player.First] = shuffledDeck.GetRange(0, shuffledDeck.Count / 2),
                [Player.Second] = shuffledDeck.GetRange(shuffledDeck.Count / 2 - 1, shuffledDeck.Count / 2)
            };
            return dict;
        }

// Один раунд игры (в том числе спор при равных картах).
// Возвращается победитель раунда и набор карт, выложенных на стол.
        internal static Tuple<Player, Table> Round(Dictionary<Player, Hand> hands)
        {
            if (hands[Player.First].Count == 0 || hands[Player.Second].Count == 0)
                throw new NoCardsException(new Table());

            var card1 = hands[Player.First][0];
            var card2 = hands[Player.Second][0];
            hands[Player.First].RemoveAt(0);
            hands[Player.Second].RemoveAt(0);

            var table = new Table
            {
                card1,
                card2
            };

            var winner = RoundWinner(card1, card2);
            if (winner != null) return new Tuple<Player, Table>((Player)winner!, table);
            try
            {
                var (player, newTable) = Round(hands);
                table.AddRange(newTable);
                return new Tuple<Player, Table>(player, table);
            }
            catch (NoCardsException ex)
            {
                table.AddRange(ex.Table);
                throw new NoCardsException(table);
            }
        }

// Полный цикл игры (возвращается победивший игрок)
// в процессе игры печатаются ходы
        internal static Player? Game(Dictionary<Player, Hand> hands)
        {
            var scores = new Dictionary<Player, int>
            {
                [Player.First] = 0,
                [Player.Second] = 0,
            };
            while (true)
            {
                try
                {
                    var (player, table) = Round(hands);
                    var score = 0;
                    scores.TryGetValue(player, out score);
                    scores[player] = score + table.Count;
                    Console.WriteLine(
                        $"Round ended with the victory of player {player:G} ({table[0]:G} vs {table[1]:G})");
                }
                catch (NoCardsException)
                {
                    break;
                }
            }

            Console.WriteLine(
                $"Score {scores[Player.First]}:{scores[Player.Second]} (player {Player.First} vs player {Player.Second})");
            if (scores[Player.First] > scores[Player.Second])
                return Player.First;
            if (scores[Player.First] < scores[Player.Second])
                return Player.Second;
            return null;
        }

        public static void Main(string[] args)
        {
            var deck = FullDeck();
            var hands = Deal(deck);
            var winner = Game(hands);
            Console.WriteLine($"Winner: player {winner:G}");
        }
    }
}