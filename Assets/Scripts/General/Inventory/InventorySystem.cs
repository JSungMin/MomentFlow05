using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InventorySystem : MonoBehaviour {

	public List<GameObject> ids;

	private string keyHash = "Brown";

	public void Awake(){
		ZPlayerPrefs.Initialize ("Inventory Infomation", keyHash);
		//ZPlayerPrefs.DeleteAll ();
		InitItemData ();
	}

	public void InitItemData(){
		for(int i = 0; i<ids.Count; i++){
			SetInventoryItem (i, 0);
		}
	}

	//return item amount ( not index ) if haven't item then return -1
	public int GetInventoryItem(int index, string itemStr){
		var splitData = itemStr.Split ("," [0]);

		if (index * 2 >= splitData.Length || index < 0)
			return -1;
		else
			return int.Parse (splitData [index * 2]);
	}

	public void SetInventoryItem(int itemIndex, int num){
		string tmp = ZPlayerPrefs.GetString ("InventoryInfo");
		string newTmp = "";
		var splitData = tmp.Split ("," [0]);

		bool isContainData = tmp.Contains(ids[itemIndex].name);
		//if Loaded data contains item then excute below statement
		if (isContainData) {
			for (int i = 0; i < splitData.Length - 1; i += 2) {
				int iNameIndex = (splitData [i] == ids [itemIndex].name) ? i : -1;
				int iDataIndex = (splitData [i] == ids [itemIndex].name) ? i + 1 : -1;

				if (iNameIndex != -1) {
					int tmpData = num;
					splitData [iDataIndex] = tmpData.ToString ();
				}

				if (i != 0)
					newTmp += "," + splitData [i] + "," + splitData [i + 1];
				else
					newTmp += splitData [i] + "," + splitData [i + 1];
			}
		}
		else { // if Loaded data not contains item then excute below statement
			for(int i = 0; i < splitData.Length; i++){
				if(splitData[i] != "")
					newTmp += splitData[i] + ",";
			}
			newTmp += ids [itemIndex].name + "," + num;
		}
		ZPlayerPrefs.SetString ("InventoryInfo", newTmp);
		ZPlayerPrefs.Save ();
	}
}
