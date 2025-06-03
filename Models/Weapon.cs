using System.Text.Json;

namespace IdfOperation.Web.Models
{
    public abstract class Weapon
    {
        protected string Name { get; }
        protected double Ammo { get; set; }
        protected int MaxAmmo { get; }
        protected List<string> TargetTypes { get; }

        //====================================
        protected Weapon(string name, double ammo, List<string> effective, int maxAmmo)
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
        public void UpdateAmmo(double count)
        {
            Ammo = Math.Min(Ammo + count, MaxAmmo);
        }

        //--------------------------------------------------------------
        public double GetAmmo()
        {
            return Ammo;
        }

        //--------------------------------------------------------------
        public virtual string GetInfoJson()
        {
            var info = new
            {
                Name,
                Ammo = $"{Ammo}/{MaxAmmo}",
                TargetTypes,
                Fuel = this is IFuelable f ? $"{f.GetFuel()} liters" : "N/A"
            };

            return JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
        }

        //--------------------------------------------------------------
        public virtual void UseAmmo()
        {
            throw new NotImplementedException("UseAmmo() must be overridden for fixed-ammo weapons.");
        }

        //--------------------------------------------------------------
        public virtual void UseAmmo(double weight)
        {
            UseAmmo();
        }

        //--------------------------------------------------------------
        public void AttackTarget(Terrorist terrorist, double weight = 1)
        {
            UseAmmo(weight);

            if (this is IFuelable fuelable)
                fuelable.LessFuel();

            terrorist.Kill();
            DbManager.UpdateTerroristInDB(terrorist);
            Console.WriteLine($"{Name} attacked and killed {terrorist.Name} with {weight} bomb.");
        }
    }
}
