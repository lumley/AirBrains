using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingShifter : MonoBehaviour {

	public float displayTime = 2f;

	public Transform[] marquees;

	void Start() {
		StartCoroutine(SetActive(0));
	}

	private IEnumerator SetActive(int index) {
		int minMax = 0;
		foreach(Transform parent in marquees) {
			for(int childIndex = 0; childIndex < parent.childCount; childIndex++) {
				parent.GetChild(childIndex).gameObject.SetActive(childIndex == index);
				if(childIndex > minMax) {
					minMax = childIndex;
				}
			}
		}
		Debug.Log("Started Waiting");
		yield return new WaitForSeconds(displayTime);
		Debug.Log("Done Waiting");
		int nextIndex = 0;
		if(index < minMax) {
			nextIndex = index + 1;
		}
		Debug.Log("Next set! " + nextIndex );
		StartCoroutine(SetActive(nextIndex));
	}
}
