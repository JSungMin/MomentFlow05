using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// 각각의 대화 컨텐츠들을 글자 하나씩 보여주고, 페이지를 넘기는 역할을 한다
public class Paging : MonoBehaviour {

	public List<string> contents;
	public int offset { private set; get; }

	private int contentOffset = 0;
	public float typingSpeed = 0.05f;

	private UILabel label;

	private IEnumerator showContent;

    private void Awake()
    {
        offset = -1;
        label = GetComponentInChildren<UILabel>();
    }

    void Start()
    {
        showContent = ShowContent();
    }

    public void EraseContent(){
        label.text = "";
		contentOffset = 0;
	}

	private IEnumerator ShowContent(){
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

    public int GetContentCount()
    {
        return contents.Count;
    }
}
