using System.Text.Json;

namespace IdfOperation.Web.Models
{
    public class IntelligenceReport
    {
        private Terrorist _terrorist;
        private int _threatLevel;
        private string _lastKnownLocation;
        private DateTime _reportTime;

        //====================================
        public IntelligenceReport(Terrorist terrorist, string lastKnownLocation, DateTime reportTime)
        {
            _terrorist = terrorist;
            _lastKnownLocation = lastKnownLocation;
            _reportTime = reportTime;
            _threatLevel = CalculateThreatScore();
        }

        //--------------------------------------------------------------
        private int CalculateThreatScore()
        {
            int weaponScore = 0;

            foreach (var weapon in _terrorist.Weapons)
            {
                switch (weapon.ToLower())
                {
                    case "knife": weaponScore += 1; break;
                    case "gun": weaponScore += 2; break;
                    case "m16":
                    case "ak47": weaponScore += 3; break;
                }
            }

            return _terrorist.Rank * weaponScore;
        }

        //--------------------------------------------------------------
        public Terrorist GetTerrorist()
        {
            return _terrorist;
        }

        //--------------------------------------------------------------
        public int GetThreatLevel()
        {
            return _threatLevel;
        }

        //--------------------------------------------------------------
        public string GetLastKnownLocation()
        {
            return _lastKnownLocation;
        }

        //--------------------------------------------------------------
        public DateTime GetReportTime()
        {
            return _reportTime;
        }

        //--------------------------------------------------------------
        public void UpdateLastKnownLocation(string newLocation)
        {
            if (!string.IsNullOrWhiteSpace(newLocation))
            {
                _lastKnownLocation = newLocation;
                _reportTime = DateTime.Now;
            }
        }

        //--------------------------------------------------------------
        public string GetInfoJson()
        {
            var info = new
            {
                Name = _terrorist.Name,
                Id = _terrorist.Id,
                Rank = _terrorist.Rank,
                Status = _terrorist.IsAlive ? "Alive" : "Dead",
                Weapons = _terrorist.Weapons,
                Threat = _threatLevel,
                Location = _lastKnownLocation,
                ReportTime = _reportTime.ToString("yyyy-MM-dd HH:mm")
            };

            return JsonSerializer.Serialize(new[] { info }, new JsonSerializerOptions { WriteIndented = true });
        }

    }
}
