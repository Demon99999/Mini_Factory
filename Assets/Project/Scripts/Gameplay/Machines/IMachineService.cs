using System;
using System.Collections.Generic;
using Project.Scripts.Config;
using Project.Scripts.Progress;

namespace Project.Scripts.Gameplay.Machines
{
    public interface IMachineService
    {
        IReadOnlyList<MachineProgress> States { get; }

        event Action OnStateChanged;
        event Action<MachineProgress> OnMachineUnlocked;
        event Action<MachineProgress> OnMachineUpgraded;

        MachineConfig GetConfig(string machineId);
        double GetProductionPerSecond(MachineProgress state);
        double GetTotalProduction();
        double GetUpgradeCost(MachineProgress state);
        double GetUnlockCost(string machineId);
        bool CanUnlock(string machineId);
        bool CanUpgrade(string machineId);
        bool TryUnlock(string machineId);
        bool TryUpgrade(string machineId);
    }
}