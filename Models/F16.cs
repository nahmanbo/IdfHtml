
namespace IdfOperation.GoodGuys.Firepower
{
    public class F16 : Weapon, IFuelable
    {
        private int _fuel = 100;

        //====================================
        public F16(int number)
            : base($"F16-{number}", 8, new List<string> { "buildings" }, 8) {}

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
            while (true)
            {
                Console.Write($"You need a {Name}. Enter Bomb weight (0.5 or 1): ");
                string input = Console.ReadLine()!;

                switch (input)
                {
                    case "1":
                        Ammo -= 1;
                        return;
                    case "0.5":
                        Ammo -= 0.5;
                        return;
                    default:
                        Console.WriteLine("Enter a valid number.");
                        break;
                }
            }
        }
    }
}
