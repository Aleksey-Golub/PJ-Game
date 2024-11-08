using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure
{
  public class SceneLoader
  {
    private readonly ICoroutineRunner _coroutineRunner;

    public SceneLoader(ICoroutineRunner coroutineRunner) => 
      _coroutineRunner = coroutineRunner;

    public static string CurrentLevel() => SceneManager.GetActiveScene().name;

    public void Load(string name, Action onLoaded = null) =>
      _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));
    
    private IEnumerator LoadScene(string nextScene, Action onLoaded = null)
    {
      if (SceneManager.GetActiveScene().name == nextScene)
      {
        onLoaded?.Invoke();
        yield break;
      }
      
      AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

      while (!waitNextScene.isDone)
        yield return null;
      
      onLoaded?.Invoke();
    }

    public GameObject[] GetSceneRoots(string sceneName) => SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
  }
}