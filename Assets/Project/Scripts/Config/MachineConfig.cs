using System;

namespace Project.Scripts.Config
{
    [Serializable]
    public class MachineConfig
    {
        public string Id;
        public string DisplayName;
        public double BaseProductionPerSecond;
        public double ProductionPerLevel;
        public double UnlockCost;
        public double UpgradeCostBase;
        public double UpgradeCostMultiplier;
        public bool UnlockedByDefault;
        public int MaxLevel = 50;
    }
}