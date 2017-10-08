using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveProvider : MonoBehaviour {

	protected int numberOfMovesPerRound = 0;

	public abstract void StartCollectingMoves();
	public abstract bool FinishedPlanningMoves ();
	public abstract List<Move> GetPlannedMoves();
	public void SetMoveCount(int movecount) {
		numberOfMovesPerRound = movecount;
	}
}
