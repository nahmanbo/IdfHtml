namespace IdfOperation
{
    public static class Constants
    {
        // נתיבי קבצים
        public static class Paths
        {
            public static readonly string Terrorists = @"DB/Terroristes.json";
            public const string IntelligenceReports = @"DB/Reports.json";
        }

        // API Tokens / Keys (לא מומלץ לשים כאן מפתחות אמיתיים בקוד פתוח!)
        public static class Api
        {
            public const string ApiOpenAi = "env:OPENAI_KEY"; // או למשוך מה-Environment
        }

        // פרומפטים
        public static class Prompts
        {
            public const string GenerateReport = "צור דוח מודיעין על בסיס הנתונים הבאים:";
            public const string Terrorist =
           "החזר לי מערך של אובייקטי JSON בלבד, ללא טקסט נוסף, שכל אובייקט יכיל את השדות הבאים:\r\n" +
           "- Name (string): באנגלית שם פרטי ושם משפחה מוסלמים או חמאסיים מגוונים (לא חוזרים על עצמם)\r\n" +
           "- Id (int): מספר אקראי בן 6 ספרות, לא עוקב רנדומלי לחלוטין\r\n" +
           "- Rank (int) בין 1 ל5 בלבד\r\n" +
           "- IsAlive (true)\r\n" +
           "- Weapons (string[]): בין 1 ל־4 מהנשקים הבאים: Knife, Gun, M16, AK47\r\n\r\n" +
           "החזר מערך JSON בלבד (ללא טקסט מקדים), כדי שאוכל להמיר אותו ישירות לרשימת אובייקטים בשפת #C.\r\n" +
           "מספר האובייקטים הרצוי הוא: ";
        }

        
    }
}
