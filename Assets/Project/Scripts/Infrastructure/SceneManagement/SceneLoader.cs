using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _runner;

        public SceneLoader(ICoroutineRunner runner)
        {
            _runner = runner;
        }

        public void Load(string name, Action onLoaded = null)
        {
            _runner.StartCoroutine(LoadScene(name, onLoaded));
        }

        private IEnumerator LoadScene(string nextScene, Action onLoaded)
        {
            if (SceneManager.GetActiveScene().name == nextScene)
            {
                onLoaded?.Invoke();
                yield break;
            }

            var op = SceneManager.LoadSceneAsync(nextScene);
            while (op != null && !op.isDone)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}