using Project.Scripts.StaticData;

namespace Project.Scripts.Infrastructure.StateMachine.States
{
    public class BootstrapState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IStaticDataService _staticDataService;

        public BootstrapState(GameStateMachine stateMachine, IStaticDataService staticDataService)
        {
            _stateMachine = stateMachine;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            RegisterServices();
            _stateMachine.Enter<LoadProgressState>();
        }

        public void Exit() { }

        private void RegisterServices()
        {
            _staticDataService.Initialize();
        }
    }
}