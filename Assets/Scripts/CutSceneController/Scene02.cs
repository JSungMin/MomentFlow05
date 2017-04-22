using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene02 : MonoBehaviour
{
    private HansAnim hansAnim;

    private float paperReadDuration = 3.0f;
    private float dialogueShowDuration = 2.0f;
    private float passengerPickUpDuration = 3f;

    public BubbleDialogue stopBusDialogue;
    
	public CutSceneUnit busUnit;

	public CutSceneUnit[] watcherAgentUnits;
	public CutSceneUnit roboUnit;

	public GameObject[] passengers;
	private CutSceneUnit[] passengersCutSceneUnit;
    private AnimationBase[] passengersAnim;

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
		StartCoroutine(UntilLoboFirstDialogue());
    }

	private IEnumerator WaitForClick(){
		while(true){
			if(Input.GetMouseButtonDown(0)){
				break;
			}
			yield return null;
		}
	}

    // 로보가 처음 대사를 하기 전까지의 일련의 행동들
    private IEnumerator UntilLoboFirstDialogue()
    {
        yield return StartCoroutine(MakeHansReadPaper());

        yield return StartCoroutine(ShowStopBusDialogue());

        yield return StartCoroutine(StopBus());

        yield return StartCoroutine(PickUpPassengers());

        yield return StartCoroutine(StopTime());

		yield return StartCoroutine (MakeHansClosePaper());

		yield return StartCoroutine (PickUpAgentsAndRobo());
    }

    private IEnumerator MakeHansReadPaper()
    {
		hansAnim.OpenPaper ();
		yield return new WaitForSeconds (1);
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
			passengers [i].SetActive (true);
            passengersCutSceneUnit[i].StartAction();
            yield return new WaitForSeconds(passengerPickUpDuration);
        }
    }

    private IEnumerator StopTime()
    {
		Camera.main.GetComponent<CameraController> ().isBlured = true;
		for(int i = 0; i < passengersCutSceneUnit.Length; i++){
			passengersCutSceneUnit [i].PasueAction ();
			passengersAnim [i].StopAnimation();
		}
		busUnit.PasueAction ();
		yield return new WaitForSeconds(1);
    }

	private IEnumerator MakeHansClosePaper(){
		hansAnim.ClosePaper ();
		yield return new WaitForSeconds(4.5f);
	}

	private IEnumerator PickUpAgentsAndRobo(){
		roboUnit.gameObject.SetActive (true);
		roboUnit.StartAction ();
		for(int i = 0; i < watcherAgentUnits.Length;i++){
			yield return new WaitForSeconds (0.5f);
			watcherAgentUnits [i].gameObject.SetActive (true);
			watcherAgentUnits [i].StartAction ();
		}
		yield return new WaitForSeconds (2);
	}

    private IEnumerator PickUpLobo()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
