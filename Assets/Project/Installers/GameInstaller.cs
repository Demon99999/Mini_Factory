using Project.Scripts.Analytics;
using Project.Scripts.Bootstrappers;
using Project.Scripts.Gameplay.Boost;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Gameplay.Loop;
using Project.Scripts.Gameplay.Machines;
using Project.Scripts.Gameplay.Offline;
using Zenject;

namespace Project.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IEconomyService>().To<EconomyService>().AsSingle();
            Container.Bind<IMachineService>().To<MachineService>().AsSingle();
            Container.Bind<IBoostService>().To<BoostService>().AsSingle();
            Container.Bind<IOfflineProgressService>().To<OfflineProgressService>().AsSingle();
            Container.Bind<IGameLoopService>().To<GameLoopService>().AsSingle();

            Container.Bind<IAnalyticsService>().To<AnalyticsService>().AsSingle();
            Container.Bind<IAnalyticsProvider>().To<ConsoleAnalyticsProvider>().AsSingle();

            Container.BindInterfacesTo<GameplayBootstrapper>().AsSingle().NonLazy();
        }
    }
}