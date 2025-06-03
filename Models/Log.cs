

using IdfOperation;
using IdfOperation.BadGuys;
using IdfOperation.GoodGuys.Firepower;
using System.Collections.Generic;


public static class Log
{
    private static string path = Constants.Paths.Log;
    //----------------------------------------------------------
    private static void WriteLog(string log)
    {
        File.AppendAllText(path, log +"\r\n");
    }
    //-----------------------------------------------------------
    private static List<string> GetLogs()
    {
        return File.ReadAllLines(path).ToList();
    }
    //-----------------------------------------------------------
    //-----------------------------------------------------------

    public static void AddLogAttack(AttackLog attacklog)
    {
        string log = $"{attacklog.Time}:{attacklog.AryaType}:{attacklog.WeaponName}:{attacklog.TerroristName}";
        WriteLog(log);
    }

    public static List<AttackLog> GetAttackLog()
    {
        List<string> list = GetLogs();
        List<AttackLog> list1 = new List<AttackLog>();
        list.ForEach(log => list1.Add(AttackLog.Create(log)));
        return list1;
    }


}