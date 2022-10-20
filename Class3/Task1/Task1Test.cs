using NUnit.Framework;
using NUnit.Framework.Internal;
using static NUnit.Framework.Assert;
using static Task1.Task1;

using Deck = System.Collections.Generic.List<Card>;
// Набор карт у игрока
using Hand = System.Collections.Generic.List<Card>;
// Набор карт, выложенных на стол
using Table = System.Collections.Generic.List<Card>;

namespace Task1;

public class Tests
{
    [Test]
    public void RoundWinnerTest()
    {
        That(RoundWinner(new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Diamonds, Rank.Ten)), Is.EqualTo(Player.First));
        That(RoundWinner(new Card(Suit.Spades, Rank.Eight), new Card(Suit.Spades, Rank.Nine)), Is.EqualTo(Player.Second));
        That(RoundWinner(new Card(Suit.Hearts, Rank.Six), new Card(Suit.Diamonds, Rank.Six)), Is.EqualTo(null));
    }

    [Test]
    public void FullDeckTest()
    {
        var deck = FullDeck();
        That(deck, Has.Count.EqualTo(DeckSize));
        
        var suits = Enum.GetValues(typeof(Suit));
        That(suits, Has.Length.EqualTo(4));
        
        foreach (Suit suit in suits)
        {
            That(deck.Count(card => card.Suit == suit), Is.EqualTo(DeckSize / suits.Length));
        }
        
        var ranks = Enum.GetValues(typeof(Rank));
        That(ranks, Has.Length.EqualTo(DeckSize / 4));
        
        foreach (Rank rank in ranks)
        {
            That(deck.Count(card => card.Rank == rank), Is.EqualTo(DeckSize / ranks.Length));
        }
    }

    [Test]
    public void RoundTest()
    {
        var handsFirst1 = new Dictionary<Player, Hand>
        {
            [Player.First] = new Hand
            {
                new Card(Suit.Hearts, Rank.Ten)
            },
            [Player.Second] = new Hand
            {
                new Card(Suit.Diamonds, Rank.Eight)
            }
        };
        var handsFirst2 = new Dictionary<Player, Hand>
        {
            [Player.First] = new Hand
            {
                new Card(Suit.Clubs, Rank.Ace),
                new Card(Suit.Hearts, Rank.Ten)
            },
            [Player.Second] = new Hand
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Diamonds, Rank.Eight)
            }
        };
        var handsSecond1 = new Dictionary<Player, Hand>
        {
            [Player.First] = new Hand
            {
                new Card(Suit.Hearts, Rank.Eight)
            },
            [Player.Second] = new Hand
            {
                new Card(Suit.Diamonds, Rank.Ten)
            }
        };
        var handsSecond2 = new Dictionary<Player, Hand>
        {
            [Player.First] = new Hand
            {
                new Card(Suit.Clubs, Rank.Ace),
                new Card(Suit.Hearts, Rank.Eight)
            },
            [Player.Second] = new Hand
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Diamonds, Rank.Ten)
            }
        };
        var handsNone1 = new Dictionary<Player, Hand>
        {
            [Player.First] = new Hand
            {
                new Card(Suit.Clubs, Rank.Ace),
            },
            [Player.Second] = new Hand
            {
                new Card(Suit.Hearts, Rank.Ace),
            }
        };
        var handsNone2 = new Dictionary<Player, Hand>
        {
            [Player.First] = new Hand
            {
                new Card(Suit.Clubs, Rank.Ace),
                new Card(Suit.Diamonds, Rank.Queen)
            },
            [Player.Second] = new Hand
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Hearts, Rank.Queen)
            }
        };
        var answerFirst1 = new Table
        {
            new Card(Suit.Hearts, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Eight),
        };
        var answerFirst2 = new Table
        {
            new Card(Suit.Clubs, Rank.Ace),
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Hearts, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Eight),
        };
        var answerSecond1 = new Table
        {
            new Card(Suit.Hearts, Rank.Eight),
            new Card(Suit.Diamonds, Rank.Ten),
        };
        var answerSecond2 = new Table
        {
            new Card(Suit.Clubs, Rank.Ace),
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Hearts, Rank.Eight),
            new Card(Suit.Diamonds, Rank.Ten),
        };
        var answerNone1 = new Table
        {
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Hearts, Rank.Ace),
        };
        var answerNone2 = new Table
        {
            new(Suit.Clubs, Rank.Ace),
            new(Suit.Hearts, Rank.Ace),
            new(Suit.Diamonds, Rank.Queen),
            new(Suit.Hearts, Rank.Queen),
        };
        That(Round(handsFirst1), Is.EqualTo(new Tuple<Player, Table>(Player.First, answerFirst1)));
        That(Round(handsFirst2), Is.EqualTo(new Tuple<Player, Table>(Player.First, answerFirst2)));
        That(Round(handsSecond1), Is.EqualTo(new Tuple<Player, Table>(Player.Second, answerSecond1)));
        That(Round(handsSecond2), Is.EqualTo(new Tuple<Player, Table>(Player.Second, answerSecond2)));
        var exNone1 = Throws<NoCardsException>(() => Round(handsNone1))!;
        That(exNone1.Table, Is.EqualTo(answerNone1));
        var exNone2 = Throws<NoCardsException>(() => Round(handsNone2))!;
        That(exNone2.Table, Is.EqualTo(answerNone2));
    }

    [Test]
    public void Game2CardsTest()
    {
        var six = new Card(Suit.Clubs, Rank.Six);
        var queen1 = new Card(Suit.Spades, Rank.Queen);
        var queen2 = new Card(Suit.Hearts, Rank.Queen);
        var ace = new Card(Suit.Diamonds, Rank.Ace);
        
        Dictionary<Player, List<Card>> handsFirst = new Dictionary<Player, List<Card>>
        {
            [Player.First] = new List<Card> {ace},
            [Player.Second] = new List<Card> {six},
        };
        var gameWinnerFirst = Game(handsFirst);
        That(gameWinnerFirst, Is.EqualTo(Player.First));
        
        Dictionary<Player, List<Card>> handsSecond = new Dictionary<Player, List<Card>>
        {
            [Player.First] = new List<Card> {queen1, six},
            [Player.Second] = new List<Card> {queen2, ace}
        };
        var gameWinnerSecond = Game(handsSecond);
        That(gameWinnerSecond, Is.EqualTo(Player.Second));
        
        Dictionary<Player, List<Card>> handsNone = new Dictionary<Player, List<Card>>
        {
            [Player.First] = new List<Card> {queen1},
            [Player.Second] = new List<Card> {queen2}
        };
        var gameWinnerNone = Game(handsNone);
        That(gameWinnerNone, Is.EqualTo(null));
    }
}