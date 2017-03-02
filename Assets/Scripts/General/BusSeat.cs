using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSeat : InteractObject
{
    private Player player;
    private PlayerAnim playerAnim;

    public TweenAlpha fadeOutCam;
    public OverSeerAnim overSeerAnim;

    public DynamicBackground[] dynamicBackgrounds;
    private CutSceneUnit cutSceneUnit;

    public void MakeResearcherSit()
    {
        if(!isInteract)
        {
            player.transform.position = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
            playerAnim.SetDir(false);
            playerAnim.SetSit();
            StartCoroutine(FadeOutAndInCo());
        }
    }

    private void Start()
    {
        interact += MakeResearcherSit;
        fadeOutCam.PlayReverse();
    }

    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
        playerAnim = player.GetComponent<PlayerAnim>();
        cutSceneUnit = GetComponentInParent<CutSceneUnit>();
    }

    private IEnumerator FadeOutAndInCo()
    {
        cutSceneUnit.enabled = false;

        for (int i = 0; i < dynamicBackgrounds.Length; i++)
            StartCoroutine(dynamicBackgrounds[i].StopSlowly());
        yield return new WaitForSeconds(2.0f);

        fadeOutCam.PlayForward();
        yield return new WaitForSeconds(2.0f);
        fadeOutCam.PlayReverse();
        
        overSeerAnim.gameObject.SetActive(true);
        yield return StartCoroutine(overSeerAnim.MakeWalkAndStopAndTalkCo());

        fadeOutCam.PlayForward();
    }
}
