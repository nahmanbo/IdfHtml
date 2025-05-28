using IdfOperation.BadGuys;
using System.Text.Json;

namespace IdfOperation.GoodGuys.Intelligence
{
    public class IntelligenceReport
    {
        public Terrorist _terrorist{get;set;}
        public int _threatLevel{get;set;}
        public string _lastKnownLocation{get;set;}
        public DateTime _reportTime{get;set;}

        //====================================
        public IntelligenceReport()
        {

        }
        public IntelligenceReport(Terrorist terrorist, string lastKnownLocation, DateTime reportTime)
        {
            _terrorist = terrorist;
            _threatLevel = CalculateThreatScore();
            _lastKnownLocation = lastKnownLocation;
            _reportTime = reportTime;
        }



        //--------------------------------------------------------------
        private int CalculateThreatScore()
        {
            int weaponScore = 0;

            foreach (var weapon in _terrorist.Weapons)
            {
                if (weapon.Equals("Knife", StringComparison.OrdinalIgnoreCase))
                    weaponScore += 1;
                else if (weapon.Equals("Gun", StringComparison.OrdinalIgnoreCase))
                    weaponScore += 2;
                else if (weapon.Equals("M16", StringComparison.OrdinalIgnoreCase) || weapon.Equals("AK47", StringComparison.OrdinalIgnoreCase))
                    weaponScore += 3;
            }

            return _terrorist.Rank * weaponScore;
        }

        //--------------------------------------------------------------
        public void UpdateLastKnownLocation(string newLocation)
        {
            if (string.IsNullOrWhiteSpace(newLocation))
                throw new ArgumentException("Location cannot be empty.");

            _lastKnownLocation = newLocation;
            UpdateReportTime(DateTime.Now);
        }

        //--------------------------------------------------------------
        public void UpdateReportTime(DateTime newTime)
        {
            _reportTime = newTime;
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

            return JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}