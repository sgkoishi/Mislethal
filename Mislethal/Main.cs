using HearthDb.Enums;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chireiden.Mislethal
{
    public class Main : IPlugin
    {
        public static PlayerHelper Self = new PlayerHelper(true);
        public static PlayerHelper Oppo = new PlayerHelper(false);
        public string Name => "Mislethal";

        public string Description => "Mislethal is a plugin for Combo decks.";

        public string ButtonText => "Open / Close Log Window";

        public string Author => "SGKoishi";

        public Version Version => new Version(1, 0, 0, 0);

        public System.Windows.Controls.MenuItem MenuItem => null;

        public readonly List<Combo> Combos = new List<Combo>();
        public Combo Current;
        public int Lethal;

        public void OnButtonPress()
        {
            Utils.ToggleConsole();
        }

        public void OnLoad()
        {
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (item.IsSubclassOf(typeof(Combo)))
                {
                    this.Combos.Add((Combo) Activator.CreateInstance(item));
                }
            }
            GameEvents.OnGameStart.Add(this.OnGameStart);
            Utils.ToggleConsole();
        }

        private void OnGameStart()
        {
            var match = 0.0;
            foreach (var combo in this.Combos)
            {
                var cm = combo.Match();
                if (cm > match)
                {
                    match = cm;
                    this.Current = combo;
                }
            }
            this.Lethal = 0;
            Console.WriteLine($"Deck detection: {this.Current?.Name ?? "Nothing"} ({match})");
        }

        public void OnUnload()
        {
        }

        public void OnUpdate()
        {
            if (this.Current == null)
            {
                return;
            }
            var damage = this.Current.Damage();
            var opponent = Core.Game.Opponent.Board.First(e => e.IsHero);
            var health = opponent.Health + opponent.GetTag(GameTag.ARMOR);
            if (damage.Lethal > health && this.Lethal != damage.Lethal)
            {
                this.Lethal = damage.Lethal;
                Console.Clear();
                Console.WriteLine($"Lethal found by {this.Current.Name}: Damage {damage.Lethal} > Opponent Health {health}.");
                Console.WriteLine($"Action:\r\n    {damage.ToString("\r\n    ")}");
                Console.WriteLine($"Message:\r\n    {damage.Message}");
            }
        }
    }
}