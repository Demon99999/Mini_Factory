using Project.Scripts.Infrastructure.SceneManagement;
using Project.Scripts.Infrastructure.StateMachine;
using Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Bootstrappers
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private GameStateMachine _stateMachine;
        private StatesFactory _statesFactory;

        [Inject]
        private void Construct(GameStateMachine stateMachine, StatesFactory statesFactory)
        {
            _stateMachine = stateMachine;
            _statesFactory = statesFactory;
        }

        private void Awake()
        {
            _stateMachine.RegisterState(_statesFactory.Create<BootstrapState>());
            _stateMachine.RegisterState(_statesFactory.Create<LoadProgressState>());
            _stateMachine.RegisterState(_statesFactory.Create<GameLoopState>());

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _stateMachine.Enter<BootstrapState>();
        }
    }
}