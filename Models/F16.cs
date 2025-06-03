namespace IdfOperation.Web.Models
{
    public class F16 : Weapon, IFuelable
    {
        private int _fuel;

        //====================================
        public F16(int number)
            : base($"F16-{number}", 8, new List<string> { "buildings" }, 8)
        {
            _fuel = 100;
        }

        //--------------------------------------------------------------
        public void AddFuel()
        {
            _fuel = 100;
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
            throw new NotImplementedException("UseAmmo(double weight) must be used for F16.");
        }

        //--------------------------------------------------------------
        public override void UseAmmo(double weight)
        {
            switch (weight)
            {
                case 0.5:
                case 1.0:
                    break;
                default:
                    throw new ArgumentException("Invalid bomb weight. Must be 0.5 or 1.");
            }

            if (Ammo < weight)
                throw new InvalidOperationException("Not enough ammo to fire.");

            Ammo -= weight;
        }
    }
}