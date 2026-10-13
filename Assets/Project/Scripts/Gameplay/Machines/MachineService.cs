using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Config;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Progress;
using Project.Scripts.StaticData;

namespace Project.Scripts.Gameplay.Machines
{
    public class MachineService : IMachineService
    {
        private readonly IEconomyService _economy;
        private readonly IPersistentProgress _progress;
        private readonly Dictionary<string, MachineConfig> _configsById;

        public IReadOnlyList<MachineProgress> States => _progress.Progress.Machines;

        public event Action OnStateChanged;
        public event Action<MachineProgress> OnMachineUnlocked;
        public event Action<MachineProgress> OnMachineUpgraded;

        public MachineService(IStaticDataService staticData, IEconomyService economy, IPersistentProgress progress)
        {
            _economy = economy;
            _progress = progress;
            _configsById = staticData.GetAllMachines().ToDictionary(m => m.Id, m => m);
        }

        public MachineConfig GetConfig(string machineId)
            => _configsById.TryGetValue(machineId, out var c) ? c : null;
        
        public double GetProductionPerSecond(MachineProgress state)
        {
            if (state == null || !state.Unlocked) return 0;
            var c = _configsById[state.Id];
            return c.BaseProductionPerSecond + c.ProductionPerLevel * state.Level;
        }

        public double GetTotalProduction()
            => States.Where(s => s.Unlocked).Sum(GetProductionPerSecond);

        public double GetUpgradeCost(MachineProgress state)
        {
            var c = _configsById[state.Id];
            return c.UpgradeCostBase * Math.Pow(c.UpgradeCostMultiplier, state.Level);
        }

        public double GetUnlockCost(string machineId)
            => _configsById[machineId].UnlockCost;

        public bool CanUnlock(string machineId)
        {
            var s = Get(machineId);
            if (s == null || s.Unlocked) return false;
            return _economy.Balance >= GetUnlockCost(machineId);
        }

        public bool CanUpgrade(string machineId)
        {
            var s = Get(machineId);
            if (s == null || !s.Unlocked) return false;
            var c = _configsById[machineId];
            if (s.Level >= c.MaxLevel) return false;
            return _economy.Balance >= GetUpgradeCost(s);
        }

        public bool TryUnlock(string machineId)
        {
            if (!CanUnlock(machineId)) return false;
            if (!_economy.TrySpend(GetUnlockCost(machineId))) return false;

            var s = Get(machineId);
            s.Unlocked = true;

            OnMachineUnlocked?.Invoke(s);
            OnStateChanged?.Invoke();
            return true;
        }

        public bool TryUpgrade(string machineId)
        {
            if (!CanUpgrade(machineId)) return false;
            var s = Get(machineId);
            if (!_economy.TrySpend(GetUpgradeCost(s))) return false;

            s.Level++;

            OnMachineUpgraded?.Invoke(s);
            OnStateChanged?.Invoke();
            return true;
        }
        
        private MachineProgress Get(string id)
            => _progress.Progress.Machines.FirstOrDefault(m => m.Id == id);
    }
}