using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSeat : InteractObject
{
    private Player player;
    private PlayerAnim playerAnim;

    private void MakeResearcherSit()
    {
        if(isInteract)
        {
            Debug.Log("!@#$");
            player.transform.position = transform.position;
            playerAnim.SetDir(false);
            playerAnim.SetSit();
        }
    }

    private void Start()
    {
        interact += MakeResearcherSit;
    }

    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
        playerAnim = player.GetComponent<PlayerAnim>();
    }
}
