using IdfOperation;
using IdfOperation.BadGuys;
using IdfOperation.GoodGuys.Intelligence;
using System.Text.Json;

namespace IdfOperation.Factory
{
    public static class DbManager
    {
        //==============================================================
        public static List<Terrorist> LoadTerrorists()
        {
            string file = File.ReadAllText(Constants.Paths.Terrorists);
            return JsonSerializer.Deserialize<List<Terrorist>>(file)!;
        }

        //--------------------------------------------------------------
        public static void SaveTerroristsToDB(List<Terrorist> terrorists)
        {
            string str = JsonSerializer.Serialize(terrorists, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.Paths.Terrorists, str);
        }

        //--------------------------------------------------------------
        public static void UpdateTerroristInDB(Terrorist terrorist)
        {
            try
            {
                List<Terrorist> terrorists = LoadTerrorists();
                List<Terrorist> newList = terrorists.Select(t => t.Id == terrorist.Id ? terrorist : t).ToList();
                SaveTerroristsToDB(newList);
            }
            catch
            {
                Console.WriteLine("עדכון בדאטה בייס נכשל");
            }
        }

        //--------------------------------------------------------------
        public static void AddTerroristToDB(Terrorist terrorist)
        {
            try
            {
                List<Terrorist> terrorists = LoadTerrorists();
                terrorists.Add(terrorist);
                SaveTerroristsToDB(terrorists);
            }
            catch
            {
                Console.WriteLine("עדכון בדאטה בייס נכשל");
            }
        }

        //==============================================================
        public static List<IntelligenceReport> LoadIntelligenceReports()
        {
            string file = File.ReadAllText(Constants.Paths.IntelligenceReports);
            return JsonSerializer.Deserialize<List<IntelligenceReport>>(file)!;
        }

        //--------------------------------------------------------------
        private static void SaveIntelligenceReportsToDB(List<IntelligenceReport> reports)
        {
            string str = JsonSerializer.Serialize(reports, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.Paths.IntelligenceReports, str);
        }

        //--------------------------------------------------------------
        public static List<IntelligenceReport> GetIntelligenceReportFromDB()
        {
            return LoadIntelligenceReports();
        }

        //--------------------------------------------------------------
        public static void AddIntelligenceReportToDB(IntelligenceReport report)
        {
            try
            {
                List<IntelligenceReport> reports = LoadIntelligenceReports();
                reports.Add(report);
                SaveIntelligenceReportsToDB(reports);
            }
            catch
            {
                Console.WriteLine("הוספה נכשלה");
            }
        }
    }
}
