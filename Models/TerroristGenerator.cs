using System.Text.Json;
using IdfOperation.Factory;

namespace IdfOperation.BadGuys
{
    public static class TerroristGenerator
    {
        //==============================================================
        public static string PromptTerrorist =
            "החזר לי מערך של אובייקטי JSON בלבד, ללא טקסט נוסף, שכל אובייקט יכיל את השדות הבאים:\r\n" +
            "- Name (string): באנגלית שם פרטי ושם משפחה מוסלמים או חמאסיים מגוונים (לא חוזרים על עצמם)\r\n" +
            "- Id (int): מספר אקראי בן 6 ספרות, לא עוקב\r\n" +
            "- Rank (int)\r\n" +
            "- IsAlive (true)\r\n" +
            "- Weapons (string[]): בין 1 ל־4 מהנשקים הבאים: Knife, Gun, M16, AK47\r\n\r\n" +
            "החזר מערך JSON בלבד (ללא טקסט מקדים), כדי שאוכל להמיר אותו ישירות לרשימת אובייקטים בשפת #C.\r\n" +
            "מספר האובייקטים הרצוי הוא: ";

        //==============================================================
        public static async Task<List<Terrorist>> Generate(int count)
        {
            Console.WriteLine("📡 Requesting terrorist data from OpenAI...");

            try
            {
                string prompt = PromptTerrorist + count;
                string json = await AiFactory.RequestOpenAi(prompt);

                var terrorists = JsonSerializer.Deserialize<List<Terrorist>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

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