using System;
using Project.Scripts.Progress;
using Project.Scripts.StaticData;

namespace Project.Scripts.Gameplay.Boost
{
    public class BoostService : IBoostService
    {
        private readonly Config.BoostConfig _config;
        private readonly IPersistentProgress _progress;
        private float _remaining;

        public bool IsActive => _remaining > 0f;
        public float RemainingSeconds => _remaining;
        public float Multiplier => IsActive ? _config.Multiplier : 1f;

        public event Action OnBoostStarted;
        public event Action OnBoostFinished;

        public BoostService(IStaticDataService staticData, IPersistentProgress progress)
        {
            _config = staticData.BoostConfig;
            _progress = progress;
        }

        public bool TryActivate()
        {
            if (!_config.Enabled || IsActive) return false;

            _remaining = _config.DurationSeconds;
            _progress.Progress.BoostEndUtcTicks = DateTime.UtcNow.AddSeconds(_remaining).Ticks;

            OnBoostStarted?.Invoke();
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (!IsActive) return;

            _remaining -= deltaTime;
            
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                _progress.Progress.BoostEndUtcTicks = 0;
                OnBoostFinished?.Invoke();
            }
        }

        public void RestoreFromProgress(long nowUtcTicks)
        {
            var ticks = _progress.Progress.BoostEndUtcTicks;
            
            if (ticks <= 0) return;

            var remainingTicks = ticks - nowUtcTicks;
            
            if (remainingTicks <= 0)
            {
                _progress.Progress.BoostEndUtcTicks = 0;
                return;
            }

            _remaining = Math.Min(
                (float)TimeSpan.FromTicks(remainingTicks).TotalSeconds,
                _config.DurationSeconds);

            OnBoostStarted?.Invoke();
        }

        public long GetBoostEndUtcTicks()
            => IsActive ? DateTime.UtcNow.AddSeconds(_remaining).Ticks : 0;
    }
}