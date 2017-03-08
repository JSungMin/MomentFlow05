using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveManager : MonoBehaviour {

	public LayerMask masking;
	public Collider2D interactiveRect;

	public List<Collider2D> nearNpcList = new List<Collider2D> ();
	public List<Collider2D> nearObjList = new List<Collider2D> ();

	public bool nowInteract = false;
	private Collider2D interactObject;

	public int index;
	private float tmpDis;

	public void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.layer == LayerMask.NameToLayer("NPC")){

			if(!nearNpcList.Contains(col))
				nearNpcList.Add (col);

			Debug.Log ("Find NPC");
		}else if(col.gameObject.layer == LayerMask.NameToLayer("Object")){
			Debug.Log ("Find Object");
			if (!nearObjList.Contains (col))
				nearObjList.Add (col);
		}
	}

	public void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.layer == LayerMask.NameToLayer ("NPC")) {
			col.gameObject.GetComponentInChildren<cakeslice.Outline> ().eraseRenderer = true;
			col.GetComponent<NPC> ().StopInteract ();
			interactObject = null;
			nearNpcList.Remove (col);
		}else if(col.gameObject.layer == LayerMask.NameToLayer("Object")){
			var outlines = col.gameObject.GetComponentsInChildren<cakeslice.Outline> ();
			for (int i = 0; i < outlines.Length; i++) {
				outlines [i].eraseRenderer = true;
			}
			if(null!=col.GetComponent<BusStop>()){
				col.GetComponent<BusStop> ().StopInteract ();
			}
			interactObject = null;
			nearObjList.Remove (col);
		}
	}

	void Update(){
		/*Set nearest NPC Outline*/
		if (nearNpcList.Count >= 1) {
			tmpDis = Vector3.Distance (new Vector3 (transform.position.x, transform.position.y, 0), new Vector3 (nearNpcList [0].transform.position.x, nearNpcList [0].transform.position.y, 0));
			index = 0;
			nearNpcList [index].GetComponentInChildren<cakeslice.Outline> ().eraseRenderer = true;
			for (int i = 1; i < nearNpcList.Count; i++) {
				var pPos = transform.position;
				var nPos = nearNpcList [i].transform.position;
				var dis = Vector3.Distance (nPos, pPos);
				if (tmpDis >= dis) {
					index = i;
					tmpDis = dis;
				}
				nearNpcList [i].GetComponentInChildren<cakeslice.Outline> ().eraseRenderer = true;
			}
			nearNpcList [index].GetComponentInChildren<cakeslice.Outline> ().eraseRenderer = false;
		}
		//Set nearest Object Outline
		if (nearObjList.Count >= 1) {
			tmpDis = Vector3.Distance (new Vector3 (transform.position.x, transform.position.y, 0), new Vector3 (nearObjList [0].transform.position.x, nearObjList [0].transform.position.y, 0));
			index = 0;
			nearObjList [index].GetComponentInChildren<cakeslice.Outline> ().eraseRenderer = true;
			for (int i = 1; i < nearObjList.Count; i++) {
				var pPos = transform.position;
				var nPos = nearObjList [i].transform.position;
				var dis = Vector3.Distance (nPos, pPos);
				if (tmpDis >= dis) {
					index = i;
					tmpDis = dis;
				}
				var outlines = nearObjList[i].GetComponentsInChildren<cakeslice.Outline> ();
				for (int j = 0; j < outlines.Length; j++) {
					outlines [j].eraseRenderer = true;
				}
			}
			var outlinesSet = nearObjList[index].GetComponentsInChildren<cakeslice.Outline> ();
			for (int j = 0; j < outlinesSet.Length; j++) {
				outlinesSet [j].eraseRenderer = false;
			}
		}

		//Interact Part
		if (Input.GetKey (KeyCode.E)) {
			if (nearNpcList.Count >= 1) {
				if (!nowInteract) {
					nearNpcList[index].GetComponent<NPC> ().Interact();
					nowInteract = true;
				}
			}
			if (nearObjList.Count >= 1) {
				if (!nowInteract) {
                    if (nearObjList[index].GetComponent<BusStop>() != null)
                        nearObjList[index].GetComponent<BusStop>().Interact();
                    if (nearObjList[index].GetComponent<BusSeat>() != null)
                        nearObjList[index].GetComponent<BusSeat>().Interact();
                    nowInteract = true;
				}
			}
		}
		//Cancel Interact
		if(Input.GetKey(KeyCode.Escape)){
			if (nowInteract) {
				nowInteract = false;
				nearNpcList[index].GetComponent<NPC> ().StopInteract ();
				interactObject = null;
			}
		}
	}
}
