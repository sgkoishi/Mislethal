using HearthDb.Enums;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chireiden.Mislethal
{
    public class Main : IPlugin
    {
        public static PlayerHelper Self = new PlayerHelper(true);
        public static PlayerHelper Oppo = new PlayerHelper(false);
        public string Name => "Mislethal";

        public string Description => "Mislethal is a plugin for Combo decks.";

        public string ButtonText => "No";

        public string Author => "SGKoishi";

        public Version Version => new Version(0, 1, 2, 0);

        public MenuItem MenuItem => null;

        public readonly List<Combo> Combos = new List<Combo>();
        private readonly Label label = new Label();
        public Combo Current;
        public int Lethal;
        private string _message;

        public void OnButtonPress()
        {
            this.label.Visibility = (this.label.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
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
            GameEvents.OnGameEnd.Add(this.OnGameEnd);
            Core.OverlayCanvas.Children.Add(this.label);
            this.label.FontSize = 16;
            this.label.Content = "";
            this.label.FontWeight = FontWeights.Bold;
            this.label.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
        }

        private void OnGameEnd()
        {
            this.Lethal = -1;
            this._message = "";
            this.label.Content = this._message;
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
            if (Core.Game.IsInMenu)
            {
                return;
            }
            var opponentHero = Core.Game.Opponent.Board.Where(e => e.IsHero);
            if (!opponentHero.Any())
            {
                return;
            }
            var damage = this.Current.Damage();
            if (damage.Lethal < 0 && this.Lethal > 0)
            {
                return;
            }
            var opponent = opponentHero.First();
            var health = opponent.Health + opponent.GetTag(GameTag.ARMOR);
            var message = $"Deck detection: {this.Current?.Name ?? "Nothing"} ({this.Current.Match()})\r\n";
            if (damage.Lethal > health && damage.Lethal > 0 && health > 0)
            {
                this.Lethal = damage.Lethal;
                message = @$"Lethal found by {this.Current.Name}: Damage {damage.Lethal} - Opponent Health {health} = {damage.Lethal - health}.
Action:
    {damage.ToString("\r\n    ")}
";
            }
            message += @$"Message:
    {damage.Message}
";
            if (this._message != message)
            {
                this._message = message;
                this.label.Content = this._message;
            }
        }
    }
}