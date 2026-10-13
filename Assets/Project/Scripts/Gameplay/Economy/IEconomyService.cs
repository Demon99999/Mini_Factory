using System;

namespace Project.Scripts.Gameplay.Economy
{
    public interface IEconomyService
    {
        double Balance { get; }
        event Action<double> OnBalanceChanged;

        void Add(double amount);
        bool TrySpend(double amount);
    }
}