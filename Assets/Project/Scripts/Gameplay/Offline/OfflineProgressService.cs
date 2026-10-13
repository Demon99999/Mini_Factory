using System;
using Project.Scripts.StaticData;

namespace Project.Scripts.Gameplay.Offline
{
    public class OfflineProgressService : IOfflineProgressService
    {
        private readonly Config.OfflineConfig _config;

        public OfflineProgressService(IStaticDataService staticData)
        {
            _config = staticData.OfflineConfig;
        }

        public OfflineResult ApplyOffline(long lastExitUtcTicks, long nowUtcTicks, double productionPerSecond)
        {
            var result = new OfflineResult();
            
            if (lastExitUtcTicks <= 0) return result;

            var deltaTicks = nowUtcTicks - lastExitUtcTicks;
            
            if (deltaTicks <= 0) return result;

            var seconds = Math.Min(
                TimeSpan.FromTicks(deltaTicks).TotalSeconds,
                _config.MaxOfflineSeconds);

            result.Seconds = seconds;
            result.Amount = productionPerSecond * seconds;
            return result;
        }
    }
}