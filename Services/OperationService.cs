using IdfOperation.GoodGuys;
using IdfOperation.BadGuys;
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
        //--------------------------------------------------------------

        
//--------------------------------------------------------------
        //--------------------------------------------------------------
        //--------------------------------------------------------------
        private string TryEliminate(Terrorist terrorist, string targetType)
        {
            var report = _idf.Intelligence.GetById(terrorist.Id);
            object[] v;

            if (report == null)
            {
                v = new object[]
                {
                    "❌ Elimination failed",
                    $"No report found for terrorist ID: {terrorist.Id}",
                    null!
                };
                return JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true });
            }

            var reportData = JsonSerializer.Deserialize<object>(report.GetInfoJson());

            if (!terrorist.IsAlive)
            {
                v = new object[]
                {
                    "❌ Elimination failed",
                    $"Target already dead | Target: {terrorist.Name}",
                    reportData
                };
                return JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true });
            }

            var weapon = _idf.Firepower.FindAvailableWeaponFor(targetType);
            if (weapon == null)
            {
                v = new object[]
                {
                    "❌ Elimination failed",
                    $"No weapon available for target type: {targetType} | Target: {terrorist.Name}",
                    reportData
                };
                return JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true });
            }

            weapon.AttackTarget(terrorist);

            var header = terrorist.IsAlive
                ? "❌ Elimination failed"
                : "✅ Elimination successful";

            var subHeader = $"Weapon: {weapon.GetType().Name} | Target: {terrorist.Name}";

            v = new object[]
            {
                header,
                subHeader,
                reportData
            };

            return JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true });
        }





        //--------------------------------------------------------------
        public string EliminateById(int id)
        {
            var report = _idf.Intelligence.GetById(id);
            if (report == null)
                return $"No intelligence report found for terrorist ID: {id}";

            var result = TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation());

        return result;
        }


        //--------------------------------------------------------------
        public string EliminateMostDangerous()
        {
            var report = _idf.Intelligence.GetMostDangerous();
            if (report == null)
                return "No alive intelligence reports available.";

            var result = TryEliminate(report.GetTerrorist(), report.GetLastKnownLocation());

            return result;
        }


        //--------------------------------------------------------------
        public string EliminateByTargetType(string targetType)
        {
            /*
                targetType = targetType.Trim().ToLower();
                var results = new List<object[]>();

                foreach (var report in _idf.Intelligence.GetReports())
                {
                    if (report.GetLastKnownLocation().Trim().ToLower() != targetType)
                        continue;

                    var result = TryEliminate(report.GetTerrorist(), targetType);
                    if (result != null)
                        results.Add(result);
                }

                if (results.Count == 0)
                    return $"No eligible terrorists found for target type: {targetType}";

                return JsonSerializer.Serialize(results, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }*/
            return " aaa";
        }

    }
}
