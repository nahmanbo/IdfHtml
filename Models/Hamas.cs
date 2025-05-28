using System.Text;
using IdfOperation.Organizations;

namespace IdfOperation.BadGuys
{
    public class Hamas : Organization
    {
        private static Hamas? _instance;
        private readonly List<Terrorist> _terrorists;

        //====================================
        public static Hamas Instance => _instance!;

        //====================================
        public Hamas(string currentCommander)
            : base(new DateTime(1987, 12, 14), currentCommander)
        {
            _instance = this;
            _terrorists = TerroristGenerator.Generate(3).Result;
        }

        //--------------------------------------------------------------
        public IReadOnlyList<Terrorist> GetTerrorists()
        {
            return _terrorists.AsReadOnly();
        }

        //--------------------------------------------------------------
        public void AddTerrorist(Terrorist terrorist)
        {
            _terrorists.Add(terrorist);
        }

        //--------------------------------------------------------------
        public override string GetInfo()        
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== Terrorist Table ===");
            sb.AppendLine("Name | Id | Rank | Status | Weapons");

            foreach (var terrorist in _terrorists)
            {
                sb.AppendLine(terrorist.GetInfo());
            }

            return sb.ToString();
        }    }
}
