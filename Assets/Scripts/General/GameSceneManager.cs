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
    private FollowUpCamera followUpCamera;
    private ElasticityGaugeManager elasticityGaugeManager;

    private void Awake()
    {
        followUpCamera = GameObject.FindObjectOfType<FollowUpCamera>();
        elasticityGaugeManager = GetComponent<ElasticityGaugeManager>();
        if (elasticityGaugeManager == null)
            elasticityGaugeManager = gameObject.AddComponent<ElasticityGaugeManager>();
    }

    public void ReplayCurrentScene()
    {
        StartCoroutine(FadeOutAndReplay(SceneManager.GetActiveScene().name));
    }

    private IEnumerator FadeOutAndReplay(string sceneName)
    {
        yield return new WaitForSeconds(1.0f);
        followUpCamera.fadeOut.duration = 1.5f;
        followUpCamera.fadeOut.enabled = true;

        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void AddElasticityGauge(int gauge, UndoDelegate undoDelegate)
    {
        elasticityGaugeManager.AddGauge(gauge, undoDelegate);
    }

    public void SubElasticityGauge(int gauge)
    {
        elasticityGaugeManager.SubGauge(gauge);
    }
}
