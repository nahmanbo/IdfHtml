
namespace IdfOperation.GoodGuys.Firepower
{
    public class Zik : Weapon, IFuelable
    {
        private int _fuel = 30;

        //==============================================================
        public Zik(int number)
            : base($"Zik-{number}", 3, new List<string> { "people", "vehicles" }, 3) {}

        //--------------------------------------------------------------
        public void AddFuel()
        {
            _fuel = 30;
        }

        //--------------------------------------------------------------
        public void LessFuel()
        {
            _fuel = Math.Max(0, _fuel - 5);
        }

        //--------------------------------------------------------------
        public int GetFuel()
        {
            return _fuel;
        }
        //--------------------------------------------------------------
        public override void UseAmmo()
        {
            Ammo -= 1;
        }
        //--------------------------------------------------------------
        public override string GetInfo()
        {
            return $"Name: {Name}, Ammo: {Ammo}/{MaxAmmo}, Effective Against: {string.Join(", ", TargetTypes)}, Fuel: {_fuel} liters";
        }
    }
}
