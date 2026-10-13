namespace Project.Scripts.Gameplay.Offline
{
    public interface IOfflineProgressService
    {
        OfflineResult ApplyOffline(long lastExitUtcTicks, long nowUtcTicks, double productionPerSecond);
    }
}