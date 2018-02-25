using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPositionEffect : MonoBehaviour {

	public float displayTime = 10f;

	public List<CharacterType> characterPortraitLinks;
	public List<Sprite> portraitCharacterLinks;
	public List<Sprite> portraitNameBoxLinks;
	public List<string> characterNameLinks;

	public Image portrait;
	public Image nameplate;
	public Text characterName;
	public Text playerName;

	private GameObject targetCharacter;
	private float delay = float.MaxValue;
	private Vector3 targetPosition;
	private Vector3 initialPosition;
	
	public void SetCharacter(CharacterType newCharacter, GameObject targetCharacter, string playersName, float delay) {
		Debug.Log("New effect Loaded! " + newCharacter + " " + targetCharacter + " " + playerName + " " + delay);
		int characterId = characterPortraitLinks.IndexOf(newCharacter);
		Debug.Log("Character ID: " + characterId);
		portrait.sprite = portraitCharacterLinks[characterId];
		nameplate.sprite = portraitNameBoxLinks[characterId];
		characterName.text = characterNameLinks[characterId];
		playerName.text = playersName;
		this.targetCharacter = targetCharacter;
		portrait.gameObject.SetActive(false);
		nameplate.gameObject.SetActive(false);
		this.delay = delay;
		transform.localScale = Vector3.one;
		targetPosition = Camera.main.WorldToScreenPoint(targetCharacter.transform.position);
		StartCoroutine(StartEffect());
	}

	private IEnumerator StartEffect() {
		yield return new WaitForSeconds(delay);
		portrait.gameObject.SetActive(true);
		nameplate.gameObject.SetActive(true);
		float totalTime = 0f;
		initialPosition = gameObject.GetComponent<RectTransform>().anchoredPosition3D;
		yield return new WaitForSeconds(displayTime / 3f);
		while(totalTime < displayTime / 3f) {
			gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(initialPosition, targetPosition, totalTime / (displayTime / 3f));
			totalTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		totalTime = 0f;
		while(totalTime < displayTime / 3f) {
			transform.localScale = Vector3.one * (1f - (totalTime / (displayTime / 3f)));
			totalTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Destroy(this.gameObject);
	}
}
