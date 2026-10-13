using Project.Scripts.Progress;
using Project.Scripts.StaticData;

namespace Project.Scripts.Infrastructure.StateMachine.States
{
    public class LoadProgressState : IState
    {
        private const double StartBalance = 0;

        private readonly GameStateMachine _stateMachine;
        private readonly IPersistentProgress _progress;
        private readonly ISaveLoadService _saveLoad;
        private readonly IStaticDataService _staticData;

        public LoadProgressState(
            GameStateMachine stateMachine,
            IPersistentProgress progress,
            ISaveLoadService saveLoad,
            IStaticDataService staticData)
        {
            _stateMachine = stateMachine;
            _progress = progress;
            _saveLoad = saveLoad;
            _staticData = staticData;
        }

        public void Enter()
        {
            LoadProgressOrInitNew();
            _stateMachine.Enter<GameLoopState>();
        }

        public void Exit() { }

        private void LoadProgressOrInitNew()
        {
            _progress.Progress = _saveLoad.LoadProgress() ?? CreateNewProgress();
        }

        private PlayerProgress CreateNewProgress()
            => new PlayerProgress(_staticData.MachinesConfig.Machines, StartBalance);
    }
}