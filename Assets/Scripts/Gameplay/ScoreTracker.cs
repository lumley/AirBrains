using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour {

	protected int currentScore;

	public int DonorCountInCurrentRound { get; set; }

	public int Score {
		get { return currentScore; }
	}

	public CharacterType Character {
		get;
		set;
	}

	void Start() {
		currentScore = 0;
	}

	public int ChangeScore(int delta) {
		currentScore += delta;
		return currentScore;
	}

}
