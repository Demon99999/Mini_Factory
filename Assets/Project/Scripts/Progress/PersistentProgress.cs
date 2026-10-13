namespace Project.Scripts.Progress
{
    public class PersistentProgress : IPersistentProgress
    {
        public PlayerProgress Progress { get; set; } = new PlayerProgress();
    }
}