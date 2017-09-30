using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

	public Vector2 gridSize;

	Dictionary<int, Dictionary<int, Tile>> tileMap = new Dictionary<int, Dictionary<int, Tile>>();

	void Awake() 
	{
		int minX = int.MaxValue;
		int minY = int.MaxValue;
		int maxX = int.MinValue;
		int maxY = int.MinValue;
		foreach (Tile tile in gameObject.GetComponentsInChildren<Tile>()) {
			int xPos = Mathf.RoundToInt (tile.transform.position.x / gridSize.x);
			int yPos = Mathf.RoundToInt (tile.transform.position.y / gridSize.y);
			if (xPos < minX) {
				minX = xPos;
			}
			if (yPos < minY) {
				minY = yPos;
			}
			if (xPos > maxX) {
				maxX = xPos;
			}
			if (yPos > maxY) {
				maxY = yPos;
			}
			if (!tileMap.ContainsKey (xPos)) {
				Dictionary<int, Tile> column = new Dictionary<int, Tile> ();
				tileMap.Add (xPos, column);
			}
			if (GetTileAt (xPos, yPos) != null) {
				Debug.LogError ("Multiple tiles found for position (" + xPos + ", " + yPos + ")");
			} else {
				tileMap [xPos].Add (yPos, tile);
			}
		}
		for (int xPos = minX; xPos <= maxX; xPos++) {
			for (int yPos = minY; yPos <= maxY; yPos++) {
				Tile thisTile = GetTileAt (xPos, yPos);
				if (thisTile != null) {
					thisTile.SetNeighbors (GetTileFrom (xPos, yPos, Direction.NORTH),
						GetTileFrom (xPos, yPos, Direction.SOUTH),
						GetTileFrom (xPos, yPos, Direction.WEST),
						GetTileFrom (xPos, yPos, Direction.EAST));
					thisTile.transform.SetPositionAndRotation (new Vector3 (xPos * gridSize.x, yPos * gridSize.y, 0f), Quaternion.identity);
				}
			}
		}
	}

	public Tile GetTileFrom(int xPos, int yPos, Direction direction){
		int checkPosX = xPos;
		int checkPosY = yPos;
		if(direction == Direction.NORTH){
			checkPosY += 1;
		}else if(direction == Direction.SOUTH){
			checkPosY -= 1;
		}else if(direction == Direction.WEST){
			checkPosX -= 1;
		}else if(direction == Direction.EAST){
			checkPosX += 1;
		}
		return GetTileAt (checkPosX, checkPosY);
	}

	public Tile GetTileAt(int xPos, int yPos){
		if(!tileMap.ContainsKey(xPos)){
			return null;
		}
		if(!tileMap[xPos].ContainsKey(yPos)){
			return null;
		}
		return tileMap [xPos] [yPos];
	}
}
