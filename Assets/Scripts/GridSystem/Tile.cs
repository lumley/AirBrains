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

	public void AcceptVisitor(TileVisitor visitor) {
		if (!visitors.ContainsKey (visitor.Tag)) {
			visitors.Add (visitor.Tag, new List<TileVisitor> ());
		}
		visitors [visitor.Tag].Add (visitor);
	}

	public void BidVisitorFarewell(TileVisitor visitor) {
		string tag = visitor.Tag;
		if (!visitors.ContainsKey (tag)) {
			Debug.LogError ("This tile doesn't contain the given visitor");
		}
		if (!visitors [tag].Contains (visitor)) {
			Debug.LogError ("This tile doesn't contain the given visitor");
		}
		visitors [tag].Remove (visitor);
		if (visitors [tag].Count <= 0) {
			visitors.Remove (tag);
		}
	}

	public int GetTotalNumberOfVisitors() {
		int total = 0;
		foreach(List<TileVisitor> list in visitors.Values){
			total += list.Count;
		}
		return total;
	}

	public int GetTotalNumberOfVisitors(string tag) {
		if (!visitors.ContainsKey (tag)) {
			return 0;
		}
		return visitors [tag].Count;
	}

	public List<TileVisitor> GetVisitorsOfTag(string tag) {
		if (!visitors.ContainsKey (tag)) {
			return new List<TileVisitor>();
		}
		return visitors [tag];
	}
}
