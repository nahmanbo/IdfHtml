namespace IdfOperation.GoodGuys.Firepower
{
    public interface IFuelable
    {
        void AddFuel();
        void LessFuel();
        int GetFuel();
    }
}