namespace IdfOperation.Web.Models
{
    public class Tank : Weapon, IFuelable
    {
        private int _fuel;

        //====================================
        public Tank(int number)
            : base($"Tank-{number}", 40, new List<string> { "open areas" }, 40)
        {
            _fuel = 50;
        }

        //--------------------------------------------------------------
        public void AddFuel()
        {
            _fuel = 50;
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
            throw new NotImplementedException("UseAmmo(weight) must be used for Tank.");
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
                throw new InvalidOperationException("Not enough ammo for Tank.");

            Ammo -= weight;
        }
    }
}