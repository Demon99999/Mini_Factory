using System.Collections.Generic;
using Project.Scripts.Config;

namespace Project.Scripts.StaticData
{
    public interface IStaticDataService
    {
        MachinesConfig MachinesConfig { get; }
        BoostConfig BoostConfig { get; }
        OfflineConfig OfflineConfig { get; }

        MachineConfig GetMachine(string id);
        IReadOnlyList<MachineConfig> GetAllMachines();
        void Initialize();
    }
}