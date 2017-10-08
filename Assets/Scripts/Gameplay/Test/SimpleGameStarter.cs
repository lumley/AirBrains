using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<GameRunner> ().StartGame ();
		
	}
}
