using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputBlocker
{
    public static IEnumerator WaitForMouseButtonDown(int button)
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(button))
            {
                break;
            }
            yield return null;
        }
    }
}
