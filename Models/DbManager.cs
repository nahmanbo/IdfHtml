using IdfOperation;
using IdfOperation.BadGuys;
using IdfOperation.GoodGuys.Intelligence;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace IdfOperation.Factory
    {
    public static class DbManager
{
    //-------------------------------------------------------------------------------
    public static List<Terrorist> LoadTerroristsFromDB()
    {
        string file = File.ReadAllText(Constants.Paths.Terrorists);

        List<Terrorist> terrorists = JsonSerializer.Deserialize<List<Terrorist>>(file)!;

        return terrorists;

    }
    //---------------------------------------------------------------------------------

    private static void WriteTerroristsToDB(List<Terrorist> terrorists)
    {
        string str = JsonSerializer.Serialize(terrorists, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(Constants.Paths.Terrorists, str);
    }

    //-----------------------------------------------------------------------------------

    public static List<Terrorist> GetTerroristsFromDB()
    {
  

        List<Terrorist> terrorists = new List<Terrorist>();
        terrorists = LoadTerroristsFromDB();


            /*
        try
        {
            terrorists = LoadTerroristsFromDB();

        }
        catch
        {
            Console.WriteLine("בעיה בטעינה : נטענה רשימה ריקה");
        }
*/
        return terrorists;
    }

    //---------------------------------------------------------------
    public static void UpdateTerroristInDB(Terrorist terorrist)
    {

        try
        {

            

           List<Terrorist> terrorists = LoadTerroristsFromDB();

           List<Terrorist> new_list = terrorists.Select(t => t.Id == terorrist.Id ? terorrist : t).ToList();

           WriteTerroristsToDB(new_list);
           //UpdateIntelligenceReportInDB(terorrist);



        }
        catch
        {
            Console.WriteLine("עדכון בדאטה בייס נכשל");
        }


    }

    //------------------------------------------------------------------
    public static void AddTerroristToDB(Terrorist terorrist)
    {
        try
        {
            List<Terrorist> terrorists = LoadTerroristsFromDB();

            terrorists.Add(terorrist);

            WriteTerroristsToDB(terrorists);
        }
        catch
        {
            Console.WriteLine("עדכון בדאטה בייס נכשל");
        }
    }



    //-----------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------

    public static List<IntelligenceReport> LoadIntelligenceReportsFromDB()
    {

        string file = File.ReadAllText(Constants.Paths.IntelligenceReports);
           
        List<IntelligenceReport> IntelligenceReports = JsonSerializer.Deserialize<List<IntelligenceReport>>(file)!;
           
        return IntelligenceReports;

    }
    //---------------------------------------------------------------------------------

    private static void WriteIntelligenceReportsToDB(List<IntelligenceReport> IntelligenceReports)
    {
        string str = JsonSerializer.Serialize(IntelligenceReports, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(Constants.Paths.IntelligenceReports, str);
    }

    //-----------------------------------------------------------------------------------
    public static List<IntelligenceReport> GetIntelligenceReportFromDB()
    {

        List<IntelligenceReport> IntelligenceReports = new List<IntelligenceReport>();
        IntelligenceReports = LoadIntelligenceReportsFromDB();

/*
        try
        {
            IntelligenceReports = LoadIntelligenceReportsFromDB();
        }
        catch
        {
            Console.WriteLine("בעיה בטעינה : נטענה רשימה ריקה");
        }*/

        return IntelligenceReports;
    }
        //--------------------------------------------------------------------------------------------------
        public static void AddIntelligenceReportToDB(IntelligenceReport intelligenceReport)
        {
            try
            {
                List<IntelligenceReport> IntelligenceReports = LoadIntelligenceReportsFromDB();

                IntelligenceReports.Add(intelligenceReport);

                WriteIntelligenceReportsToDB(IntelligenceReports);
            }
            catch
            {
                Console.WriteLine("הוספה נכשלה");
            }

        }
        //------------------------------------------------------------------------------

        /*
        public static void UpdateIntelligenceReportInDB(Terrorist terrorist)
    {
        try
        {
            List<IntelligenceReport> IntelligenceReports = LoadIntelligenceReportsFromDB();

               IntelligenceReports.Find(i => i._terrorist.Id == terrorist.Id)!._terrorist=terrorist;

            WriteIntelligenceReportsToDB(IntelligenceReports);
        }
        catch
        {
            Console.WriteLine("עדכון נכשל");
        }*/

    }

    
}