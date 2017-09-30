using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public bool isBlocked;
	public bool topBlocked;
	public bool leftBlocked;

	private Dictionary<Direction, Tile> neighbors = new Dictionary<Direction, Tile>();

	private Dictionary<string, List<TileVisitor>> visitors = new Dictionary<string, List<TileVisitor>>();

	public void SetNeighbors(Tile north, Tile south, Tile west, Tile east)
	{
		if (north != null) {
			neighbors.Add (Direction.NORTH, north);
		}
		if (south != null) {
			neighbors.Add (Direction.SOUTH, south);
		}
		if (west != null) {
			neighbors.Add (Direction.WEST, west);
		}
		if (east != null) {
			neighbors.Add (Direction.EAST, east);
		}
	}

	public bool canMove(Direction direction){
		if (isBlocked) {
			return false;
		}
		if (!neighbors.ContainsKey (direction)) {
			return false;
		}
		Tile neighbor = GetNeighbor (direction);
		if (neighbor.isBlocked) {
			return false;
		}
		if (direction == Direction.NORTH) {
			return !topBlocked;
		}
		if (direction == Direction.SOUTH) {
			return !neighbors[direction].topBlocked;
		}
		if (direction == Direction.WEST) {
			return !leftBlocked;
		}
		if (direction == Direction.EAST) {
			return !neighbors[direction].leftBlocked;
		}
		return false;
	}

	public Tile GetNeighbor(Direction direction) {
		if(!neighbors.ContainsKey(direction)) {
			return null;
		}
		return neighbors [direction];
	}
}
