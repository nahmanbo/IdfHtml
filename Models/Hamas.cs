using System.Text.Json;
using IdfOperation.BadGuys;
using IdfOperation.Factory;
using IdfOperation.Organizations;

namespace IdfOperation.BadGuys
{
    public class Hamas : Organization
    {
        private static Hamas? _instance;
        private readonly List<Terrorist> _terrorists;

        //====================================
        public static Hamas Instance => _instance!;

        //====================================
        public Hamas(string currentCommander)
            : base(new DateTime(1987, 12, 14), currentCommander)
        {
            _terrorists = TerroristManager.GetOrGenerateAsync().Result;
            _instance = this;
        }

        //--------------------------------------------------------------
        public IReadOnlyList<Terrorist> GetTerrorists()
        {
            return _terrorists.AsReadOnly();
        }
        
        //--------------------------------------------------------------
        public override string GetInfo()
        {
            return JsonSerializer.Serialize(_terrorists, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}