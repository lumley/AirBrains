using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReplacer : MonoBehaviour {

	public GameObject prefabToSpawn;
	public bool replaceOnAwake = false;

	void Awake() {
		if (replaceOnAwake) {
			SpawnPrefab ();
		}
	}

	public GameObject SpawnPrefab() {
		GameObject newObject = Instantiate (prefabToSpawn);
		newObject.transform.SetParent (transform.parent);
		newObject.transform.localPosition = transform.localPosition;
		Destroy (gameObject);
		return newObject;
	}
}
