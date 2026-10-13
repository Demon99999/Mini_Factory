using NUnit.Framework;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Progress;

namespace MiniFactory.Tests.EditMode.Project.Tests.EditMode
{
    [TestFixture]
    public class EconomyServiceTests
    {
        private IEconomyService _economy;

        [SetUp]
        public void SetUp()
        {
            var progress = new PersistentProgress();
            progress.Progress = new PlayerProgress();
            _economy = new EconomyService(progress);
        }

        [Test]
        public void Add_IncreasesBalance()
        {
            _economy.Add(100);
            Assert.AreEqual(100, _economy.Balance);
        }

        [Test]
        public void Add_IgnoresNonPositiveAmounts()
        {
            _economy.Add(0);
            _economy.Add(-50);
            Assert.AreEqual(0, _economy.Balance);
        }

        [Test]
        public void TrySpend_ReturnsFalse_WhenNotEnough()
        {
            _economy.Add(10);
            var spent = _economy.TrySpend(100);
            Assert.IsFalse(spent);
            Assert.AreEqual(10, _economy.Balance);
        }

        [Test]
        public void TrySpend_ReturnsTrue_AndDecreasesBalance()
        {
            _economy.Add(100);
            var spent = _economy.TrySpend(30);
            Assert.IsTrue(spent);
            Assert.AreEqual(70, _economy.Balance);
        }

        [Test]
        public void OnBalanceChanged_FiresAfterChanges()
        {
            var lastBalance = -1.0;
            _economy.OnBalanceChanged += b => lastBalance = b;

            _economy.Add(100);
            Assert.AreEqual(100, lastBalance);

            _economy.TrySpend(50);
            Assert.AreEqual(50, lastBalance);
        }
    }
}