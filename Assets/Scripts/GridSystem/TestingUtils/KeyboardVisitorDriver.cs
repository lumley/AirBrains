using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardVisitorDriver : MonoBehaviour {

	public GridBuilder grid;
	public KeyCode upCode;
	public KeyCode downCode;
	public KeyCode leftCode;
	public KeyCode rightCode;

	private TileVisitor myVisitor;

	void Start () {
		myVisitor = GetComponent<TileVisitor> ();
		if (myVisitor == null) {
			Debug.LogError ("I need a TileVisitor to drive!");
		}
		Tile newTile = grid.GetTileAt (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y));
		myVisitor.CurrentlyVisiting = newTile;
	}

	void Update() {
		if (Input.GetKeyDown (upCode)) {
			SetPositionIfValid (Direction.NORTH);
		} else if (Input.GetKeyDown (downCode)) {
			SetPositionIfValid (Direction.SOUTH);
		} else if (Input.GetKeyDown (leftCode)) {
			SetPositionIfValid (Direction.WEST);
		} else if (Input.GetKeyDown (rightCode)) {
			SetPositionIfValid (Direction.EAST);
		}
	}

	private void SetPositionIfValid(Direction direction){
		if (myVisitor.CurrentlyVisiting.canMove (direction)) {
			myVisitor.CurrentlyVisiting = myVisitor.CurrentlyVisiting.GetNeighbor (direction);
		}
	}
}
