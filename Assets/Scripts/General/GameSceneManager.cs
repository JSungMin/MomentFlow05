using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameScene에서 쓰이는 씬 관리자
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GameSceneManager)) as GameSceneManager;
            }

            if (instance == null)
            {
                GameObject container = new GameObject("GameSceneManager");
                instance = container.AddComponent(typeof(GameSceneManager)) as GameSceneManager;
            }

            return instance;
        }
    }

    private static GameSceneManager instance;

    public void ReplayCurrentScene()
    {
        StartCoroutine(FadeOutAndReplay(SceneManager.GetActiveScene().name));
    }

    private IEnumerator FadeOutAndReplay(string sceneName)
    {
        UI2DSprite aa = GameObject.FindObjectOfType<UI2DSprite>();
        aa.GetComponent<TweenAlpha>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
