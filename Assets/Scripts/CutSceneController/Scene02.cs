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

	public ChatDialogue chatDialogue;

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

		yield return StartCoroutine (AllCutUnitActionPause());

		yield return StartCoroutine (ProcessChatDialog ());
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
		for(int i = 0; i < watcherAgentUnits.Length;i++){
			watcherAgentUnits [i].gameObject.SetActive (true);
			watcherAgentUnits [i].StartAction ();
			yield return new WaitForSeconds (0.5f);
		}

		roboUnit.gameObject.SetActive (true);
		roboUnit.StartAction ();

		yield return new WaitForSeconds (4f);
	}

	private IEnumerator StartChatDialogue(){
		chatDialogue.StartChat ();
		yield return StartCoroutine (InputBlocker.WaitForMouseButtonDown(0));
	}

	private IEnumerator ContinueChatDialogue(){
		yield return StartCoroutine (InputBlocker.WaitForMouseButtonDown(0));
		chatDialogue.NextPage ();
		yield return StartCoroutine (InputBlocker.WaitForMouseButtonDown(0));
	}

    private IEnumerator PickUpLobo()
    {
        yield return new WaitForSeconds(1.0f);
    }

	private IEnumerator AllCutUnitActionPause(){
		foreach (var unit in watcherAgentUnits) {
			unit.PasueAction ();
		}
		foreach (var unit in passengersCutSceneUnit) {
			unit.PasueAction ();
		}
		roboUnit.PasueAction ();
		yield return null;
	}

	private IEnumerator ProcessChatDialog(){
		yield return StartCoroutine (StartChatDialogue());
		for(int i = 1; i < chatDialogue.GetContentCount(); i++){
			yield return StartCoroutine (ContinueChatDialogue ());
			switch(i){
			case 10:
				//Angry Lobo
				yield return StartCoroutine(LoboAttackHans());
				break;
			case 13:
				yield return StartCoroutine (LoboStapToEntrance ());
				break;
			}
		}
	}

	private IEnumerator LoboAttackHans(){
		yield return StartCoroutine (AngryFaceLobo ());

	}

	private IEnumerator AngryFaceLobo(){
		roboUnit.GetComponent<LoboAnim> ().AngryFace ();
		yield return StartCoroutine (LoboStapToHansAndAttack ());
	}

	private IEnumerator LoboStapToHansAndAttack(){
		roboUnit.GetComponent<LoboAnim> ().Run ();
		while(true){
			roboUnit.transform.position = Vector3.MoveTowards (roboUnit.transform.position, hansAnim.transform.position, Time.deltaTime*0.5f);	
			if(roboUnit.transform.position.x <= hansAnim.transform.position.x +0.15f){
				break;
			}
			yield return null;
		}
		roboUnit.GetComponent<LoboAnim> ().Attack ();
		yield return new WaitForSeconds (2.5f);
	}

	private IEnumerator LoboStapToEntrance(){
		roboUnit.GetComponent<LoboAnim> ().SetDir (false);
		while (true) {
			roboUnit.GetComponent<LoboAnim> ().Walk ();
			roboUnit.transform.localPosition = Vector3.MoveTowards (roboUnit.transform.localPosition, new Vector3(0,roboUnit.transform.localPosition.y), Time.deltaTime*0.4f);
			if(roboUnit.transform.localPosition.x>=0){
				break;
			}
			yield return null;
		}
		roboUnit.GetComponent<LoboAnim> ().LoboIdle ();
		yield return new WaitForSeconds (2f);
	}
}
