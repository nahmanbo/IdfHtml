using IdfOperation.Organizations;
using IdfOperation.GoodGuys.Firepower;
using IdfOperation.GoodGuys.Intelligence;
using System.Text.Json;

namespace IdfOperation.GoodGuys
{
    public class Idf : Organization
    {
        public FirepowerDivision Firepower { get; private set; }
        public IntelligenceDivision Intelligence { get; private set; }

        public Idf(string currentCommander)
            : base(new DateTime(1948, 5, 31), currentCommander)
        {
            Firepower = new FirepowerDivision();
            Intelligence = new IntelligenceDivision();
        }

        public override string GetInfo()
        {
            var info = new
            {
                Firepower = JsonSerializer.Deserialize<object>(Firepower.GetInfoJson()),
                IntelligenceReports = JsonSerializer.Deserialize<object>(Intelligence.GetInfoJson())
            };

            return JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}