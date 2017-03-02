using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paging : MonoBehaviour {

	public List<string> contents;
	public int offset = 0;

	public int contentOffset = 0;
	public float typingSpeed = 0.05f;

	public UILabel label;

	public IEnumerator showContent;

	public void EraseContent(){
        label.text = "";
		contentOffset = 0;
	}

	public IEnumerator ShowContent(){
		Debug.Log ("Yes");
		while (contentOffset < contents [offset].Length) {
			yield return new WaitForSeconds (typingSpeed);
			label.text += contents [offset].ToCharArray () [contentOffset];
			contentOffset++;
		}
	}

	public void StopShowContent(){
		contentOffset = 0;
		offset = -1;
		StopCoroutine (showContent);
		showContent = ShowContent ();
	}

	public void NextPage(){
		if (offset < contents.Count) {
			Debug.Log ("NP");
			EraseContent ();
			StopCoroutine (showContent);
			offset+=1;
            if (offset < contents.Count)
                StartCoroutine(ShowContent());
			//StartCoroutine (showContent);
		}
	}
	public void PreviousPage(){
		if (offset - 1 >= -1) {
			EraseContent ();
			StopCoroutine (showContent);
			offset--;
			StartCoroutine (showContent);
		}
	}
		
	void Start(){
        showContent = ShowContent();
        //StartCoroutine(showContent);
    }

    public int GetContentCount()
    {
        return contents.Count;
    }
}
