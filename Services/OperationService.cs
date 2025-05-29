using IdfOperation.Web.Models;
using System.Text.Json;
using System.Text.Encodings.Web;


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
        
        public string ViewReportById(int id)
        {
            var report = _idf.Intelligence.GetById(id);
            return report != null ? report.GetInfoJson() : $"No report found for terrorist ID: {id}";
        }

        public string ViewMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            return report != null ? report.GetInfoJson() : "No alive terrorist reports available.";
        }

        //--------------------------------------------------------------
        public string EliminateById(int id)
        {
            var report = _idf.Intelligence.GetById(id);
            return report != null ? TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation()) : $"No intelligence report found for terrorist ID: {id}";
        }

        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            return report != null ? TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation()) : "No alive intelligence reports available.";
        }

        public string EliminateByTargetType(string targetType)
        {
            var normalizedTarget = targetType.Trim().ToLower();
            var reports = _idf.Intelligence.GetReports()
                .Where(r => r.GetLastKnownLocation().Trim().ToLower() == normalizedTarget)
                .ToList();

            if (!reports.Any())
                return $"No eligible terrorists found for target type: {targetType}";

            var results = reports
                .Select(r => TryEliminate(r.GetTerrorist(), normalizedTarget))
                .ToList();

            return JsonSerializer.Serialize(results, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        //--------------------------------------------------------------
        private string TryEliminate(Terrorist terrorist, string targetType)
        {
            var report = _idf.Intelligence.GetById(terrorist.Id);
            var reportData = report != null ? JsonSerializer.Deserialize<object>(report.GetInfoJson()) : null;

            if (report == null)
                return FormatResult("❌ Elimination failed", $"No report found for terrorist ID: {terrorist.Id}", null);

            if (!terrorist.IsAlive)
                return FormatResult("❌ Elimination failed", $"Target already dead | Target: {terrorist.Name}", reportData);

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
                return FormatResult("❌ Elimination failed", $"No weapon available for target type: {targetType} | Target: {terrorist.Name}", reportData);

            weapon.AttackTarget(terrorist);

            var status = terrorist.IsAlive ? "❌ Elimination failed" : "✅ Elimination successful";
            var summary = $"Weapon: {weapon.GetType().Name} | Target: {terrorist.Name}";

            return FormatResult(status, summary, reportData);
        }

        //--------------------------------------------------------------
        private string FormatResult(string status, string message, object? data)
        {
            var result = new object[] { status, message, data };
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
