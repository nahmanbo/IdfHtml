public class AttackLog
{
    public DateTime Time { get; set; }
    public string AryaType { get; set; }
    public string WeaponName { get; set; }
    public string TerroristName { get; set; }
    
    public AttackLog(DateTime Time , string AryaType, string WeaponName , string TerroristName)
    {
        this.Time = Time ;
        this.AryaType = AryaType;
        this.WeaponName = WeaponName ;
        this.TerroristName = TerroristName ;
    }


    public static AttackLog Create(string str)
    {
        string[] logs= str.Split(':');
        AttackLog log = new AttackLog(DateTime.Parse(logs[0]), logs[1], logs[2], logs[3]);
        return log;
    }


}