using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour {

	private Dictionary<Move, Sprite> spritesPerMove = new Dictionary<Move, Sprite>();

	private MoveProvider myProvider;

	public SpriteRenderer[] displaySprites;
	public SpriteRenderer[] moveSprites;
	public float fadeTime;

	public Sprite leftSprite;
	public Sprite rightSprite;
	public Sprite upSprite;
	public Sprite downSprite;
	public Sprite waitSprite;

	void Start() { 
		spritesPerMove.Add (Move.UP, upSprite);
		spritesPerMove.Add (Move.RIGHT, rightSprite);
		spritesPerMove.Add (Move.LEFT, leftSprite);
		spritesPerMove.Add (Move.DOWN, downSprite);
		spritesPerMove.Add (Move.STAY, waitSprite);

		GameRunner runner = GameObject.FindObjectOfType<GameRunner> ();
		if (runner == null) {
			Debug.LogError ("No GameRunner Found!");
		}else{
			runner.StartCollectingMoves += new GameEvent(StartFadeIn);
			runner.EndCollectingMoves += new GameEvent(StartFadeOut);
		}

		myProvider = gameObject.GetComponent<MoveProvider> ();
	}

	public void SetMove(int index, Move move) {
		if (index >= moveSprites.Length) {
			Debug.LogError ("Index " + index + "is too large!");
			return;
		}
		if (!spritesPerMove.ContainsKey (move)) {
			Debug.LogError ("No Sprite found for Move " + move);
			return;
		}
		moveSprites [index].sprite = spritesPerMove [move];
	}

	public void StartFadeIn() {
		StopAllCoroutines ();
		StartCoroutine (CollectMoves ());
		StartCoroutine (FadeIn());
	}

	public IEnumerator CollectMoves() { 
		yield return new WaitForEndOfFrame ();
		List<Move> moves = myProvider.GetPlannedMoves ();
		for (int index = 0; index < moveSprites.Length; index++) {
			SetMove (index, moves [index]);
		}
	}

	public void StartFadeOut() {
		StopAllCoroutines ();
		StartCoroutine (FadeOut());
	}

	private IEnumerator FadeIn() {
		float startAlpha = displaySprites [0].color.a;
		float time = 0f;
		while (time <= fadeTime) {
			SetAllAlphas (Mathf.Lerp(startAlpha, 1f, time / fadeTime));
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		SetAllAlphas (1f);
	}

	private IEnumerator FadeOut() {
		float startAlpha = displaySprites [0].color.a;
		float time = 0f;
		while (time <= fadeTime) {
			SetAllAlphas (Mathf.Lerp(startAlpha, 0f, time / fadeTime));
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		SetAllAlphas (0f);
	}

	private void SetAllAlphas(float newAlpha){
		Color newColor = new Color (1f, 1f, 1f, newAlpha);
		foreach (SpriteRenderer renderer in displaySprites) { 
			renderer.color = newColor;
		}
	}

}
