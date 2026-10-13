using System;
using System.Collections.Generic;
using Project.Scripts.Config;

namespace Project.Scripts.Progress
{
    [Serializable]
    public class PlayerProgress
    {
        public double Balance;
        public long LastExitUtcTicks;
        public long BoostEndUtcTicks;
        public List<MachineProgress> Machines = new List<MachineProgress>();

        public PlayerProgress() { }

        public PlayerProgress(IEnumerable<MachineConfig> machines, double startBalance = 0)
        {
            Balance = startBalance;
            foreach (var m in machines)
            {
                Machines.Add(new MachineProgress
                {
                    Id = m.Id,
                    Unlocked = m.UnlockedByDefault,
                    Level = 0
                });
            }
        }
    }
}