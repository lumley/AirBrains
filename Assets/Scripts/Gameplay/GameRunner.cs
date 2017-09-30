using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public float secondsToWaitForInput = 30f;
	public int wrapUpRounds;

	private bool gameRunning = false;
	private bool isFirstRound = false;
	private int roundNumber = 0;
	private int lastRoundNumber = -1;

	public void StartGame() {
		SetupGameVariables ();
		StartCoroutine (RunGame ());
	}

	private void SetupGameVariables() {
		isFirstRound = false;
		roundNumber = 0;
	}

	private IEnumerator RunGame() {
		if (gameRunning) {
			Debug.LogError ("TRIED TO START TWO INSTANCES OF THE GAME OVER ITSELF!");
			yield break;
		}
		gameRunning = true;
		while (gameRunning) {
			roundNumber++;
			isFirstRound = roundNumber == 1;
			yield return StartCoroutine (WaitForReadyOrTime ());
			yield return CollectPlayerInput ();
			while (HaveTurnToProcess ()) {
				yield return ProcessTurn ();
			}
			CollectPoints ();
			yield return HandleRoundEndDisplay ();
		}
		SetupVictoryScreen ();
	}

	private IEnumerator WaitForReadyOrTime() {
		bool readyToMoveOn = false;
		float startTime = Time.timeSinceLevelLoad;
		while (!readyToMoveOn) {
			if (!isFirstRound) {
				if (Time.timeSinceLevelLoad - startTime > secondsToWaitForInput) {
					readyToMoveOn = true;
				}
			}
			if (AreAllPlayersReady()) {
				readyToMoveOn = true;
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				readyToMoveOn = true;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	private bool AreAllPlayersReady() {
		//TODO: DO THIS!
		return false;
	}

	private IEnumerator CollectPlayerInput() {
		bool allInputCollected = false;
		while (!allInputCollected) {
			//TODO: DO THIS
			if (Input.GetKeyDown (KeyCode.Escape)) {
				allInputCollected = true;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	private bool HaveTurnToProcess() {
		//TODO: GET THIS INFO FROM...SOMEWHERE
		return false;
	}

	private IEnumerator ProcessTurn() {
		//TODO:
		//Save original positions
		//Update positions based on input
		//Check validity, and make push-backs if the occupancy rules are violated
		//Handle the conversions of humans to donors
		yield return 0;
	}

	private void CollectPoints() {
		//TODO:
		//Add the number of donors to the points that each player currently has
		//Check and see if anyone has crossed the funding threshold. If they have, set the last round number
	}

	private IEnumerator HandleRoundEndDisplay() {
		//TODO: SHOW THE PLAYERS THE RESULTS AT THE END OF THE ROUND
		if (lastRoundNumber > 0 && lastRoundNumber >= roundNumber) {
			gameRunning = false;
		}
		yield return 0;
	}

	private void SetupVictoryScreen() {
		//TODO: DO THIS
	}
}
