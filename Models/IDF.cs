
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
            var sb = new StringBuilder();
            sb.AppendLine($"IDF - Commander: {GetCommander()}, Established: {GetEstablishmentDate():d}");
            sb.AppendLine("=== Firepower Division ===");
            sb.AppendLine(Firepower.GetInfo());
            sb.AppendLine("=== Intelligence Division ===");
            sb.AppendLine(Intelligence.GetInfo());
            return sb.ToString();
        }
    }
}
