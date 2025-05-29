using IdfOperation.GoodGuys;
using IdfOperation.BadGuys;
using System.Text;

namespace IdfOperation.Web.Services
{
    public class OperationService
    {
        private readonly Idf _idf;
        private readonly Hamas _hamas;

        //====================================
        public OperationService()
        {
            _hamas = new Hamas("Yahya Sinwar");
            _idf = new Idf("Eyal Zamir");
        }

        //--------------------------------------------------------------
        public string ViewFullIdfInfo()
        {
            return _idf.GetInfo();
        }

        //--------------------------------------------------------------
        public string ViewFullHamasInfo()
        {
            return _hamas.GetInfo();
        }

        //--------------------------------------------------------------
        public string ViewFirepowerData()
        {
            return _idf.Firepower.GetInfoJson();
        }

        //--------------------------------------------------------------
        public string ViewIntelligenceReports()
        {
            return _idf.Intelligence.GetInfoJson();
        }

        //--------------------------------------------------------------
        public string ViewReportById(int id)
        {
            var report = _idf.Intelligence.GetById(id);

            if (report == null)
                return $"No report found for terrorist ID: {id}";

            return report.GetInfoJson();
        }

        //--------------------------------------------------------------
        public string ViewMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            if (report == null)
                return "No alive terrorist reports available.";
            
            Console.WriteLine(report.GetInfoJson());
            return report.GetInfoJson();
        }

        //--------------------------------------------------------------
        private string? TryEliminate(Terrorist terrorist, string targetType)
        {
            Console.WriteLine($"➡️ Attempting to eliminate: {terrorist.Name} (ID: {terrorist.Id})");
            Console.WriteLine($"    Alive before: {terrorist.IsAlive}");

            if (!terrorist.IsAlive)
            {
                Console.WriteLine("⛔ Already dead.");
                return $"{terrorist.Name} is already dead.";
            }

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
            {
                Console.WriteLine("⚠️ No weapon available for target type: " + targetType);
                return $"No weapon available for target type: {targetType}";
            }

            Console.WriteLine($"✅ Using weapon: {weapon.GetType().Name}");
            weapon.AttackTarget(terrorist);

            Console.WriteLine($"    Alive after: {terrorist.IsAlive}");

            return $"=== Elimination Result ===\nWeapon: {weapon.GetType().Name} | Target: {terrorist.Name} | Status: Eliminated";
        }


        //--------------------------------------------------------------
        public string EliminateById(int id)
        {
            var report = _idf.Intelligence.GetById(id);

            if (report == null)
                return $"No intelligence report found for terrorist ID: {id}";

            return TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation())
                   ?? $"Unable to eliminate terrorist ID: {id}.";
        }

        //--------------------------------------------------------------
        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            if (report == null)
                return "No alive intelligence reports available.";

            var terrorist = report.GetTerrorist();
            return TryEliminate(terrorist, report.GetLastKnownLocation())
                   ?? $"Unable to eliminate {terrorist.Name}.";
        }

        //--------------------------------------------------------------
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
