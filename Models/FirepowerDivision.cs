
using System;
using System.Collections.Generic;
using System.Text;
using IdfOperation.GoodGuys.Firepower;
using IdfOperation.BadGuys;

namespace IdfOperation.GoodGuys.Firepower
{
    public class FirepowerDivision
    {
        private readonly Dictionary<string, List<Weapon>> _weaponsByTarget;

        //====================================
        public FirepowerDivision()
        {
            _weaponsByTarget = new Dictionary<string, List<Weapon>>();

            AddF16(5);
            AddEyeFire(2);
            AddZik(8);
            AddTank(3);
        }

        //--------------------------------------------------------------
        private void AddF16(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var f16 = new F16(i + 1);
                MapWeaponToTargets(f16);
            }
        }

        //--------------------------------------------------------------
        private void AddZik(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var zik = new Zik(i + 1);
                MapWeaponToTargets(zik);
            }
        }

        //--------------------------------------------------------------
        private void AddTank(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var tank = new Tank(i + 1);
                MapWeaponToTargets(tank);
            }
        }

        //--------------------------------------------------------------
        private void AddEyeFire(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var eyeFire = new EyeFire(i + 1);
                MapWeaponToTargets(eyeFire);
            }
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

                if (weapon is IFuelable fuelable && fuelable.GetFuel() <= 0)
                    continue;

                return weapon;
            }

            return null;
        }

        //--------------------------------------------------------------
        public string GetInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== Firepower Table ===");
            sb.AppendLine("Name | Ammo | Targets | Fuel");

            var added = new HashSet<Weapon>();
            foreach (var kvp in _weaponsByTarget)
            {
                foreach (var weapon in kvp.Value)
                {
                    if (added.Add(weapon))
                        sb.AppendLine(weapon.GetInfo());
                }
            }

            return sb.ToString();
        }

    }
}
