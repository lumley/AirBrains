using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorCounter : MonoBehaviour {

	public int expectedVisitors = 1;
	public bool spam = false;

	void Update () {
		int totalVisitors = 0;
		foreach (Tile tile in gameObject.GetComponentsInChildren<Tile>()) {
			totalVisitors += tile.GetTotalNumberOfVisitors ();
		}
		if (expectedVisitors != totalVisitors) {
			Debug.LogError ("THERE ARE " + totalVisitors + " COUNTED VISITORS! EXPECTED " + expectedVisitors);
		}
		if (spam) {
			Debug.Log ("THERE ARE " + totalVisitors + " TOTAL VISITORS");
		}
	}
}
