using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardTracker : MonoBehaviour {

	public int pointsToThreshold;
	public LeaderboardEntry prefab;

	public Transform prefabParent;
	public RectTransform thresholdBackground;
	public float perLineHeight = 98.18f;

	public bool launchOnStart = true;
	public bool liveUpdate = true;
	public Text FundsOrRoundText;
	public Color FundsOrRoundTextDefaultColor = Color.white;
	public Color FundsOrRoundTextRoundColor = Color.red;

	private Dictionary<ScoreTracker, LeaderboardEntry> leaderboardEntries = new Dictionary<ScoreTracker, LeaderboardEntry>();
	private float thresholdBackgroundSize = 0f;
	private float currentTBS = 0f;
	
	public static LeaderboardTracker FindInScene()
	{
		return FindObjectOfType<LeaderboardTracker>();
	}

	void Start () {
		thresholdBackgroundSize = perLineHeight;
		currentTBS = perLineHeight;
		if (launchOnStart) {
			OnGameStart ();
		}
	}

	public void OnGameStart() {
		GetTrackers ();
	}

	private void GetTrackers() { 
		leaderboardEntries.Clear ();
		foreach (ScoreTracker tracker in GameObject.FindObjectsOfType<ScoreTracker> ()) {
			LeaderboardEntry newEntry = Instantiate (prefab) as LeaderboardEntry;
			newEntry.SetScoreTracker (tracker);
			newEntry.gameObject.transform.SetParent(prefabParent);
			RectTransform newTransform = newEntry.gameObject.GetComponent<RectTransform> ();
			newTransform.localScale = Vector3.one;
			newTransform.anchoredPosition = Vector2.down * 12f * perLineHeight;
			leaderboardEntries.Add (tracker, newEntry);
		}
		SortEntries ();
	}

	private void SortEntries() {
		List<ScoreTracker> sortedTrackers = new List<ScoreTracker>(leaderboardEntries.Keys).OrderByDescending(score=>score.Score).ToList();
		float currentPos = perLineHeight;
		int playersInThreshold = 1;
		foreach (ScoreTracker tracker in sortedTrackers) {
			if (tracker.Score > pointsToThreshold) {
				playersInThreshold++;
			}
			leaderboardEntries [tracker].SetVerticalPositionTarget (currentPos);
			currentPos += perLineHeight;
		}
		thresholdBackgroundSize = playersInThreshold * perLineHeight;
	}

	void Update() {
		if (liveUpdate) {
			SortEntries ();
		}
		currentTBS = Mathf.Lerp (currentTBS, thresholdBackgroundSize, .3f);
		thresholdBackground.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, currentTBS);
	}

	public void TriggerScoreUpdate(){
		SortEntries ();
	}

	public void UpdateText(string text, bool isAfterThreshold)
	{
		FundsOrRoundText.text = text;
		FundsOrRoundText.color = isAfterThreshold ? FundsOrRoundTextRoundColor : FundsOrRoundTextDefaultColor;
	}
}
