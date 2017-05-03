using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    public bool isOpened { private set; get; }

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 기본 상태는 문을 닫은 상태임
        CloseDoor();
    }

    public void OpenDoor()
    {
        boxCollider2D.enabled = false;
        // TODO: 문이 닫히고 열린 상태를 애니메이션으로 처리할 것
        spriteRenderer.enabled = false;
        isOpened = true;
    }

    public void CloseDoor()
    {
        boxCollider2D.enabled = true;
        spriteRenderer.enabled = true;
        isOpened = false;
    }
}