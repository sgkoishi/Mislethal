using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using System.Collections.Generic;
using System.Linq;

namespace Chireiden.Mislethal
{
    public abstract class Combo
    {
        public abstract string Name { get; }
        public abstract double Match();
        public abstract ComboAction Damage();
    }
}
