using Project.Scripts.Infrastructure.AssetManagement;
using Project.Scripts.Infrastructure.SceneManagement;
using Project.Scripts.Progress;
using Project.Scripts.StaticData;
using Zenject;

namespace Project.Installers
{
    public class GlobalInstaller : MonoInstaller, ICoroutineRunner
    {
        public override void InstallBindings()
        {
            Container.Bind<ICoroutineRunner>().FromInstance(this).AsSingle();

            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();

            Container.Bind<IPersistentProgress>().To<PersistentProgress>().AsSingle();
            Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();

            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

            GameStateMachineInstaller.Install(Container);
        }
    }
}