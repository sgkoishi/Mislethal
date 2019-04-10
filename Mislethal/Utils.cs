using Hearthstone_Deck_Tracker.Hearthstone;
using System.Collections.Generic;
using System.Linq;

namespace Chireiden.Mislethal
{
    public static class Utils
    {
        public static Card First(this List<Card> cards, string target)
        {
            return cards.First(c => c.Id == target);
        }

        public static List<Card> Of(this List<Card> cards, string target)
        {
            return cards.Where(c => c.Id == target).ToList();
        }

        public static bool Contains(this List<Card> cards, string target)
        {
            return cards.Any(c => c.Id == target);
        }

        public static int Count(this List<Card> source, string target)
        {
            return source.Count(c => c.Id == target);
        }

        public static int ToMill(this int draws, int remain)
        {
            return draws > remain ? (draws - remain) * (draws - remain + 1) / 2 : 0;
        }

        public static int[] ToMill(this int[] draws, int remain)
        {
            return draws.Select(d => d.ToMill(remain)).ToArray();
        }
    }
}