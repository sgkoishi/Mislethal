using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using MageCards = HearthDb.CardIds.Collectible.Mage;
using NeutralCards = HearthDb.CardIds.Collectible.Neutral;

namespace Chireiden.Mislethal.Combos
{
    public class VargothThreeTurnKillMage : Combo
    {
        private const string Vargoth = NeutralCards.ArchmageVargoth;
        private const string Quest = MageCards.OpenTheWaygate;
        private const string MoltenReflection = MageCards.MoltenReflection;
        private const string Drakkari = NeutralCards.DrakkariEnchanter;
        private const string GreaterMissle = MageCards.GreaterArcaneMissiles;
        private const string Cinderstorm = MageCards.Cinderstorm;
        private const string TimeWrap = HearthDb.CardIds.NonCollectible.Mage.OpentheWaygate_TimeWarpToken;

        public override string Name => "Vargoth Time Warp Mage";

        public override ComboAction Damage()
        {
            if (!Main.Self.Hand.Contains(Vargoth))
            {
                return new ComboAction(0) { Message = "Vargoth missing." };
            }

            if (!Main.Self.Hand.Contains(TimeWrap))
            {
                return new ComboAction(0) { Message = "Time Wrap missing." };
            }

            var damagePerSpell = 0;
            if (Main.Self.Hand.Contains(MageCards.ArcaneMissiles))
            {
                damagePerSpell = 3;
                if (Main.Self.Hand.Contains(NeutralCards.Malygos))
                {
                    damagePerSpell = 8;
                }
            }
            if (Main.Self.Hand.Contains(Cinderstorm))
            {
                damagePerSpell = Math.Max(damagePerSpell, 5);
            }
            if (Main.Self.Hand.Contains(GreaterMissle))
            {
                damagePerSpell = 9;
            }
            var useMill = Main.Self.Hand.Contains(MageCards.ResearchProject) && Main.Self.Hand.Contains(NeutralCards.VioletIllusionist);
            if (damagePerSpell == 0 && !useMill)
            {
                return new ComboAction(0) { Message = "Damage spell, Research Project and Violet Illusionist missing." };
            }

            var doubleMolten = Main.Self.Hand.Count(MoltenReflection) > 1;
            var hasDrakkari = Main.Self.Hand.Contains(Drakkari);
            var numVargoth = 0;
            var numVargothRange = new int[] { 2, 3, 4, 5, 6 };
            var action = new List<GameAction>
            {
                new GameAction(ActionType.Play, Main.Self.Hand.First(Vargoth)),
                new GameAction(ActionType.Play, Main.Self.Hand.First(TimeWrap)),
                new GameAction(ActionType.EndTurn),
                new GameAction(ActionType.PlayTarget, Main.Self.Hand.First(MoltenReflection), Main.Self.Hand.First(Vargoth))
            };
            if (doubleMolten)
            {
                action.Add(new GameAction(ActionType.PlayTarget, Main.Self.Hand.First(MoltenReflection), Main.Self.Hand.First(Vargoth)));
            }
            action.Add(new GameAction(ActionType.EndTurn));
            if (useMill)
            {
                action.Add(new GameAction(ActionType.Play, Main.Self.Hand.First(NeutralCards.VioletIllusionist)));
                if (hasDrakkari)
                {
                    action.Add(new GameAction(ActionType.Play, Main.Self.Hand.First(Drakkari)));
                    numVargoth = doubleMolten ? 22 : 18;
                    numVargothRange = new int[] { 10, 14, 18, 22 };
                }
                else
                {
                    numVargoth = doubleMolten ? 14 : 10;
                    numVargothRange = new int[] { numVargoth };
                }
                action.Add(new GameAction(ActionType.Play, Main.Self.Hand.First(MageCards.ResearchProject)));
                action.Add(new GameAction(ActionType.EndTurn));
                return new ComboAction(numVargoth.ToMill(Core.Game.Opponent.DeckCount - 1), action)
                {
                    Message = @$"Draw Possibilities: {string.Join(", ", numVargothRange)} ({numVargoth})
    Estimate Damage: {string.Join(", ", numVargothRange.ToMill(Core.Game.Opponent.DeckCount))} ({numVargoth.ToMill(Core.Game.Opponent.DeckCount)})"
                };
            }
            else
            {
                if (hasDrakkari)
                {
                    if (damagePerSpell == 8)
                    {
                        action.Add(new GameAction(ActionType.Play, Main.Self.Hand.First(NeutralCards.Malygos)));
                        numVargoth = doubleMolten ? 6 : 4;
                        numVargothRange = new int[] { 4, 6, 8, 10 };
                    }
                    else
                    {
                        action.Add(new GameAction(ActionType.Play, Main.Self.Hand.First(Drakkari)));
                        numVargoth = doubleMolten ? 12 : 8;
                        numVargothRange = new int[] { 4, 6, 8, 10, 12 };
                    }
                }
                else
                {
                    numVargoth = doubleMolten ? 6 : 4;
                    numVargothRange = (new int[] { numVargoth });
                }
                Card damageSpell;
                switch (damagePerSpell)
                {
                    case 5:
                    {
                        damageSpell = Main.Self.Hand.First(Cinderstorm);
                        break;
                    }
                    case 9:
                    {
                        damageSpell = Main.Self.Hand.First(GreaterMissle);
                        break;
                    }
                    default:
                    {
                        damageSpell = Main.Self.Hand.First(MageCards.ArcaneMissiles);
                        break;
                    }
                }
                action.Add(new GameAction(ActionType.Play, damageSpell));
                action.Add(new GameAction(ActionType.EndTurn));
                numVargothRange = numVargothRange.Select(n => n * damagePerSpell).ToArray();
                var enemyBoard = damagePerSpell == 9
                    ? Main.Oppo.Board.Sum(e => 3 * (int) Math.Ceiling(e.Health / 3.0))
                    : Main.Oppo.Board.Sum(e => e.Health);
                return new ComboAction((numVargoth * damagePerSpell) - enemyBoard, action)
                {
                    Message = $"Damage Possibilities: {string.Join(", ", numVargothRange)} ({numVargoth * damagePerSpell} - {enemyBoard})"
                };
            }
        }

        // No idea how to match.
        public override double Match()
        {
            var prob = 0.0;
            if (!Main.Self.Deck.Contains(Quest) || !Main.Self.Deck.Contains(Vargoth))
            {
                return 0;
            }
            if (Main.Self.Deck.Contains(Drakkari))
            {
                if (!Main.Self.Deck.Contains(NeutralCards.EmperorThaurissan))
                {
                    prob += 0.3;
                }
                prob += 0.1;
            }
            if (Main.Self.Deck.Contains(GreaterMissle))
            {
                prob += 0.4;
            }
            else if (Main.Self.Deck.Contains(Cinderstorm))
            {
                prob += 0.3;
            }
            return Math.Min(1.0, prob + (0.2 * Main.Self.Deck.Count(MoltenReflection)));
        }
    }
}
