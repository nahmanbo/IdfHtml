
namespace IdfOperation.GoodGuys.Firepower
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
            while (true)
            {
                Console.WriteLine($"You need a {Name}. Select quantity of shell: (2 or 3)");
                string input = Console.ReadLine()!;

                switch (input)
                {
                    case "2":
                        Ammo -= 2;
                        return;
                    case "3":
                        Ammo -= 3;
                        return;
                    default:
                        Console.WriteLine("Enter a valid number.");
                        break;
                }
            }
        }


    }
}
