using System;

namespace Project.Scripts.Infrastructure.SceneManagement
{
    public interface ISceneLoader
    {
        void Load(string name, Action onLoaded = null);
    }
}