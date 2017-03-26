using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour {

	public Player playerObject { protected set; get; }
    
	public State enemyState;
    public IState[] istate;

	public AnimationBase anim;

    public float holdDuration;
	public float walkDuration;

	public float detectionDuration;
	protected float detectionDurationTimer = 0;

	public Vector3 velocity;

	public float findOutGauge = 0;
	public float findOutGaugeIncrement;
	public int browseDensity = 3;
	public LayerMask browseMask;

	public float transitionDuration;
	protected float transitionDurationTimer = 0;

	protected void InitEnemy(){
		playerObject = GameObject.FindObjectOfType<Player> ();
		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;
		colXLen = GetComponent<Collider2D> ().bounds.size.x;
		colYLen = GetComponent<Collider2D> ().bounds.size.y;
	}

	public void Awake(){
		Debug.Log ("In Parent");
		InitEnemy ();
	}

	public IState GetState(State enemyState)
    {
        return istate[(int)enemyState];
    }

	float maxX,maxY;
	float minX,minY;
	float colXLen;
	float colYLen;
	//만약 플레이어를 탐지시 return true else then return false
	public float Browse(){
		RaycastHit2D[] browseHits = new RaycastHit2D[browseDensity];
		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;
		for(int i =0;i<browseDensity;i++){
			if (transform.localScale.x > 0) {
				browseHits [i] = Physics2D.Raycast (
					new Vector2 (maxX, minY + colYLen * (i / (browseDensity - 1))),
					Vector2.right,1,browseMask);
			}
			else{
				browseHits [i] = Physics2D.Raycast (
					new Vector2 (minX, minY + colYLen * (i / (browseDensity - 1))),
					Vector2.left,1,browseMask);
			}

			if(browseHits[i].collider!=null){
				if(browseHits[i].collider.CompareTag("Player")){
					return browseHits[i].distance;
				}
			}
		}
		return -1;
	}

}
