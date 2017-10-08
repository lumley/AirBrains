using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveProvider : MoveProvider {

	private List<Move> myMoves = new List<Move> ();
	private System.Random random = new System.Random();
	private Array moveValues = Enum.GetValues(typeof(Move));

	public override void StartCollectingMoves(){
		myMoves.Clear ();
		for (int moveIndex = 0; moveIndex < numberOfMovesPerRound; moveIndex++) {
			Move randomMove = (Move)moveValues.GetValue(random.Next(moveValues.Length));
			myMoves.Add (randomMove);
		}
	}
	public override bool FinishedPlanningMoves () {
		return myMoves.Count == numberOfMovesPerRound;
	}
	
	public override List<Move> GetPlannedMoves() {
		return myMoves;
	}
}
