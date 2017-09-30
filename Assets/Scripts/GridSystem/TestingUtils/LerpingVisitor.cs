using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpingVisitor : TileVisitor {

	void Update() { 
		transform.position = Vector3.Lerp (transform.position, currentlyVisiting.transform.position + offset, .5f);
	}
}
