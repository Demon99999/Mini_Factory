using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Config;
using Project.Scripts.StaticData;

namespace MiniFactory.Tests.EditMode.Project.Tests.EditMode
{
    public class FakeStaticData : IStaticDataService
    {
        private readonly List<MachineConfig> _machines;
        private readonly OfflineConfig _offline;
        private readonly BoostConfig _boost;

        public MachinesConfig MachinesConfig => null;
        public BoostConfig BoostConfig => _boost;
        public OfflineConfig OfflineConfig => _offline;

        public FakeStaticData(
            List<MachineConfig> machines = null,
            BoostConfig boost = null,
            OfflineConfig offline = null)
        {
            _machines = machines ?? new List<MachineConfig>();
            _boost = boost ?? new BoostConfig
            {
                Enabled = true,
                DurationSeconds = 30f,
                Multiplier = 2f
            };
            _offline = offline ?? new OfflineConfig
            {
                MaxOfflineSeconds = 3600
            };
        }

        public MachineConfig GetMachine(string id)
            => _machines.FirstOrDefault(m => m.Id == id);

        public IReadOnlyList<MachineConfig> GetAllMachines()
            => _machines;

        public void Initialize() { }
    }
}