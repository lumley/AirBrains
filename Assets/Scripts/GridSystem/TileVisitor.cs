using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisitor : MonoBehaviour {

	public string Tag;
	public Vector3 offset;

	protected Tile currentlyVisiting;
	public Tile CurrentlyVisiting {
		get { return currentlyVisiting; }
		set {
			if (currentlyVisiting != null) {
				currentlyVisiting.BidVisitorFarewell (this);
			}
			currentlyVisiting = value;
			currentlyVisiting.AcceptVisitor (this);
		}
	}
}
