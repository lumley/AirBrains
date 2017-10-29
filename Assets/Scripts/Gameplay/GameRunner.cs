using System.Collections;
using System.Collections.Generic;
using Gameplay.Player;
using ScreenLogic;
using UnityEngine;

public class GameRunner : MonoBehaviour
{
    public int turnsPerRound = 4;
    public float secondsToWaitForInput = 30f;
    public int scoreThreshold = 15;
    public int wrapUpRounds;

    public string tagForPoints = "human";

    public GameObject victoryScreen;

    private bool gameRunning = false;
    private bool isFirstRound = false;
    private int roundNumber = 0;
    private int lastRoundNumber = -1;
    private int currentTurn = 0;
    private float moveSelectionTimeRemaining = -1f;

    private List<MoveProvider> moveProviders = new List<MoveProvider>();
    private List<ScoreTracker> scoreTrackers = new List<ScoreTracker>();
    private List<PointsGiver> pointsGivers = new List<PointsGiver>();

    private Dictionary<MoveProvider, List<Move>> movesPerProvider = new Dictionary<MoveProvider, List<Move>>();
    private Dictionary<GameObject, Tile> originalPositions = new Dictionary<GameObject, Tile>();

    public static GameRunner FindInScene()
    {
        return FindObjectOfType<GameRunner>();
    }

    public void StartGame()
    {
        moveProviders.Clear();
        moveProviders.AddRange(FindObjectsOfType<MoveProvider>());
        scoreTrackers.Clear();
        scoreTrackers.AddRange(FindObjectsOfType<ScoreTracker>());
        pointsGivers.Clear();
        pointsGivers.AddRange(FindObjectsOfType<PointsGiver>());
        foreach (MoveProvider moveProvider in moveProviders)
        {
            moveProvider.SetMoveCount(turnsPerRound);
        }
        SetupGameVariables();
        StartCoroutine(RunGame());
    }

    private void SetupGameVariables()
    {
        isFirstRound = false;
        roundNumber = 0;
    }

    private IEnumerator RunGame()
    {
        if (gameRunning)
        {
            Debug.LogError("TRIED TO START TWO INSTANCES OF THE GAME OVER ITSELF!");
            yield break;
        }
        gameRunning = true;
        while (gameRunning)
        {
            roundNumber++;
            isFirstRound = roundNumber == 1;
            currentTurn = 0;
            Debug.Log("Starting Round " + roundNumber);
            yield return StartCoroutine(WaitForReadyOrTime());
            var airConsoleBridge = AirConsoleBridge.Instance;
            if (airConsoleBridge)
            {
                airConsoleBridge.BroadcastBlockRound();
            }
            yield return CollectPlayerInput();
            while (HaveTurnToProcess())
            {
                yield return ProcessTurn();
            }
            CollectPoints();
            yield return HandleRoundEndDisplay();
        }
        SetupVictoryScreen();
        yield return StartCoroutine(WaitToMoveOn());
    }

    private IEnumerator WaitForReadyOrTime()
    {
        bool readyToMoveOn = false;
        foreach (MoveProvider moveProvider in moveProviders)
        {
            moveProvider.StartCollectingMoves();
        }
        moveSelectionTimeRemaining = secondsToWaitForInput;
        FireEvent(StartCollectingMoves);
        while (!readyToMoveOn)
        {
            if (!isFirstRound)
            {
                if (moveSelectionTimeRemaining <= 0f)
                {
                    readyToMoveOn = true;
                }
            }
            else
            {
                moveSelectionTimeRemaining = -1f;
            }
            if (AreAllPlayersReady())
            {
                readyToMoveOn = true;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                readyToMoveOn = true;
            }
            moveSelectionTimeRemaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        moveSelectionTimeRemaining = -1f;
    }

    private bool AreAllPlayersReady()
    {
        foreach (MoveProvider provider in moveProviders)
        {
            if (!provider.FinishedPlanningMoves())
            {
                return false;
            }
        }
        Debug.Log("All players ready!");
        return true;
    }

    private IEnumerator CollectPlayerInput()
    {
        bool allInputCollected = false;
        movesPerProvider.Clear();

        while (!allInputCollected)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                allInputCollected = true;
            }
            foreach (MoveProvider provider in moveProviders)
            {
                if (movesPerProvider.ContainsKey(provider))
                {
                    continue;
                }
                movesPerProvider.Add(provider, provider.GetPlannedMoves());
            }
            allInputCollected = movesPerProvider.Count >= moveProviders.Count;
            if (!allInputCollected)
            {
                Debug.Log("Still waiting on " + (moveProviders.Count - movesPerProvider.Count) + " move providers");
            }
            yield return new WaitForEndOfFrame();
        }
        FireEvent(EndCollectingMoves);
    }

    private bool HaveTurnToProcess()
    {
        return currentTurn < turnsPerRound;
    }

    private IEnumerator ProcessTurn()
    {
        SaveOriginalPositions();
        UpdatePositionsBasedOnInput();

        //give time to walk somewhere
        yield return new WaitForSeconds(0.75f);

        CheckValidityAndHandlePushBacks();

        //give time to walk back
        yield return new WaitForSeconds(0.75f);

        ChangeOwnership();


        currentTurn++;
    }

    private void CollectPoints()
    {
        Dictionary<ScoreTracker, int> roundPointsForPlayer = new Dictionary<ScoreTracker, int>();
        foreach (PointsGiver giver in pointsGivers)
        {
            if (giver.OwnedBy != null)
            {
                if (!roundPointsForPlayer.ContainsKey(giver.OwnedBy))
                {
                    roundPointsForPlayer.Add(giver.OwnedBy, 0);
                }
                roundPointsForPlayer[giver.OwnedBy] += giver.pointValue;
            }
        }
        foreach (KeyValuePair<ScoreTracker, int> entry in roundPointsForPlayer)
        {
            entry.Key.ChangeScore(entry.Value);
            if (lastRoundNumber == -1 && entry.Key.Score >= scoreThreshold)
            {
                //TODO: trigger the wrap-up notification
                lastRoundNumber = roundNumber + wrapUpRounds;
            }
        }

        //DUMMY
        foreach (ScoreTracker tracker in scoreTrackers)
        {
            if (tracker.Score > 0)
            {
                Debug.Log(tracker.gameObject.name + " currently has " + tracker.Score + " points");
            }
        }
        //END DUMMY
    }

    private IEnumerator HandleRoundEndDisplay()
    {
        //TODO: SHOW THE PLAYERS THE RESULTS AT THE END OF THE ROUND
        if (lastRoundNumber > 0 && lastRoundNumber <= roundNumber)
        {
            gameRunning = false;
            CheckForTiedScores();
        }

        //DUMMY
        yield return new WaitForSeconds(.5f);
        //END DUMMY
    }

    private void SetupVictoryScreen()
    {
        var sortedScoreTrackers = new List<ScoreTracker>(scoreTrackers);
        sortedScoreTrackers.Sort((tracker1, tracker2) => tracker2.Score.CompareTo(tracker1.Score));
        if (sortedScoreTrackers.Count > 0)
        {
            for (var i = 0; i < moveProviders.Count; i++)
            {
                var moveProvider = moveProviders[i];
                var playerCharacter = moveProvider as IPlayerToGameStateBridge;
                if (playerCharacter != null)
                {
                    playerCharacter.SendFinishMessage(sortedScoreTrackers);
                }
            }
        }
        
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }
    }

    private IEnumerator WaitToMoveOn()
    {
        yield return new WaitForSeconds(5f); //Delay so that no one moves on before the gloating is over
        foreach (MoveProvider moveProvider in moveProviders)
        {
            moveProvider.StartCollectingMoves();
        }
        bool readyToMoveOn = false;
        while (!readyToMoveOn)
        {
            foreach (MoveProvider provider in moveProviders)
            {
                if (!(provider is RandomMoveProvider))
                {
                    readyToMoveOn = readyToMoveOn || provider.FinishedPlanningMoves();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                readyToMoveOn = true;
            }
            yield return new WaitForEndOfFrame();
        }
        GameStateController.FindInScene().HeadToTheLobby();
    }

    private void SaveOriginalPositions()
    {
        originalPositions.Clear();
        foreach (KeyValuePair<MoveProvider, List<Move>> entry in movesPerProvider)
        {
            TileVisitor visitor = entry.Key.gameObject.GetComponent<TileVisitor>();
            originalPositions.Add(visitor.gameObject, visitor.CurrentlyVisiting);
        }
    }

    private void UpdatePositionsBasedOnInput()
    {
        foreach (KeyValuePair<MoveProvider, List<Move>> entry in movesPerProvider)
        {
            Move currentMove = entry.Value[currentTurn];
            if (currentMove == Move.STAY)
            {
                continue;
            }
            TileVisitor visitor = entry.Key.gameObject.GetComponent<TileVisitor>();
            Direction moveIn = MoveUtils.GetDirectionFor(currentMove);
            if (visitor.CurrentlyVisiting.canMove(moveIn))
            {
                visitor.CurrentlyVisiting = visitor.CurrentlyVisiting.GetNeighbor(moveIn);

                visitor.GetComponent<CharacterAnimationController>().ApplyState(StateType.Walk, moveIn,
                    visitor.CurrentlyVisiting.transform.position);
            }
        }
    }

    private void CheckValidityAndHandlePushBacks()
    {
        int conflicts = 1;
        while (conflicts > 0)
        {
            conflicts = 0;
            List<TileVisitor> toPushBack = new List<TileVisitor>();
            foreach (KeyValuePair<GameObject, Tile> pair in originalPositions)
            {
                GameObject dude = pair.Key;
                TileVisitor tileVisitor = dude.GetComponent<TileVisitor>();
                if (tileVisitor.CurrentlyVisiting.GetTotalNumberOfVisitors(tileVisitor.Tag) > 1)
                {
                    //TODO: Mark this as a conflict, and notify the animation handler...somehow, so we can play the fight animations
                    conflicts++;
                    toPushBack.AddRange(tileVisitor.CurrentlyVisiting.GetVisitorsOfTag(tileVisitor.Tag));
                }
            }
            foreach (TileVisitor visitor in toPushBack)
            {
                if (originalPositions.ContainsKey(visitor.gameObject))
                {
                    Tile sourceTile = originalPositions[visitor.gameObject];
                    if (visitor.CurrentlyVisiting != sourceTile)
                    {
                        //sorry, you need to walk back.... need to get direction somehow
                        Direction moveToDirection =
                            visitor.CurrentlyVisiting.GetMoveDirectionForNeighbor(
                                originalPositions[visitor.gameObject]);

                        visitor.CurrentlyVisiting = originalPositions[visitor.gameObject];

                        visitor.GetComponent<CharacterAnimationController>().ApplyState(StateType.Walk, moveToDirection,
                            visitor.CurrentlyVisiting.transform.position);
                    }
                }
                else
                {
                    Debug.LogError("Unable to push back " + visitor.gameObject.name +
                                   " because their original position isn't known!");
                }
            }
        }
    }

    private void ChangeOwnership()
    {
        for (var i = 0; i < scoreTrackers.Count; i++)
        {
            ScoreTracker tracker = scoreTrackers[i];
            TileVisitor trackerVisitor = tracker.gameObject.GetComponent<TileVisitor>();
            if (trackerVisitor.CurrentlyVisiting.GetTotalNumberOfVisitors(tagForPoints) > 0)
            {
                foreach (TileVisitor otherOccupant in trackerVisitor.CurrentlyVisiting.GetVisitorsOfTag(tagForPoints))
                {
                    PointsGiver pointsGiver = otherOccupant.gameObject.GetComponent<PointsGiver>();
                    if (pointsGiver != null)
                    {
                        if (pointsGiver.OwnedBy != tracker)
                        {
                            //TODO: Handle line of sight exclusion!
                            pointsGiver.OwnedBy = tracker;

                            tracker.GetComponent<CharacterAnimationController>().ApplyState(StateType.Sticker,
                                pointsGiver.GetComponent<HumanAnimationController>());
                        }
                    }
                    else
                    {
                        Debug.LogError("Gameobject: " + otherOccupant.gameObject.name + " is tagged as " +
                                       tagForPoints +
                                       " but doesn't have a pointsgiver object!");
                    }
                }
            }
        }
    }

    private void CheckForTiedScores()
    {
        int currentHighscore = 0;
        int numberWithHighscore = 0;
        foreach (ScoreTracker tracker in scoreTrackers)
        {
            if (tracker.Score > currentHighscore)
            {
                currentHighscore = tracker.Score;
                numberWithHighscore = 1;
            }
            else if (tracker.Score == currentHighscore)
            {
                numberWithHighscore++;
            }
        }
        if (numberWithHighscore > 1)
        {
            gameRunning = true;
        }
    }

    public float TimeLeft
    {
        get { return moveSelectionTimeRemaining; }
    }

    private void FireEvent(GameEvent myEvent)
    {
        if (myEvent != null)
        {
            myEvent.Invoke();
        }
    }

    public event GameEvent StartCollectingMoves;
    public event GameEvent EndCollectingMoves;
}

public delegate void GameEvent();