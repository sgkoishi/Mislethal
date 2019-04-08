using HearthDb.Enums;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Chireiden.Mislethal
{
    public class Main : IPlugin
    {
        public string Name => "Mislethal";

        public string Description => "Mislethal is a plugin for Combo decks.";

        public string ButtonText => "Open / Close Log Window";

        public string Author => "SGKoishi";

        public Version Version => new Version(1, 0, 0, 0);

        public System.Windows.Controls.MenuItem MenuItem => null;

        private readonly List<Combo> Combos = new List<Combo>();
        private Combo Current;

        public void OnButtonPress()
        {
            Utils.ToggleConsole();
        }

        public void OnLoad()
        {
            this.Combos.Add(new VargothThreeTurnKillMage());
            GameEvents.OnGameStart.Add(this.OnGameStart);
            GameEvents.OnPlayerDraw.Add(this.OnPlayerDraw);
            Utils.ToggleConsole();
        }

        private void OnGameStart()
        {
            var deck = Core.Game.Player.PlayerCardList.Select(c => c.Id).ToArray();
            var match = 0.0;
            foreach (var combo in this.Combos)
            {
                var cm = combo.Match(deck);
                if (cm > match)
                {
                    match = cm;
                    this.Current = combo;
                }
            }
            Console.WriteLine($"Deck detection: {this.Current.Name} ({match})");
        }

        private void OnPlayerDraw(Card obj)
        {
            if (this.Current == null)
            {
                return;
            }
            var hand = Core.Game.Player.Hand.Select(c => c.CardId).ToArray();
            var damage = this.Current.Damage(hand);
            var opponent = Core.Game.Opponent.Board.First(e => e.IsHero);
            var health = opponent.Health + opponent.GetTag(GameTag.ARMOR);
            if (damage > health)
            {
                Console.WriteLine($"Lethal found by {this.Current.Name}: Damage {damage} > Opponent Health {health}.");
            }
        }

        public void OnUnload()
        {
        }

        public void OnUpdate()
        {
            // Render something here?
        }
    }
}