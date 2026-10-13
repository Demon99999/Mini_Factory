using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Config;
using Project.Scripts.Infrastructure.AssetManagement;

namespace Project.Scripts.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly IAssetProvider _assets;
        private Dictionary<string, MachineConfig> _machinesById;

        public MachinesConfig MachinesConfig { get; private set; }
        public BoostConfig BoostConfig { get; private set; }
        public OfflineConfig OfflineConfig { get; private set; }

        public StaticDataService(IAssetProvider assets)
        {
            _assets = assets;
        }

        public void Initialize()
        {
            MachinesConfig = _assets.Load<MachinesConfig>(StaticDataConfigsPath.MachinesConfig);
            _machinesById = MachinesConfig.Machines.ToDictionary(m => m.Id, m => m);

            BoostConfig = _assets.Load<BoostSettingsConfig>(StaticDataConfigsPath.BoostSettings).Boost;
            OfflineConfig = _assets.Load<OfflineSettingsConfig>(StaticDataConfigsPath.OfflineSettings).Offline;
        }

        public MachineConfig GetMachine(string id)
            => _machinesById.TryGetValue(id, out var m) ? m : null;
    }
}