using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene02 : MonoBehaviour
{
    private HansAnim hansAnim;

    private float paperReadDuration = 3.0f;
    private float dialogueShowDuration = 2.0f;
    private float passengerPickUpDuration = 2.0f;

    public BubbleDialogue stopBusDialogue;
    public GameObject[] passengers;
    private CutSceneUnit[] passengersCutSceneUnit;
    private AnimationBase[] passengersAnim;

    private int offset = 0;
    private bool isRoutineOver = false;

    private void Awake()
    {
        hansAnim = GameObject.FindObjectOfType<HansAnim>();

        passengersCutSceneUnit = new CutSceneUnit[passengers.Length];
        passengersAnim = new AnimationBase[passengers.Length];

        for (int i = 0; i < passengers.Length; i++)
        {
            passengersCutSceneUnit[i] = passengers[i].GetComponent<CutSceneUnit>();
            passengersAnim[i] = passengers[i].GetComponent<AnimationBase>();
        }
    }

    private void Start()
    {
        SendToProcessCoroutine(UntilLoboFirstDialogue());
    }

    // 로보가 처음 대사를 하기 전까지의 일련의 행동들
    private IEnumerator UntilLoboFirstDialogue()
    {
        yield return StartCoroutine(MakeHansReadPaper());

        yield return StartCoroutine(ShowStopBusDialogue());

        yield return StartCoroutine(StopBus());

        yield return StartCoroutine(PickUpPassengers());

        yield return StartCoroutine(StopTime());
    }

    private IEnumerator MakeHansReadPaper()
    {
        hansAnim.ReadPaper();

        yield return new WaitForSeconds(paperReadDuration);
    }

    // 버스를 멈추겠다는 안내 방송
    private IEnumerator ShowStopBusDialogue()
    {
        stopBusDialogue.StartBubble();
        for (int i = 1; i < stopBusDialogue.GetContentCount(); i++)
        {
            yield return new WaitForSeconds(dialogueShowDuration);
            stopBusDialogue.NextPage();
        }
    }
    
    private IEnumerator StopBus()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator PickUpPassengers()
    {
        for (int i = 0; i < passengersCutSceneUnit.Length; i++)
        {
            passengersCutSceneUnit[i].StartAction();
            yield return new WaitForSeconds(passengerPickUpDuration);
        }
    }

    private IEnumerator StopTime()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator PickUpLobo()
    {
        yield return new WaitForSeconds(1.0f);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isRoutineOver)
            {
                offset++;
                isRoutineOver = false;
            }
            else
            {
                switch (offset)
                {
                    case 1:
                        break;
                    default:
                        break;
                }   
            }
        }
    }
    
    private void SendToProcessCoroutine(IEnumerator ienum)
    {
        StartCoroutine(ProcessCoroutine(ienum));
    }

    private IEnumerator ProcessCoroutine(IEnumerator ienum)
    {
        yield return StartCoroutine(ienum);
        isRoutineOver = true;
    }
}
