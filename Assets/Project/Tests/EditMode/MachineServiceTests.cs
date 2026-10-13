using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Project.Scripts.Config;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Gameplay.Machines;
using Project.Scripts.Progress;
using Project.Scripts.StaticData;

namespace MiniFactory.Tests.EditMode.Project.Tests.EditMode
{
    [TestFixture]
    public class MachineServiceTests
    {
        private class FakeStaticData : IStaticDataService
        {
            private readonly List<MachineConfig> _machines;

            public FakeStaticData(List<MachineConfig> machines) => _machines = machines;

            public MachinesConfig MachinesConfig => null;
            public BoostConfig BoostConfig => null;
            public OfflineConfig OfflineConfig => null;

            public MachineConfig GetMachine(string id)
                => _machines.FirstOrDefault(m => m.Id == id);

            public IReadOnlyList<MachineConfig> GetAllMachines() => _machines;

            public void Initialize() { }
        }

        private IEconomyService _economy;
        private IPersistentProgress _progress;
        private IMachineService _machines;

        [SetUp]
        public void SetUp()
        {
            var configs = new List<MachineConfig>
            {
                new MachineConfig
                {
                    Id = "m1",
                    BaseProductionPerSecond = 1,
                    ProductionPerLevel = 1,
                    UnlockCost = 0,
                    UpgradeCostBase = 10,
                    UpgradeCostMultiplier = 2,
                    UnlockedByDefault = true,
                    MaxLevel = 3
                },
                new MachineConfig
                {
                    Id = "m2",
                    BaseProductionPerSecond = 5,
                    ProductionPerLevel = 3,
                    UnlockCost = 100,
                    UpgradeCostBase = 50,
                    UpgradeCostMultiplier = 2,
                    UnlockedByDefault = false,
                    MaxLevel = 3
                }
            };

            var staticData = new FakeStaticData(configs);
            _progress = new PersistentProgress { Progress = new PlayerProgress(configs, 0) };
            _economy = new EconomyService(_progress);
            _machines = new MachineService(staticData, _economy, _progress);
        }

        [Test]
        public void FirstMachine_IsUnlockedByDefault()
        {
            Assert.IsTrue(_machines.States.First(s => s.Id == "m1").Unlocked);
        }

        [Test]
        public void SecondMachine_IsLockedByDefault()
        {
            Assert.IsFalse(_machines.States.First(s => s.Id == "m2").Unlocked);
        }

        [Test]
        public void TotalProduction_CountsOnlyUnlocked()
        {
            Assert.AreEqual(1, _machines.GetTotalProduction());
        }

        [Test]
        public void CannotUnlock_WhenNotEnoughBalance()
        {
            var result = _machines.TryUnlock("m2");
            Assert.IsFalse(result);
            Assert.IsFalse(_machines.States.First(s => s.Id == "m2").Unlocked);
        }

        [Test]
        public void CanUnlock_WhenBalanceIsEnough()
        {
            _economy.Add(100);
            var result = _machines.TryUnlock("m2");
            Assert.IsTrue(result);
            Assert.IsTrue(_machines.States.First(s => s.Id == "m2").Unlocked);
            Assert.AreEqual(0, _economy.Balance);
        }

        [Test]
        public void Upgrade_IncreasesLevelAndProduction()
        {
            _economy.Add(100);

            var before = _machines.GetProductionPerSecond(_machines.States.First(s => s.Id == "m1"));
            _machines.TryUpgrade("m1");
            var after = _machines.GetProductionPerSecond(_machines.States.First(s => s.Id == "m1"));

            Assert.AreEqual(before + 1, after);
        }

        [Test]
        public void UpgradeCost_GrowsGeometrically()
        {
            var state = _machines.States.First(s => s.Id == "m1");
            Assert.AreEqual(10, _machines.GetUpgradeCost(state));

            _economy.Add(1000);
            _machines.TryUpgrade("m1");
            Assert.AreEqual(20, _machines.GetUpgradeCost(state));
        }

        [Test]
        public void Upgrade_StopsAtMaxLevel()
        {
            _economy.Add(10_000);
            for (int i = 0; i < 10; i++) _machines.TryUpgrade("m1");
            Assert.AreEqual(3, _machines.States.First(s => s.Id == "m1").Level);
        }

        [Test]
        public void OnStateChanged_FiresAfterUnlock()
        {
            var fired = false;
            _machines.OnStateChanged += () => fired = true;

            _economy.Add(100);
            _machines.TryUnlock("m2");

            Assert.IsTrue(fired);
        }
    }
}