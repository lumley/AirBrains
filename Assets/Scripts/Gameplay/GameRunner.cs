using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunner : MonoBehaviour {

	public int turnsPerRound = 4;
	public float secondsToWaitForInput = 30f;
	public int wrapUpRounds;

	private bool gameRunning = false;
	private bool isFirstRound = false;
	private int roundNumber = 0;
	private int lastRoundNumber = -1;
	private int currentTurn = 0;

	private List<MoveProvider> moveProviders = new List<MoveProvider>();
	private Dictionary<MoveProvider, List<Move>> movesPerProvider = new Dictionary<MoveProvider, List<Move>>();

	public void StartGame() {
		moveProviders.Clear ();
		moveProviders.AddRange(GameObject.FindObjectsOfType<MoveProvider> ());
		foreach (MoveProvider moveProvider in moveProviders) {
			moveProvider.SetMoveCount (turnsPerRound);
		}
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
			currentTurn = 0;
			Debug.Log ("Starting Round " + roundNumber);
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
		foreach (MoveProvider moveProvider in moveProviders) {
			moveProvider.StartCollectingMoves ();
		}
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
		foreach (MoveProvider provider in moveProviders) {
			if (!provider.FinishedPlanningMoves ()) {
				return false;
			}
		}
		Debug.Log ("All players ready!");
		return true;
	}

	private IEnumerator CollectPlayerInput() {
		bool allInputCollected = false;
		movesPerProvider.Clear ();

		while (!allInputCollected) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				allInputCollected = true;
			}
			foreach (MoveProvider provider in moveProviders) {
				if (movesPerProvider.ContainsKey (provider)) {
					continue;
				}
				movesPerProvider.Add (provider, provider.GetPlannedMoves ());
			}
			allInputCollected = movesPerProvider.Count >= moveProviders.Count;
			yield return new WaitForEndOfFrame ();
		}
	}

	private bool HaveTurnToProcess() {
		return currentTurn < turnsPerRound;
	}

	private IEnumerator ProcessTurn() {
		//TODO:
		//Save original positions
		//Update positions based on input
		//Check validity, and make push-backs if the occupancy rules are violated
		//Handle the conversions of humans to donors
		foreach (KeyValuePair<MoveProvider, List<Move>> entry in movesPerProvider) {
			Move currentMove = entry.Value [currentTurn];
			if (currentMove == Move.STAY) {
				continue;
			}
			TileVisitor visitor = entry.Key.gameObject.GetComponent<TileVisitor> ();
			Direction moveIn = MoveUtils.GetDirectionFor (currentMove);
			if (visitor.CurrentlyVisiting.canMove (moveIn)) {
				visitor.CurrentlyVisiting = visitor.CurrentlyVisiting.GetNeighbor (moveIn);
			}
		}
		yield return new WaitForSeconds(1f);
		currentTurn++;
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
