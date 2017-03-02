using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bus : MonoBehaviour
{
    private void Awake()
    {

    }

    public void GotoBusScene()
    {
        StartCoroutine(GotoBusSceneCo());
    }

    private IEnumerator GotoBusSceneCo()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("BusScene");
    }
}
