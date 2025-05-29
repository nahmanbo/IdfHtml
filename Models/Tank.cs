namespace IdfOperation.Web.Models
{
    public class Tank : Weapon, IFuelable
    {
        private int _fuel = 50;

        //====================================
        public Tank(int number)
            : base($"Tank-{number}", 40, new List<string> { "open areas" }, 40)
        {
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
            throw new NotImplementedException("UseAmmo(int shells) must be used instead.");
        }

        //--------------------------------------------------------------
        public void UseAmmo(int shells)
        {
            if (shells != 2 && shells != 3)
                throw new ArgumentException("Invalid shell quantity. Must be 2 or 3.");

            Ammo -= shells;
        }
    }
}