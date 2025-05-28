
using IdfOperation.Organizations;
using IdfOperation.GoodGuys.Firepower;
using IdfOperation.GoodGuys.Intelligence;
using System.Text;

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
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== Firepower Table ===");
            sb.AppendLine("Name | Ammo | Targets | Fuel");
            sb.AppendLine(string.Join("\n", Firepower.GetInfo()
                .Split('\n')
                .Skip(2))); // skip duplicated title and headers

            sb.AppendLine();
            sb.AppendLine("=== Intelligence Reports Table ===");
            sb.AppendLine("Name | Id | Rank | Status | Weapons | Threat | Location | Report Time");
            sb.AppendLine(string.Join("\n", Intelligence.GetInfo()
                .Split('\n')
                .Skip(2))); // skip duplicated title and headers

            return sb.ToString();
        }
    }
}
