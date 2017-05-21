using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class ReplayController : MonoBehaviour {

	public int maxStorage = 300;

	public PlayerSkillState skillState;

	private Player player;

	public Dictionary<GameObject,List<Transform>> transformDictionary = new Dictionary<GameObject, List<Transform>>();
	public Dictionary<GameObject,List<SkeletonAnimation>> animationDictionary = new Dictionary<GameObject, List<SkeletonAnimation>>();
	public Dictionary<GameObject,List<EnemyScript>> enemiesDictionary = new Dictionary<GameObject, List<EnemyScript>>();
	public static List<List<int>> playerList = new List<List<int>>();

	public Dictionary<GameObject, int> indexOfTransfomDictionary = new Dictionary<GameObject, int>();
	public Dictionary<GameObject, int> indexOfAnimationDictionary = new Dictionary<GameObject, int>();
	public Dictionary<GameObject, int> indexOfEnemiesDictionary = new Dictionary<GameObject, int>();
	public int indexOfPlayerList = 0;

	public void Awake()
	{
		InitReplayController ();
	}

	void InitReplayController ()
	{
		player = GameObject.FindObjectOfType<Player> ();
		var transforms = GameObject.FindObjectsOfType<Transform> ();
		foreach (var t in transforms) {
			if (!t.gameObject.isStatic) {
				UpdateTransformDictionary (t.gameObject);
			}
		}
		var animations = GameObject.FindObjectsOfType<SkeletonAnimation> ();
		foreach (var a in animations) {
			UpdateAnimationDictionary (a.gameObject);
		}
		var enemies = GameObject.FindObjectsOfType<EnemyScript> ();
		foreach (var e in enemies) {
			UpdateEnemiesDictionary (e.gameObject);
		}
		UpdatePlayerList (player.gameObject);
	}

	Transform GetTransfomFromLastIndex (GameObject targetTransform)
	{
		return transformDictionary [targetTransform] [indexOfTransfomDictionary [targetTransform]];
	}

	SkeletonAnimation GetAnimaitonFromLastIndex (GameObject targetAnimation)
	{
		return animationDictionary [targetAnimation] [indexOfAnimationDictionary [targetAnimation]];
	}

	EnemyScript GetEnemiesFromLastIndex (GameObject targetEnemies)
	{
		return enemiesDictionary [targetEnemies] [indexOfEnemiesDictionary [targetEnemies]];
	}

	public void UpdateTransformDictionary (GameObject targetTransform)
	{
		if (!transformDictionary.ContainsKey (targetTransform))
		{
			transformDictionary.Add (targetTransform, new List<Transform>(new Transform[maxStorage]));
			indexOfTransfomDictionary.Add (targetTransform, 0);
		}

		transformDictionary [targetTransform] [indexOfTransfomDictionary [targetTransform]] = targetTransform.transform;

		if (indexOfTransfomDictionary[targetTransform] + 1 < transformDictionary [targetTransform].Count)
			indexOfTransfomDictionary[targetTransform]++;
		else
			indexOfTransfomDictionary[targetTransform] = 0;
	}

	public void UpdateAnimationDictionary (GameObject targetAnimation)
	{
		if (!animationDictionary.ContainsKey (targetAnimation))
		{
			animationDictionary.Add (targetAnimation, new List<SkeletonAnimation>(new SkeletonAnimation[maxStorage]));
			indexOfAnimationDictionary.Add (targetAnimation, 0);
		}

		animationDictionary [targetAnimation] [indexOfAnimationDictionary [targetAnimation]] = targetAnimation.GetComponent<SkeletonAnimation> ();

		if (indexOfAnimationDictionary[targetAnimation] + 1 < animationDictionary [targetAnimation].Count)
			indexOfAnimationDictionary[targetAnimation]++;
		else
			indexOfAnimationDictionary[targetAnimation] = 0;
	}

	public void UpdateEnemiesDictionary (GameObject targetEnemies)
	{
		if (!enemiesDictionary.ContainsKey (targetEnemies))
		{
			enemiesDictionary.Add (targetEnemies, new List<EnemyScript>(new EnemyScript[maxStorage]));
			indexOfEnemiesDictionary.Add (targetEnemies, 0);
		}

		enemiesDictionary [targetEnemies] [indexOfEnemiesDictionary [targetEnemies]] = targetEnemies.GetComponent<EnemyScript> ();

		if (indexOfEnemiesDictionary[targetEnemies] + 1 < enemiesDictionary [targetEnemies].Count)
			indexOfEnemiesDictionary[targetEnemies]++;
		else
			indexOfEnemiesDictionary[targetEnemies] = 0;
	}

	public void UpdatePlayerList (GameObject targetPlayer)
	{
		if (playerList.Count == 0) {
			for (int i = 0; i < maxStorage; i++)
			{
				playerList.Add (new List<int> ());
			}
		}
		playerList [indexOfPlayerList] = targetPlayer.GetComponent<Player> ().stateQueue;

		if (indexOfPlayerList + 1 < playerList.Count)
			indexOfPlayerList++;
		else
			indexOfPlayerList = 0;
	}

	public Transform GetTargetTransformFromIndex (GameObject target, int index)
	{
		return transformDictionary [target] [index];
	}

	public SkeletonAnimation GetTargetAnimationFromIndex (GameObject target, int index)
	{
		return animationDictionary [target] [index];
	}

	public EnemyScript GetTargetEnemyFromIndex (GameObject target, int index)
	{
		return enemiesDictionary [target] [index];
	}

	public List<int> GetTargetPlayerFromIndex (int index)
	{
		return playerList [index];
	}

	private void FixedUpdate()
	{
		if (Input.GetKey (KeyCode.E))
			skillState = PlayerSkillState.Replay;
		else
			skillState = PlayerSkillState.None;

		if (skillState == PlayerSkillState.None) {
			UpdatePlayerList (player.gameObject);
			var transforms = GameObject.FindObjectsOfType<Transform> ();
			foreach (var t in transforms) {
				if (!t.gameObject.isStatic) {
					UpdateTransformDictionary (t.gameObject);
				}
			}
			var animations = GameObject.FindObjectsOfType<SkeletonAnimation> ();
			foreach (var a in animations) {
				UpdateAnimationDictionary (a.gameObject);
			}
			var enemies = GameObject.FindObjectsOfType<EnemyScript> ();
			foreach (var e in enemies) {
				UpdateEnemiesDictionary (e.gameObject);
			}
		}
		else if (skillState == PlayerSkillState.Replay)
		{
			var transforms = GameObject.FindObjectsOfType<Transform> ();
			for (int i = 0; i < transforms.Length; i++) {
				var t = transforms [i];
				if (t.gameObject.isStatic)
					continue;
				if (indexOfTransfomDictionary[t.gameObject] - 1 >= 0)
					indexOfTransfomDictionary [t.gameObject]--;
				t = GetTransfomFromLastIndex (t.gameObject);
			}

			if (indexOfPlayerList - 1 >= 0)
				indexOfPlayerList--;
			Debug.Log ("Now : " + player.state + " , Index : " + playerList[indexOfPlayerList][0]);
			player.stateQueue = playerList [indexOfPlayerList];
		}
	}
}