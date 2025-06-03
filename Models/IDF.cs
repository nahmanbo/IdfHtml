using System.Text.Json;
using IdfOperation.Web.Models;

namespace IdfOperation.Web.Models
{
    public class Idf : Organization
    {
        public FirepowerDivision Firepower { get; private set; }
        public IntelligenceDivision Intelligence { get; private set; }

        //====================================
        public Idf(string currentCommander)
            : base(new DateTime(1948, 5, 31), currentCommander)
        {
            Firepower = new FirepowerDivision();
            Intelligence = new IntelligenceDivision();
        }

        //--------------------------------------------------------------


        //--------------------------------------------------------------
        public override string GetInfo()
        {
            var flat = new List<object>();

            flat.AddRange(Firepower.GetInfo());
            flat.AddRange(Intelligence.GetInfo());

            return JsonSerializer.Serialize(flat, new JsonSerializerOptions { WriteIndented = true });
        }

    }
}