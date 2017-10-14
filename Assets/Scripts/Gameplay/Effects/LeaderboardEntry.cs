using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour {

	public Image portrait;
	public Text scoreText;
	public string beforeText = "$";
	public string afterText = "k";

	private ScoreTracker tracker;
	private RectTransform _transform;
	private float targetYPos = 0f;
	private int displayScore = 0;

	void Start() { 
		_transform = GetComponent<RectTransform> ();
	}

	public void SetScoreTracker(ScoreTracker newTracker) {
		tracker = newTracker;
		scoreText.text = tracker.Score.ToString ();
	}

	public void SetVerticalPositionTarget(float yPos) {
		targetYPos = yPos;
	}

	void Update(){
		displayScore = tracker.Score;
		if (Mathf.Abs(_transform.anchoredPosition.y + targetYPos) > .25f) {
			_transform.anchoredPosition = Vector2.Lerp (_transform.anchoredPosition, Vector2.down * targetYPos, .3f);
		}
		scoreText.text = beforeText + displayScore.ToString() + afterText;
	}
}
