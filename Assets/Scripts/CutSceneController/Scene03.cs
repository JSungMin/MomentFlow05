using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene03 : MonoBehaviour
{
    private void Awake()
    {

    }

    private IEnumerator WaitForClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }
            yield return null;
        }
    }
}
