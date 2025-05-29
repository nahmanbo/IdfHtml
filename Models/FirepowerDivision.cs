using System.Text.Json;

namespace IdfOperation.Web.Models
{
    public class FirepowerDivision
    {
        private readonly Dictionary<string, List<Weapon>> _weaponsByTarget = new();

        //====================================
        public FirepowerDivision()
        {
            AddF16(5);
            AddEyeFire(2);
            AddZik(8);
            AddTank(3);
        }

        //--------------------------------------------------------------
        private void AddF16(int count) => AddWeapons(count, i => new F16(i + 1));
        private void AddEyeFire(int count) => AddWeapons(count, i => new EyeFire(i + 1));
        private void AddZik(int count) => AddWeapons(count, i => new Zik(i + 1));
        private void AddTank(int count) => AddWeapons(count, i => new Tank(i + 1));

        //--------------------------------------------------------------
        private void AddWeapons(int count, Func<int, Weapon> factory)
        {
            for (int i = 0; i < count; i++)
                MapWeaponToTargets(factory(i));
        }

        //--------------------------------------------------------------
        private void MapWeaponToTargets(Weapon weapon)
        {
            foreach (var target in weapon.GetTargetTypes())
            {
                if (!_weaponsByTarget.ContainsKey(target))
                    _weaponsByTarget[target] = new List<Weapon>();

                _weaponsByTarget[target].Add(weapon);
            }
        }

        //--------------------------------------------------------------
        public IReadOnlyDictionary<string, List<Weapon>> GetAllWeaponsByTarget()
        {
            return _weaponsByTarget;
        }

        //--------------------------------------------------------------
        public Weapon? FindAvailableWeaponFor(string targetType)
        {
            if (!_weaponsByTarget.ContainsKey(targetType))
                return null;

            foreach (var weapon in _weaponsByTarget[targetType])
            {
                if (weapon.GetAmmo() <= 0)
                    continue;

                if (weapon is IFuelable f && f.GetFuel() <= 0)
                    continue;

                return weapon;
            }

            return null;
        }

        //--------------------------------------------------------------
        public object[] GetInfo()
        {
            var header = "IDF - Firepower Division";
            var description = "Weapons categorized by target type";

            var categorized = _weaponsByTarget
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(w => JsonSerializer.Deserialize<object>(w.GetInfoJson())).ToList());


            return new object[] { header, description, categorized };
        }
        
    }
}
