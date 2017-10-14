using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunner : MonoBehaviour {

	public int turnsPerRound = 4;
	public float secondsToWaitForInput = 30f;
	public int scoreThreshold = 15;
	public int wrapUpRounds;

	public string tagForPoints = "human";

	private bool gameRunning = false;
	private bool isFirstRound = false;
	private int roundNumber = 0;
	private int lastRoundNumber = -1;
	private int currentTurn = 0;

	private List<MoveProvider> moveProviders = new List<MoveProvider>();
	private List<ScoreTracker> scoreTrackers = new List<ScoreTracker>();
	private List<PointsGiver> pointsGivers = new List<PointsGiver>();

	private Dictionary<MoveProvider, List<Move>> movesPerProvider = new Dictionary<MoveProvider, List<Move>>();
	private Dictionary<GameObject, Tile> originalPositions = new Dictionary<GameObject, Tile>();

	public void StartGame() {
		moveProviders.Clear ();
		moveProviders.AddRange(GameObject.FindObjectsOfType<MoveProvider> ());
		scoreTrackers.Clear ();
		scoreTrackers.AddRange (GameObject.FindObjectsOfType<ScoreTracker> ());
		pointsGivers.Clear ();
		pointsGivers.AddRange (GameObject.FindObjectsOfType<PointsGiver> ());
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
		FireEvent (StartCollectingMoves);
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
		yield return new WaitForSeconds (1f); //TODO: REMOVE! JUST FOR TESTING THE THOUGHT BUBBLE EFFECT!
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
		FireEvent (EndCollectingMoves);
	}

	private bool HaveTurnToProcess() {
		return currentTurn < turnsPerRound;
	}

	private IEnumerator ProcessTurn() {
		SaveOriginalPositions();
		UpdatePositionsBasedOnInput ();
		CheckValidityAndHandlePushBacks ();
		ChangeOwnership ();

		//DUMMY
		yield return new WaitForSeconds(.1f);
		//END DUMMY

		currentTurn++;
	}

	private void CollectPoints() {
		Dictionary<ScoreTracker, int> roundPointsForPlayer = new Dictionary<ScoreTracker, int> ();
		foreach (PointsGiver giver in pointsGivers) {
			if (giver.OwnedBy != null) {
				if (!roundPointsForPlayer.ContainsKey (giver.OwnedBy)) {
					roundPointsForPlayer.Add (giver.OwnedBy, 0);
				}
				roundPointsForPlayer [giver.OwnedBy] += giver.pointValue;
			}
		}
		foreach (KeyValuePair<ScoreTracker, int> entry in roundPointsForPlayer) {
			entry.Key.ChangeScore (entry.Value);
			if (lastRoundNumber == -1 && entry.Key.Score >= scoreThreshold) {
				//TODO: trigger the wrap-up notification
				lastRoundNumber = roundNumber + wrapUpRounds;
			}
		}

		//DUMMY
		foreach (ScoreTracker tracker in scoreTrackers) {
			if (tracker.Score > 0) {
				Debug.Log (tracker.gameObject.name + " currently has " + tracker.Score + " points");
			}
		}
		//END DUMMY
	}

	private IEnumerator HandleRoundEndDisplay() {
		//TODO: SHOW THE PLAYERS THE RESULTS AT THE END OF THE ROUND
		if (lastRoundNumber > 0 && lastRoundNumber <= roundNumber) {
			gameRunning = false;
			CheckForTiedScores ();
		}

		//DUMMY
		yield return new WaitForSeconds(.5f);
		//END DUMMY
	}

	private void SetupVictoryScreen() {
		ScoreTracker highestScore = null;

		foreach (ScoreTracker tracker in scoreTrackers) {
			if (highestScore == null || tracker.Score > highestScore.Score) {
				highestScore = tracker;
			}
		}
		Debug.Log (highestScore.gameObject.name + " wins with " + highestScore.Score + " points!");
		//TODO: Set up the victory screen
	}

	private void SaveOriginalPositions() {
		originalPositions.Clear ();
		foreach (KeyValuePair<MoveProvider, List<Move>> entry in movesPerProvider) {
			TileVisitor visitor = entry.Key.gameObject.GetComponent<TileVisitor> ();
			originalPositions.Add (visitor.gameObject, visitor.CurrentlyVisiting);
		}
	}

	private void UpdatePositionsBasedOnInput() {
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
	}

	private void CheckValidityAndHandlePushBacks() {
		int conflicts = 1;
		while (conflicts > 0) {
			conflicts = 0;
			List<TileVisitor> toPushBack = new List<TileVisitor> ();
			foreach (KeyValuePair<GameObject, Tile> pair in originalPositions) {
				GameObject dude = pair.Key;
				TileVisitor tileVisitor = dude.GetComponent<TileVisitor> ();
				if (tileVisitor.CurrentlyVisiting.GetTotalNumberOfVisitors (tileVisitor.Tag) > 1) {
					//TODO: Mark this as a conflict, and notify the animation handler...somehow, so we can play the fight animations
					conflicts++;
					toPushBack.AddRange(tileVisitor.CurrentlyVisiting.GetVisitorsOfTag (tileVisitor.Tag));
				}
			}
			foreach (TileVisitor visitor in toPushBack) {
				if (originalPositions.ContainsKey (visitor.gameObject)) {
					Tile sourceTile = originalPositions [visitor.gameObject];
					if (visitor.CurrentlyVisiting != sourceTile) {
						visitor.CurrentlyVisiting = originalPositions [visitor.gameObject];
					}
				} else {
					Debug.LogError ("Unable to push back " + visitor.gameObject.name + " because their original position isn't known!");
				}
			}
		}
	}

	private void ChangeOwnership(){
		foreach (ScoreTracker tracker in scoreTrackers) {
			TileVisitor trackerVisitor = tracker.gameObject.GetComponent<TileVisitor> ();
			if (trackerVisitor.CurrentlyVisiting.GetTotalNumberOfVisitors (tagForPoints) > 0) {
				foreach (TileVisitor otherOccupant in trackerVisitor.CurrentlyVisiting.GetVisitorsOfTag(tagForPoints)) {
					PointsGiver pointsGiver = otherOccupant.gameObject.GetComponent<PointsGiver> ();
					if (pointsGiver != null) {
						//TODO: Handle line of sight exclusion!
						pointsGiver.OwnedBy = tracker;
					} else {
						Debug.LogError ("Gameobject: " + otherOccupant.gameObject.name + " is tagged as " + tagForPoints + " but doesn't have a pointsgiver object!");
					}
				}
			}
		}
	}

	private void CheckForTiedScores() {
		int currentHighscore = 0;
		int numberWithHighscore = 0;
		foreach (ScoreTracker tracker in scoreTrackers) {
			if (tracker.Score > currentHighscore) {
				currentHighscore = tracker.Score;
				numberWithHighscore = 1;
			} else if (tracker.Score == currentHighscore) {
				numberWithHighscore++;
			}
		}
		if (numberWithHighscore > 1) {
			gameRunning = true;
		}
	}

	private void FireEvent(GameEvent myEvent) {
		if (myEvent != null) {
			myEvent.Invoke ();
		}
	}

	public event GameEvent StartCollectingMoves;
	public event GameEvent EndCollectingMoves;
}

public delegate void GameEvent();
