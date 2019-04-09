using Hearthstone_Deck_Tracker.Hearthstone;
using System.Collections.Generic;
using System.Linq;

namespace Chireiden.Mislethal
{
    public class ComboAction
    {
        public ComboAction(int lethal, List<GameAction> gameActions)
        {
            this.Actions = gameActions;
            this.Lethal = lethal;
        }

        public ComboAction(int lethal, params GameAction[] gameActions)
        {
            this.Actions = gameActions.ToList();
            this.Lethal = lethal;
        }

        public List<GameAction> Actions = new List<GameAction>();
        public int Lethal;
        public string Message;

        public string ToString(string seperator)
        {
            return string.Join(seperator, this.Actions);
        }

        public static implicit operator ComboAction(int i)
        {
            return new ComboAction(i);
        }
    }

    public class GameAction
    {
        public GameAction(ActionType type, Card card = null, object target = null)
        {
            this.Action = type;
            this.Play = card;
            this._target = target;
        }

        public ActionType Action;
        public Card Play;
        private readonly object _target;
        public Card Target => (Card) this._target;
        public int Choice => (int) this._target;

        public override string ToString()
        {
            switch (this.Action)
            {
                case ActionType.Play:
                    return this.Play.Name;
                case ActionType.PlayTarget:
                    return this.Play.Name + "@" + this.Target.Name;
                case ActionType.PlayChoice:
                    return this.Play.Name + "@" + this.Choice;
                case ActionType.EndTurn:
                    return "[EndTurn]";
                default:
                    return "[Unknown]";
            }
        }
    }

    public enum ActionType
    {
        Play,
        PlayTarget,
        PlayChoice,
        EndTurn
    }
}
