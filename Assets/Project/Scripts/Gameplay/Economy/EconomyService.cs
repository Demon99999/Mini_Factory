using System;
using Project.Scripts.Progress;

namespace Project.Scripts.Gameplay.Economy
{
    public class EconomyService : IEconomyService
    {
        private readonly IPersistentProgress _progress;

        public double Balance => _progress.Progress.Balance;
        public event Action<double> OnBalanceChanged;

        public EconomyService(IPersistentProgress progress)
        {
            _progress = progress;
        }

        public void Add(double amount)
        {
            if (amount <= 0) return;
            
            _progress.Progress.Balance += amount;
            OnBalanceChanged?.Invoke(Balance);
        }

        public bool TrySpend(double amount)
        {
            if (amount < 0 || _progress.Progress.Balance < amount) return false;
            
            _progress.Progress.Balance -= amount;
            OnBalanceChanged?.Invoke(Balance);
            return true;
        }
    }
}