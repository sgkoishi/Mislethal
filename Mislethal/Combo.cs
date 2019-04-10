namespace Chireiden.Mislethal
{
    public abstract class Combo
    {
        public abstract string Name { get; }
        public abstract double Match();
        public abstract ComboAction Damage();
    }
}
