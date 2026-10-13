using System;

namespace Project.Scripts.Gameplay.Boost
{
    public interface IBoostService
    {
        bool IsActive { get; }
        float RemainingSeconds { get; }
        float Multiplier { get; }

        event Action OnBoostStarted;
        event Action OnBoostFinished;

        bool TryActivate();
        void Tick(float deltaTime);
        void RestoreFromProgress(long nowUtcTicks);
        long GetBoostEndUtcTicks();
    }
}