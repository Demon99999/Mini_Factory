using Project.Scripts.Infrastructure.StateMachine;
using Zenject;

namespace Project.Installers
{
    public static class GameStateMachineInstaller
    {
        public static void Install(DiContainer container)
        {
            container.Bind<GameStateMachine>().AsSingle();
            container.Bind<StatesFactory>().AsSingle();
        }
    }
}