using System.Text.Json;

namespace IdfOperation.Web.Models
{
    public class IntelligenceDivision
    {
        private List<IntelligenceReport> _reports = new();
        private static Random _rand = new();

        private static List<string> _locations = new()
        {
            "buildings", "people", "vehicles", "open areas", "Fence space"
        };

        //====================================
        public IntelligenceDivision()
        {
            foreach (var terrorist in Hamas.Instance.GetTerrorists())
            {
                string location = _locations[_rand.Next(_locations.Count)];
                _reports.Add(new IntelligenceReport(terrorist, location, DateTime.Now));
            }
        }

        //--------------------------------------------------------------
        public List<IntelligenceReport> GetReports()
        {
            return _reports;
        }

        //--------------------------------------------------------------
        public IntelligenceReport? GetById(int id)
        {
            foreach (var report in _reports)
            {
                var t = report.GetTerrorist();
                if (t.IsAlive && t.Id == id)
                    return report;
            }
            return null;
        }

        //--------------------------------------------------------------
        public IntelligenceReport? GetMostDangerous()
        {
            IntelligenceReport? top = null;

            foreach (var report in _reports)
            {
                var t = report.GetTerrorist();
                if (!t.IsAlive) continue;

                if (top == null || report.GetThreatLevel() > top.GetThreatLevel())
                    top = report;
            }

            return top;
        }

        //--------------------------------------------------------------
        public object[] GetInfo()
        {
            var grouped = _reports
                .Select(r => new
                {
                    Name = r.GetTerrorist().Name,
                    Id = r.GetTerrorist().Id,
                    Rank = r.GetTerrorist().Rank,
                    Status = r.GetTerrorist().IsAlive ? "Alive" : "Dead",
                    Weapons = r.GetTerrorist().Weapons,
                    Threat = r.GetThreatLevel(),
                    Location = r.GetLastKnownLocation(),
                    ReportTime = r.GetReportTime().ToString("yyyy-MM-dd HH:mm")
                })
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key, g => g.ToList<object>());

            var header = "IDF - Terrorist Reports";
            var description = $"Alive: {grouped.GetValueOrDefault("Alive")?.Count ?? 0}, Dead: {grouped.GetValueOrDefault("Dead")?.Count ?? 0}";

            return new object[] { header, description, grouped };
        }

    }
}
