namespace IdfOperation.GoodGuys.Firepower
{
    public class F16 : Weapon, IFuelable
    {
        private int _fuel = 100;

        //====================================
        public F16(int number)
            : base($"F16-{number}", 8, new List<string> { "buildings" }, 8)
        {
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
            throw new NotImplementedException("UseAmmo(double weight) must be used instead.");
        }

        //--------------------------------------------------------------
        public void UseAmmo(double weight)
        {
            if (weight != 0.5 && weight != 1)
                throw new ArgumentException("Invalid bomb weight. Must be 0.5 or 1.");

            Ammo -= weight;
        }
    }
}