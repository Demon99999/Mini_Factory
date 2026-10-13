using System.Collections;
using UnityEngine;

namespace Project.Scripts.Infrastructure.SceneManagement
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }
}
