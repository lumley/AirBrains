using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveProvider : MoveProvider {

	private List<Move> myMoves = new List<Move> ();
	private Array moveValues = Enum.GetValues(typeof(Move));

	public override void StartCollectingMoves(){
		myMoves.Clear ();
		string moves = "";
		for (int moveIndex = 0; moveIndex < numberOfMovesPerRound; moveIndex++) {
			Move randomMove = (Move)moveValues.GetValue(UnityEngine.Random.Range(0, moveValues.Length));
			myMoves.Add (randomMove);
			moves += randomMove + " ";
		}
		Debug.Log ("GOT MOVE REQUEST FOR : " + gameObject.name + " moves: " + moves);
	}
	public override bool FinishedPlanningMoves () {
		return myMoves.Count == numberOfMovesPerRound;
	}
	
	public override List<Move> GetPlannedMoves() {
		return myMoves;
	}
}
