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
            return Serialize(_idf.Firepower.GetInfo());
        }

        //--------------------------------------------------------------
        public string ViewIntelligenceReports()
        {
            return Serialize(_idf.Intelligence.GetInfo());
        }

        //--------------------------------------------------------------
        public string ViewReportById(int id)
        {
            var report = _idf.Intelligence.GetById(id);
            if (report == null)
                return FormatResponse("❌ Report not found", $"No report found for terrorist ID: {id}");

            var data = DeserializeArray(report.GetInfoJson());
            return FormatResponse("📄 Report found", $"Terrorist ID: {id}", data);
        }

        //--------------------------------------------------------------
        public string ViewMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            if (report == null)
                return FormatResponse("❌ No target", "No alive terrorist reports available.");

            var data = DeserializeArray(report.GetInfoJson());
            return FormatResponse("🎯 Target Found", $"Most dangerous terrorist: {report.GetTerrorist().Name}", data);
        }

        //--------------------------------------------------------------
        public string EliminateById(int id)
        {
            var report = _idf.Intelligence.GetById(id);
            if (report == null)
                return FormatResponse("❌ Elimination failed", $"No intelligence report found for terrorist ID: {id}");

            var result = TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation());
            return Serialize(result);
        }

        //--------------------------------------------------------------
        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            if (report == null)
                return FormatResponse("❌ Elimination failed", "No alive intelligence reports available.");

            var result = TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation());
            return Serialize(result);
        }

        //--------------------------------------------------------------
        public string EliminateByTargetType(string targetType)
        {
            var normalized = targetType.Trim().ToLower();

            var reports = _idf.Intelligence.GetReports()
                .Where(r => r.GetLastKnownLocation().Trim().ToLower() == normalized)
                .ToList();

            if (!reports.Any())
                return FormatResponse("❌ Elimination failed", $"No eligible terrorists found for target type: {targetType}");

            var results = reports
                .Select(r => TryEliminate(r.GetTerrorist(), normalized))
                .ToList();

            return Serialize(results, escape: false);
        }

        //--------------------------------------------------------------
        private object[] TryEliminate(Terrorist terrorist, string targetType)
        {
            var report = _idf.Intelligence.GetById(terrorist.Id);
            var data = report != null ? DeserializeArray(report.GetInfoJson()) : new object[0];

            if (report == null)
                return new object[] { "❌ Elimination failed", $"No report found for terrorist ID: {terrorist.Id}", data };

            if (!terrorist.IsAlive)
                return new object[] { "❌ Elimination failed", $"Target already dead | Target: {terrorist.Name}", data };

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
                return new object[] { "❌ Elimination failed", $"No weapon available for target type: {targetType} | Target: {terrorist.Name}", data };

            weapon.AttackTarget(terrorist);
            return new object[] { "✅ Elimination successful", $"Weapon: {weapon.GetType().Name} | Target: {terrorist.Name}", data };
        }

        //--------------------------------------------------------------
        private static object[] DeserializeArray(string json)
        {
            return JsonSerializer.Deserialize<object[]>(json) ?? new object[0];
        }

        //--------------------------------------------------------------
        private static string Serialize(object obj, bool escape = true)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = escape ? JavaScriptEncoder.Default : JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        //--------------------------------------------------------------
        private static string FormatResponse(string status, string message, object data = null!)
        {
            return Serialize(new object[] { status, message, data ?? new object[0] });
        }
    }
}
