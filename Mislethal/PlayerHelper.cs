using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using System.Collections.Generic;
using System.Linq;

namespace Chireiden.Mislethal
{
    public class PlayerHelper
    {
        public PlayerHelper(bool friendly)
        {
            this.Friendly = friendly;
        }

        public bool Friendly { get; }
        public Player Player => this.Friendly ? Core.Game.Player : Core.Game.Opponent;
        public List<Card> Hand => this.Player.Hand.Select(c => c.Card).ToList();
        public List<Card> Deck => this.Player.PlayerCardList;
        public List<Card> Board => this.Player.Board.Where(e => !e.IsHero).Select(c => c.Card).ToList();
    }
}
