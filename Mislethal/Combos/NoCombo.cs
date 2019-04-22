using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using MageCards = HearthDb.CardIds.Collectible.Mage;
using NeutralCards = HearthDb.CardIds.Collectible.Neutral;

namespace Chireiden.Mislethal.Combos
{
    public class NoCombo : Combo
    {
        public override string Name => "No Match";

        public override ComboAction Damage()
        {
            return new ComboAction(0) { Message = "No matching deck." };
        }

        // No idea how to match.
        public override double Match()
        {
            return 0.5;
        }
    }
}
