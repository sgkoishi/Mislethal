using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthDb;
using HearthDb.CardDefs;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker;
using DruidCards = HearthDb.CardIds.Collectible.Druid;
using HunterCards = HearthDb.CardIds.Collectible.Hunter;
using MageCards = HearthDb.CardIds.Collectible.Mage;
using NeutralCards = HearthDb.CardIds.Collectible.Neutral;
using PaladinCards = HearthDb.CardIds.Collectible.Paladin;
using PriestCards = HearthDb.CardIds.Collectible.Priest;
using RogueCards = HearthDb.CardIds.Collectible.Rogue;
using ShamanCards = HearthDb.CardIds.Collectible.Shaman;
using WarlockCards = HearthDb.CardIds.Collectible.Warlock;
using WarriorCards = HearthDb.CardIds.Collectible.Warrior;

namespace Chireiden.Mislethal
{
    public abstract class Combo
    {
        public abstract string Name { get; }
        public abstract double Match(string[] deck);
        public abstract int Damage(string[] hand);
    }

    public class VargothThreeTurnKillMage : Combo
    {
        private const string Vargoth = NeutralCards.ArchmageVargoth;
        private const string Quest = MageCards.OpenTheWaygate;
        private const string MoltenReflection = MageCards.MoltenReflection;
        private const string Drakkari = NeutralCards.DrakkariEnchanter;
        private const string GreaterMissle = MageCards.GreaterArcaneMissiles;
        private const string Cinderstorm = MageCards.Cinderstorm;
        private const string TimeWrap = CardIds.NonCollectible.Mage.OpentheWaygate_TimeWarpToken;

        public override string Name => "Vargoth Time Warp Mage";

        public override int Damage(string[] hand)
        {
            if (!hand.Contains(Vargoth) || !hand.Contains(TimeWrap))
            {
                return 0;
            }

            var damagePerSpell = 0;
            if (hand.Contains(MageCards.ArcaneMissiles))
            {
                damagePerSpell = 3;
                if (hand.Contains(NeutralCards.Malygos))
                {
                    damagePerSpell = 8;
                }
            }
            if (hand.Contains(Cinderstorm))
            {
                damagePerSpell = Math.Max(damagePerSpell, 5);
            }
            if (hand.Contains(GreaterMissle))
            {
                damagePerSpell = 9;
            }
            var useMill = hand.Contains(MageCards.ResearchProject) && hand.Contains(NeutralCards.VioletIllusionist);
            if (damagePerSpell == 0 && !useMill)
            {
                return 0;
            }

            var doubleMolten = hand.Count(MoltenReflection) > 1;
            var hasDrakkari = hand.Contains(Drakkari);
            var numVargoth = 0;
            var numVargothRange = new int[] { 2, 3, 4, 5, 6 };
            if (useMill)
            {
                numVargoth = doubleMolten ? 6 : 4;
                if (hasDrakkari)
                {
                    numVargoth = doubleMolten ? 10 : 8;
                    numVargothRange = new int[] { 4, 6, 8, 10 };
                }
                else
                {
                    numVargothRange = new int[] { numVargoth };
                }
                Console.WriteLine($"Draw Possibilities: {string.Join(", ", numVargothRange)} ({numVargoth})");
                Console.WriteLine($"Estimate Damage: {string.Join(", ", numVargothRange.ToMill(Core.Game.Opponent.DeckCount))} ({numVargoth.ToMill(Core.Game.Opponent.DeckCount)})");
                return numVargoth.ToMill(Core.Game.Opponent.DeckCount);
            }
            else
            {
                numVargoth = doubleMolten ? 6 : 4;
                if (hasDrakkari)
                {
                    if (damagePerSpell == 8)
                    {
                        numVargothRange = new int[] { 4, 6, 8, 10 };
                    }
                    else
                    {
                        numVargoth *= 2;
                        numVargothRange = new int[] { 4, 6, 8, 10, 12 };
                    }
                }
                else
                {
                    numVargothRange = (new int[] { numVargoth });
                }
                numVargothRange = numVargothRange.Select(n => n * damagePerSpell).ToArray();
                Console.WriteLine($"Damage Possibilities: {string.Join(", ", numVargothRange)} ({numVargoth * damagePerSpell})");
                return (damagePerSpell != 9)
                    ? (numVargoth * damagePerSpell) - Utils.EnemyMinions.Sum(e => e.Health)
                    : ((numVargoth * 3) - Utils.EnemyMinions.Sum(e => (int) Math.Ceiling(e.Health / 3.0))) * 3;
            }
        }

        // No idea how to match.
        public override double Match(string[] deck)
        {
            var prob = 0.0;
            if (!deck.Contains(Quest) || !deck.Contains(Vargoth))
            {
                return 0;
            }
            if (deck.Contains(Drakkari))
            {
                if (!deck.Contains(NeutralCards.EmperorThaurissan))
                {
                    prob += 0.3;
                }
                prob += 0.1;
            }
            if (deck.Contains(GreaterMissle))
            {
                prob += 0.4;
            }
            else if (deck.Contains(Cinderstorm))
            {
                prob += 0.3;
            }
            return Math.Min(1.0, prob + (0.2 * deck.Count(MoltenReflection)));
        }
    }
}
