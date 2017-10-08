using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsGiver : MonoBehaviour {

	public int pointValue;

	private ScoreTracker ownedBy;
	public ScoreTracker OwnedBy {
		get { return ownedBy; }
		set { ownedBy = value; }
	}

	void OnDrawGizmos() {
		if (ownedBy != null) {
			Gizmos.DrawLine (transform.position, ownedBy.transform.position);
		}
	}
}
