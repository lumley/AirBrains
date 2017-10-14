using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpingVisitor : TileVisitor {

	void Update()
	{
		return;
		transform.position = Vector3.Lerp (transform.position, currentlyVisiting.transform.position + offset, .5f);
	}
}
