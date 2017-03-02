using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    private Camera myMainCamera;
    private AudioSource audioSource;
    private static BackGroundMusic s_Instance = null;
    
    void Start()
    {
        DontDestroyOnLoad(this);
        myMainCamera = GameObject.FindObjectOfType<Camera>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
   
    public static BackGroundMusic instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new GameObject("Manager").AddComponent<BackGroundMusic>();
                //오브젝트가 생성이 안되있을경우 생성. 
            }
            return s_Instance;
        }
    }
    
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
}
