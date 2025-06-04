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

            var requiresInput = normalized is "buildings" or "open areas";
            if (requiresInput)
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

            // fallback for all other target types — eliminate all at once
            var results = reports
                .Select(r => TryEliminate(r.GetTerrorist(), normalized))
                .ToList();

            return Serialize(results, escape: false);
        }


        //--------------------------------------------------------------
        public string ExecuteStrikeWithAmmo(StrikePayload payload)
        {
            var terrorist = _idf.Intelligence.GetById(payload.Id)?.GetTerrorist();
            if (terrorist == null)
                return Serialize(new object[] { "❌", "Terrorist not found", new object[0] });

            var result = TryEliminate(terrorist, payload.Target.Trim().ToLower(), payload.Ammo);
            return Serialize(result);
        }

        //--------------------------------------------------------------
        private object[] TryEliminate(Terrorist terrorist, string targetType, double? optionalWeight = null)
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

            var requiresInput = targetType is "buildings" or "open areas";
            if (requiresInput && optionalWeight == null)
            {
                return new object[]
                {
                    "🕓 Ammo Input Required",
                    $"Target type '{targetType}' requires selecting ammo amount (0.5 or 1)",
                    new { Target = targetType, Id = terrorist.Id }
                };
            }

            var weight = optionalWeight ?? 1;
            try
            {
                weapon.AttackTarget(terrorist, weight);
            }
            catch (Exception ex)
            {
                return new object[] { "❌ Attack failed", ex.Message, data };
            }

            return new object[]
            {
                "✅ Elimination successful",
                $"Weapon: {weapon.GetType().Name} | Target: {terrorist.Name} | Ammo Used: {weight}",
                data
            };
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

    //====================================
    public class StrikePayload
    {
        public string Target { get; set; } = "";
        public int Id { get; set; }
        public double Ammo { get; set; }
    }
}
