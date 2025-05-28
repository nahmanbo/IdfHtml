
using IdfOperation.GoodGuys;
using IdfOperation.BadGuys;
using System.Text;

namespace IdfOperation.Web.Services
{
    public class OperationService
    {
        private readonly Idf _idf;
        private readonly Hamas _hamas;

        public OperationService()
        {
            _hamas = new Hamas("Yahya Sinwar");
            _idf = new Idf("Eyal Zamir");
        }

        public string ViewFullIdfInfo()
        {
            return _idf.GetInfo();
        }

        public string ViewFullHamasInfo()
        {
            return _hamas.GetInfo();
        }

        public string ViewFirepowerData()
        {
            return _idf.Firepower.GetInfo();
        }

        public string ViewIntelligenceReports()
        {
            return _idf.Intelligence.GetInfo();
        }

        public string ViewReportByName(string name)
        {
            var report = _idf.Intelligence.GetReportByTerroristName(name);

            if (report == null)
                return $"No report found for terrorist: {name}";

            return report.GetInfo();
        }

        public string ViewMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerousAliveReport();
            if (report == null)
                return "No alive terrorist reports available.";

            return report.GetInfo();
        }

        private string? TryEliminate(Terrorist terrorist, string targetType)
        {
            if (!terrorist.IsAlive)
                return $"{terrorist.Name} is already dead.";

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
                return $"No weapon available for target type: {targetType}";

            weapon.AttackTarget(terrorist);
            return
                $"=== Elimination Result ===Weapon: {weapon.GetType().Name} Target: {terrorist.Name} Status: Eliminated";
        }

        public string EliminateByName(string name)
        {
            name = name.Trim();
            var report = _idf.Intelligence.GetReportByTerroristName(name);

            if (report == null)
                return $"No intelligence report found for terrorist: {name}";

            return TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation())
                   ?? $"Unable to eliminate {name}.";
        }

        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerousAliveReport();
            if (report == null)
                return "No alive intelligence reports available.";

            var terrorist = report.GetTerrorist();
            return TryEliminate(terrorist, report.GetLastKnownLocation())
                   ?? $"Unable to eliminate {terrorist.Name}.";
        }

        public string EliminateByTargetType(string targetType)
        {
            targetType = targetType.Trim().ToLower();
            var messages = new List<string>();

            foreach (var report in _idf.Intelligence.GetReports())
            {
                if (report.GetLastKnownLocation().Trim().ToLower() != targetType)
                    continue;

                var result = TryEliminate(report.GetTerrorist(), targetType);
                if (result != null)
                    messages.Add(result);
            }

            return messages.Count > 0
                ? string.Join("\n---\n", messages)
                : $"No eligible terrorists found for target type: {targetType}";
        }
    }
}
