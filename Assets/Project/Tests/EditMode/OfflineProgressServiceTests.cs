using System;
using NUnit.Framework;
using Project.Scripts.Config;
using Project.Scripts.Gameplay.Offline;
using Project.Scripts.StaticData;

namespace MiniFactory.Tests.EditMode.Project.Tests.EditMode
{
    [TestFixture]
    public class OfflineProgressServiceTests
    {
        private IOfflineProgressService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new OfflineProgressService(new FakeStaticData());
        }

        [Test]
        public void ReturnsZero_WhenNoPreviousExit()
        {
            var now = DateTime.UtcNow.Ticks;
            var result = _service.ApplyOffline(0, now, 10);
            Assert.AreEqual(0, result.Amount);
        }

        [Test]
        public void ReturnsZero_WhenTimeIsNegative()
        {
            var now = DateTime.UtcNow.Ticks;
            var result = _service.ApplyOffline(now + TimeSpan.FromHours(1).Ticks, now, 10);
            Assert.AreEqual(0, result.Amount);
        }

        [Test]
        public void ClampsOffline_ToMaxDuration()
        {
            var now = DateTime.UtcNow.Ticks;
            var last = now - TimeSpan.FromHours(10).Ticks;

            var result = _service.ApplyOffline(last, now, 10);

            Assert.AreEqual(3600, result.Seconds, 0.01);
            Assert.AreEqual(36000, result.Amount, 0.01);
        }

        [Test]
        public void ComputesCorrectly_WithinMaxDuration()
        {
            var now = DateTime.UtcNow.Ticks;
            var last = now - TimeSpan.FromMinutes(10).Ticks;

            var result = _service.ApplyOffline(last, now, 5);

            Assert.AreEqual(600, result.Seconds, 0.5);
            Assert.AreEqual(3000, result.Amount, 5);
        }

        private class FakeStaticData : IStaticDataService
        {
            public MachinesConfig MachinesConfig => null;
            public BoostConfig BoostConfig => null;
            public OfflineConfig OfflineConfig { get; } = new OfflineConfig { MaxOfflineSeconds = 3600 };

            public MachineConfig GetMachine(string id) => null;

            public System.Collections.Generic.IReadOnlyList<MachineConfig> GetAllMachines()
                => System.Array.Empty<MachineConfig>();

            public void Initialize() { }
        }
    }
}