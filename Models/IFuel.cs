namespace IdfOperation.Web.Models
{
    public interface IFuelable
    {
        void AddFuel();
        void LessFuel();
        int GetFuel();
    }
}