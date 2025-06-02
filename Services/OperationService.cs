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
            return JsonSerializer.Serialize(_idf.Firepower.GetInfo(), new JsonSerializerOptions { WriteIndented = true });
        }

        //--------------------------------------------------------------
        public string ViewIntelligenceReports()
        {
            return JsonSerializer.Serialize(_idf.Intelligence.GetInfo(), new JsonSerializerOptions { WriteIndented = true });
        }


        //--------------------------------------------------------------
        public string ViewReportById(int id)
        {
            var report = _idf.Intelligence.GetById(id);

            if (report == null)
            {
                return JsonSerializer.Serialize(new object[]
                {
                    "❌ Report not found",
                    $"No report found for terrorist ID: {id}",
                    new object[0]
                }, new JsonSerializerOptions { WriteIndented = true });
            }

            var reportData = JsonSerializer.Deserialize<object[]>(report.GetInfoJson());

            return JsonSerializer.Serialize(new object[]
            {
                "📄 Report found",
                $"Terrorist ID: {id}",
                reportData
            }, new JsonSerializerOptions { WriteIndented = true });
        }


        //--------------------------------------------------------------
        public string ViewMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();

            if (report == null)
            {
                return JsonSerializer.Serialize(new object[]
                {
                    "❌ No target",
                    "No alive terrorist reports available.",
                    new object[0]
                }, new JsonSerializerOptions { WriteIndented = true });
            }

            var reportData = JsonSerializer.Deserialize<object[]>(report.GetInfoJson());

            return JsonSerializer.Serialize(new object[]
            {
                "🎯 Target Found",
                $"Most dangerous terrorist: {report.GetTerrorist().Name}",
                reportData
            }, new JsonSerializerOptions { WriteIndented = true });
        }

        //--------------------------------------------------------------
        public string EliminateById(int id)
        {
            var report = _idf.Intelligence.GetById(id);
            return report != null
                ? JsonSerializer.Serialize(TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation()), new JsonSerializerOptions { WriteIndented = true })
                : JsonSerializer.Serialize(new object[]
                {
                    "❌ Elimination failed",
                    $"No intelligence report found for terrorist ID: {id}",
                    new object[0]
                }, new JsonSerializerOptions { WriteIndented = true });
        }

        //--------------------------------------------------------------
        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            return report != null
                ? JsonSerializer.Serialize(TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation()), new JsonSerializerOptions { WriteIndented = true })
                : JsonSerializer.Serialize(new object[]
                {
                    "❌ Elimination failed",
                    "No alive intelligence reports available.",
                    new object[0]
                }, new JsonSerializerOptions { WriteIndented = true });
        }

        //--------------------------------------------------------------
        public string EliminateByTargetType(string targetType)
        {
            var normalizedTarget = targetType.Trim().ToLower();
            var reports = _idf.Intelligence.GetReports()
                .Where(r => r.GetLastKnownLocation().Trim().ToLower() == normalizedTarget)
                .ToList();

            if (!reports.Any())
            {
                return JsonSerializer.Serialize(new object[]
                {
                    "❌ Elimination failed",
                    $"No eligible terrorists found for target type: {targetType}",
                    new object[0]
                }, new JsonSerializerOptions { WriteIndented = true });
            }

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
        private object[] TryEliminate(Terrorist terrorist, string targetType)
        {
            var report = _idf.Intelligence.GetById(terrorist.Id);
            var data = report != null ? JsonSerializer.Deserialize<object>(report.GetInfoJson()) : null;

            if (report == null)
            {
                return new object[]
                {
                    "❌ Elimination failed",
                    $"No report found for terrorist ID: {terrorist.Id}",
                    new object[0]
                };
            }

            if (!terrorist.IsAlive)
            {
                return new object[]
                {
                    "❌ Elimination failed",
                    $"Target already dead | Target: {terrorist.Name}",
                    data ?? new object[0]
                };
            }

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
            {
                return new object[]
                {
                    "❌ Elimination failed",
                    $"No weapon available for target type: {targetType} | Target: {terrorist.Name}",
                    data ?? new object[0]
                };
            }

            weapon.AttackTarget(terrorist);

            return new object[]
            {
                "✅ Elimination successful",
                $"Weapon: {weapon.GetType().Name} | Target: {terrorist.Name}",
                data ?? new object[0]
            };
        }
    }
}
