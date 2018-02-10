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
		gameObject.GetComponent<RectTransform>().anchoredPosition3D = targetCharacter.transform.position;
	}

	void Update() {
		if(delay >= 0) {
			delay -= Time.deltaTime;
			if(delay <= 0) {
				StartCoroutine(StartEffect());
			}
		}
	}

	private IEnumerator StartEffect() {
		portrait.gameObject.SetActive(true);
		nameplate.gameObject.SetActive(true);
		gameObject.GetComponent<RectTransform>().anchoredPosition3D = targetCharacter.transform.position;
		yield return new WaitForSeconds(displayTime);
		Destroy(this);
	}
}
