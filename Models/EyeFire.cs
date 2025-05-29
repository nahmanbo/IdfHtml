namespace IdfOperation.Web.Models
{
    public class EyeFire : Weapon
    {
        //====================================
        public EyeFire(int number)
            : base($"EyeFire-{number}", 1200, new List<string> { "Fence space" }, 1200)
        {
        }

        //--------------------------------------------------------------
        public override void UseAmmo()
        {
            Ammo -= 30;
        }
    }
}