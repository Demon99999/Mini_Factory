using Project.Scripts.Infrastructure.SceneManagement;

namespace Project.Scripts.Infrastructure.StateMachine.States
{
    public class GameLoopState : IState
    {
        private const string GameScene = "Game";

        private readonly ISceneLoader _sceneLoader;

        public GameLoopState(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.Load(GameScene);
        }

        public void Exit() { }
    }
}