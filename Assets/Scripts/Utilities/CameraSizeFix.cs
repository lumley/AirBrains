using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeFix : MonoBehaviour {

	public float ratio = 1.6f;
	public float newOrthoSize = 8.2f;

	void Start () {
		if(((float)Screen.width / (float)Screen.height) < ratio){
			if(gameObject.GetComponent<Camera>() == null) return;
			gameObject.GetComponent<Camera>().orthographicSize = newOrthoSize;
		}
	}
}
