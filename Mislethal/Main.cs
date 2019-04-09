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

        public Version Version => new Version(0, 1, 1, 0);

        public System.Windows.Controls.MenuItem MenuItem => null;

        public readonly List<Combo> Combos = new List<Combo>();
        public Combo Current;
        public int Lethal;
        private string _message;

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
            this.Lethal = -1;
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
            if (Core.Game.IsInMenu)
            {
                return;
            }
            var opponentHero = Core.Game.Opponent.Board.Where(e => e.IsHero);
            if (!opponentHero.Any())
            {
                return;
            }
            if (damage.Lethal > this.Lethal)
            {
                var opponent = opponentHero.First();
                var health = opponent.Health + opponent.GetTag(GameTag.ARMOR);
                var message = $"Current Deck: {this.Current.Name}\r\n";
                if (damage.Lethal > health && this.Lethal != damage.Lethal && damage.Lethal > 0 && health > 0)
                {
                    this.Lethal = damage.Lethal;
                    message = @$"Lethal found by {this.Current.Name}: Damage {damage.Lethal} > Opponent Health {health}.
Action:
    {damage.ToString("\r\n    ")}
";
                }
                message += @$"Message:
    {damage.Message}
";
                if (this._message != message)
                {
                    Console.Clear();
                    this._message = message;
                    Console.WriteLine(this._message);
                }
            }
        }
    }
}