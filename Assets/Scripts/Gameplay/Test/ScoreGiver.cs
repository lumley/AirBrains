using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGiver : MonoBehaviour {

	public int updateScore = 0;

	private ScoreTracker myTracker;

	void Start () {
		myTracker = gameObject.GetComponent<ScoreTracker> ();
	}

	void Update () {
		myTracker.ChangeScore (updateScore);
		updateScore = 0;
	}
}
