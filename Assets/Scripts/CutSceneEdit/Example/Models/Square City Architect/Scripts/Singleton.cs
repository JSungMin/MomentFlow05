using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("Instance "+typeof(T) +" not assigned - searching scene");
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError("Instance of type: " + typeof(T) +
                                   " was not found on the scene");
                }
            }
            return instance;
        }
        protected set { instance = value; }
    }
}
