namespace IdfOperation.BadGuys
{
    public class Terrorist
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Rank { get; set; }
        public bool IsAlive { get; set; }
        public List<string> Weapons { get; set; }

        //==============================================================
        public Terrorist(string name, int id, int rank, List<string> weapons)
        {
            Name = name;
            Id = id;
            Rank = rank;
            Weapons = weapons;
            IsAlive = true;
        }

        //--------------------------------------------------------------
        public string GetInfo()
        {
            string status = IsAlive ? "Alive" : "Dead";
            string weaponList = string.Join(", ", Weapons);
            return $"{Name} | {Id} | {Rank} | {status} | {weaponList}";
        }
        
        //--------------------------------------------------------------
        public void Kill()
        {
            IsAlive = false;
        }
    }
}