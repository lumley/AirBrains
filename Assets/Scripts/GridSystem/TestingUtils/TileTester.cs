using System;
using UnityEngine;

public class TileTester : MonoBehaviour {

	public Material normalMat;
	public Material highlightMat;

	private Tile myTile;

	void Start(){
		myTile = GetComponent<Tile> ();
		if (myTile == null) {
			Debug.LogWarning ("No Tile found ont Tile Tester");
		}
	}

	void OnMouseDown(){
		foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
			if (myTile.canMove(direction)) {
				myTile.GetNeighbor (direction).GetComponent<Renderer> ().sharedMaterial = highlightMat;
			}
		}
		GetComponent<Renderer> ().sharedMaterial = highlightMat;
	}

	void OnMouseUp(){
		foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
			if (myTile.GetNeighbor (direction) != null) {
				myTile.GetNeighbor (direction).GetComponent<Renderer> ().sharedMaterial = normalMat;
			}
		}
		GetComponent<Renderer> ().sharedMaterial = normalMat;
	}
}
