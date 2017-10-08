using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutowiredVisitor : MonoBehaviour {

	private TileVisitor myVisitor;

	void Start () {
		GridBuilder grid = GameObject.FindObjectOfType<GridBuilder> ();
		myVisitor = GetComponent<TileVisitor> ();
		if (myVisitor == null) {
			Debug.LogError ("I need a TileVisitor to drive!");
		}
		Tile newTile = grid.GetTileAt (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y));
		myVisitor.CurrentlyVisiting = newTile;
	}
}
