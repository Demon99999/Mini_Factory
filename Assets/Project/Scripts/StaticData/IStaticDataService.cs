using Project.Scripts.Config;

namespace Project.Scripts.StaticData
{
    public interface IStaticDataService
    {
        MachinesConfig MachinesConfig { get; }
        BoostConfig BoostConfig { get; }
        OfflineConfig OfflineConfig { get; }

        MachineConfig GetMachine(string id);
        void Initialize();
    }
}