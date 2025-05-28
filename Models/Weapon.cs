
using IdfOperation.BadGuys;

namespace IdfOperation.GoodGuys.Firepower
{
    public abstract class Weapon
    {
        protected string Name { get; }
        protected double Ammo { get; set; }
        protected int MaxAmmo { get; }
        protected List<string> TargetTypes { get; }

        //====================================
        protected Weapon(string name, float ammo, List<string> effective, int maxAmmo)
        {
            Name = name;
            Ammo = ammo;
            TargetTypes = effective;
            MaxAmmo = maxAmmo;
        }

        //--------------------------------------------------------------
        public IReadOnlyList<string> GetTargetTypes()
        {
            return TargetTypes.AsReadOnly();
        }

        //--------------------------------------------------------------
        public abstract void UseAmmo();

        //--------------------------------------------------------------
        public void UpdateAmmo(float count)
        {
            Ammo = Math.Min(Ammo + count, MaxAmmo);
        }

        //--------------------------------------------------------------
        public double GetAmmo()
        {
            return Ammo;
        }

        //--------------------------------------------------------------
        public virtual string GetInfo()
        {
            string fuel = this is IFuelable f ? $"{f.GetFuel()} liters" : "N/A";
            return $"{Name} | {Ammo} / {MaxAmmo} | {string.Join(", ", TargetTypes)} | {fuel}";
        }
        //--------------------------------------------------------------
        public void AttackTarget(Terrorist terrorist)
        {
            UseAmmo();

            if (this is IFuelable fuelable)
                fuelable.LessFuel();

            terrorist.Kill();
            Console.WriteLine($"{Name} attacked and killed {terrorist.Name}.");
        }
    }
}
