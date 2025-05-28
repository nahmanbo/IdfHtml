using System.Text.Json;
using IdfOperation.BadGuys;
using IdfOperation.Factory;

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
            _reports = DbManager.GetIntelligenceReportFromDB();
            /*_reports = new List<IntelligenceReport>();

            foreach (var terrorist in Hamas.Instance.GetTerrorists())
            {
                string location = GetRandomLocation();
                var report = new IntelligenceReport(terrorist, location, DateTime.Now);
                _reports.Add(report);
            }*/
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
            name = name.Trim();

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
        public string GetInfoJson()
        {
            var reports = _reports.Select(r => new
            {
                Name = r.GetTerrorist().Name,
                Id = r.GetTerrorist().Id,
                Rank = r.GetTerrorist().Rank,
                Status = r.GetTerrorist().IsAlive ? "Alive" : "Dead",
                Weapons = r.GetTerrorist().Weapons,
                Threat = r.GetThreatLevel(),
                Location = r.GetLastKnownLocation(),
                ReportTime = r.GetReportTime().ToString("yyyy-MM-dd HH:mm")
            });

            return JsonSerializer.Serialize(reports, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
