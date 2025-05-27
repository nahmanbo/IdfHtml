using IdfOperation.BadGuys;
using System.Text;

namespace IdfOperation.GoodGuys.Intelligence
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
        public string GetInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== Intelligence Report ===");
            sb.AppendLine(_terrorist.GetInfo());
            sb.AppendLine($"Threat Level: {_threatLevel}");
            sb.AppendLine($"Last Known Location: {_lastKnownLocation}");
            sb.AppendLine($"Report Date: {_reportTime:yyyy-MM-dd HH:mm}");

            return sb.ToString();
        }
    }
}
