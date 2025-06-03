namespace IdfOperation.Web.Models
{
    public class Zik : Weapon, IFuelable
    {
        private int _fuel;

        //====================================
        public Zik(int number)
            : base($"Zik-{number}", 3, new List<string> { "people", "vehicles" }, 3)
        {
            _fuel = 30;
        }

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
            if (Ammo < 1)
                throw new InvalidOperationException("Not enough ammo for Zik.");

            Ammo -= 1;
        }

        //--------------------------------------------------------------
        public override void UseAmmo(double weight)
        {
            UseAmmo();
        }
    }
}