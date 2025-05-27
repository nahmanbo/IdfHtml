using System.Text;
using IdfOperation.BadGuys;

namespace IdfOperation.GoodGuys.Intelligence
{
    public class IntelligenceDivision
    {
        private readonly List<IntelligenceReport> _reports;
        private static readonly List<string> Locations = new() { "buildings", "people", "vehicles", "open areas", "Fence space" };
        private static readonly Random Random = new();

        //====================================
        public IntelligenceDivision()
        {
            _reports = new List<IntelligenceReport>();

            foreach (var terrorist in Hamas.Instance.GetTerrorists())
            {
                string location = GetRandomLocation();
                var report = new IntelligenceReport(terrorist, location, DateTime.Now);
                _reports.Add(report);
            }
        }

        //--------------------------------------------------------------
        private string GetRandomLocation()
        {
            int index = Random.Next(Locations.Count);
            return Locations[index];
        }

        //--------------------------------------------------------------
        public IReadOnlyList<IntelligenceReport> GetReports()
        {
            return _reports.AsReadOnly();
        }

        //--------------------------------------------------------------
        public IntelligenceReport? GetMostDangerousAliveReport()
        {
            IntelligenceReport? mostDangerous = null;

            foreach (var report in _reports)
            {
                var terrorist = report.GetTerrorist();

                if (!terrorist.IsAlive)
                    continue;

                if (mostDangerous == null || report.GetThreatLevel() > mostDangerous.GetThreatLevel())
                    mostDangerous = report;
            }

            return mostDangerous;
        }

        //--------------------------------------------------------------
        public IntelligenceReport? GetReportByTerroristName(string name)
        {
            name = name.Trim();  // מסיר רווחים מיותרים או ירידת שורה

            foreach (var report in _reports)
            {
                var terrorist = report.GetTerrorist();

                if (string.Equals(terrorist.Name, name, StringComparison.OrdinalIgnoreCase) && terrorist.IsAlive)
                {
                    return report;
                }
            }
            return null;
        }

        //--------------------------------------------------------------
        public string GetInfo()
        {
            var sb = new StringBuilder();

            foreach (var report in _reports)
            {
                sb.AppendLine(report.GetInfo());
                sb.AppendLine("--------------------------------");
            }

            return sb.ToString();
        }
    }
}
