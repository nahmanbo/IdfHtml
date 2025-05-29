using System.Text.Json;

namespace IdfOperation.Web.Models
{
    public static class TerroristManager
    {
        //====================================
        private const int MinAliveTerrorists = 20;
        
        //--------------------------------------------------------------
        public static async Task<List<Terrorist>> GetOrGenerateAsync()
        {
            List<Terrorist> terrorists;

            try
            {
                terrorists = DbManager.LoadTerrorists();
            }
            catch
            {
                Console.WriteLine("⚠️ שגיאה בטעינת מחבלים. תתבצע יצירה חדשה.");
                terrorists = new List<Terrorist>();
            }

            int alive = terrorists.Count(t => t.IsAlive);

            if (alive < MinAliveTerrorists)
            {
                int toGenerate = MinAliveTerrorists - alive;
                var newTerrorists = await Generate(toGenerate);
                terrorists.AddRange(newTerrorists);
                DbManager.SaveTerroristsToDB(terrorists);
            }

            return terrorists;
        }

        //--------------------------------------------------------------
        public static async Task<List<Terrorist>> Generate(int count)
        {
            Console.WriteLine("📡 Requesting terrorist data from OpenAI...");

            try
            {
                string prompt = Constants.Prompts.Terrorist + count;
                string json = await AiFactory.RequestOpenAi(prompt);

                var terrorists = JsonSerializer.Deserialize<List<Terrorist>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (terrorists == null)
                    throw new Exception("OpenAI returned null or empty array.");

                return terrorists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to parse terrorists: {ex.Message}");
                return new List<Terrorist>();
            }
        }
    }
}
