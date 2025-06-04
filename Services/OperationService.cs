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
        public string ViewFullIdfInfo() => _idf.GetInfo();

        //--------------------------------------------------------------
        public string ViewFullHamasInfo() => _hamas.GetInfo();

        //--------------------------------------------------------------
        public string ViewFirepowerData() => Serialize(_idf.Firepower.GetInfo());

        //--------------------------------------------------------------
        public string ViewIntelligenceReports() => Serialize(_idf.Intelligence.GetInfo());

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
                return FormatResponse("❌ Elimination failed", $"We couldn’t find an intelligence report for terrorist ID {id}.");

            var result = TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation());
            return Serialize(result);
        }

        //--------------------------------------------------------------
        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            if (report == null)
                return FormatResponse("❌ Elimination failed", "No alive terrorist reports available.");

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

            if (normalized is "buildings" or "open areas")
            {
                var batchList = reports
                    .Select(r => new { Id = r.GetTerrorist().Id, Target = normalized })
                    .ToList();

                return Serialize(new object[]
                {
                    "🕓 Batch Ammo Input Required",
                    $"Multiple targets found for '{normalized}', ammo input required per target",
                    batchList
                });
            }

            var results = reports
                .Select(r => TryEliminate(r.GetTerrorist(), normalized))
                .ToList();

            return Serialize(results, escape: false);
        }

        //--------------------------------------------------------------
        public string ExecuteStrikeWithAmmo(JsonElement payload)
        {
            var id = payload.GetProperty("Id").GetInt32();
            var target = payload.GetProperty("Target").GetString()?.Trim().ToLower() ?? "";
            var ammo = payload.GetProperty("Ammo").GetDouble();

            var terrorist = _idf.Intelligence.GetById(id)?.GetTerrorist();
            if (terrorist == null)
                return Serialize(new object[] { "❌", "Terrorist not found", Array.Empty<object>() });

            var result = TryEliminate(terrorist, target, ammo);
            return Serialize(result);
        }

        //--------------------------------------------------------------
        private object[] TryEliminate(Terrorist terrorist, string targetType, double? ammo = null)
        {
            var report = _idf.Intelligence.GetById(terrorist.Id);
            var data = report != null ? DeserializeArray(report.GetInfoJson()) : Array.Empty<object>();

            if (report == null)
                return new object[]
                {
                    "❌ Elimination failed",
                    $"We couldn’t find an intelligence report for terrorist ID {terrorist.Id}.",
                    data
                };

            if (!terrorist.IsAlive)
                return new object[]
                {
                    "❌ Elimination failed",
                    $"Target '{terrorist.Name}' has already been eliminated.",
                    data
                };

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
                return new object[]
                {
                    "❌ Elimination failed",
                    $"No available weapon found for target type '{targetType}'. Cannot eliminate '{terrorist.Name}'.",
                    data
                };

            if ((targetType is "buildings" or "open areas") && ammo == null)
                return new object[]
                {
                    "🕓 Ammo Input Required",
                    $"Please choose how much ammo to use for target type '{targetType}'.",
                    new { Target = targetType, Id = terrorist.Id }
                };

            try
            {
                weapon.AttackTarget(terrorist, ammo ?? 1);
            }
            catch (Exception ex)
            {
                return new object[]
                {
                    "❌ Attack failed",
                    $"Something went wrong during the attack: {ex.Message}",
                    data
                };
            }

            return new object[]
            {
                "✅ Elimination successful",
                $"The weapon '{weapon.GetType().Name}' successfully eliminated terrorist '{terrorist.Name}' using {ammo ?? 1} ammo.",
                data
            };
        }

        //--------------------------------------------------------------
        private static object[] DeserializeArray(string json) =>
            JsonSerializer.Deserialize<object[]>(json) ?? Array.Empty<object>();

        //--------------------------------------------------------------
        private static string Serialize(object obj, bool escape = true) =>
            JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = escape ? JavaScriptEncoder.Default : JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        //--------------------------------------------------------------
        private static string FormatResponse(string status, string message, object data = null) =>
            Serialize(new object[] { status, message, data ?? Array.Empty<object>() });
    }
} 