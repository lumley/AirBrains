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

	public SpriteRenderer portraitRenderer;
	public SpriteRenderer nameplateRenderer;
	public TextMesh characterNameTextMesh;
	public TextMesh playerNameTextMesh;

	public string sortingLayer;
	public int sortingLayerIndex;

	private GameObject targetCharacter;
	private float delay = float.MaxValue;
	private Vector3 targetPosition;
	private Vector3 initialPosition;

	private List<GameObject> toRender;
	
	public void SetCharacter(CharacterType newCharacter, GameObject targetCharacter, string playersName, float delay) {
		Debug.Log("New effect Loaded! " + newCharacter + " " + targetCharacter + " " + playerName + " " + delay);
		int characterId = characterPortraitLinks.IndexOf(newCharacter);
		Debug.Log("Character ID: " + characterId);
		toRender = new List<GameObject>();
		if(portrait != null){
			portrait.sprite = portraitCharacterLinks[characterId];
			toRender.Add(portrait.gameObject);
		} 
		if(nameplate != null) {
			nameplate.sprite = portraitNameBoxLinks[characterId];
			toRender.Add(nameplate.gameObject);
		} 
		if(characterName != null) {
			characterName.text = characterNameLinks[characterId];
			toRender.Add(characterName.gameObject);
		} 
		if(playerName != null) {
			playerName.text = playersName;
			toRender.Add(playerName.gameObject);
		} 
		if(portraitRenderer != null) {
			portraitRenderer.sprite = portraitCharacterLinks[characterId];
			toRender.Add(portraitRenderer.gameObject);
		}
		if(nameplateRenderer != null) {
			nameplateRenderer.sprite = portraitNameBoxLinks[characterId];
			toRender.Add(nameplateRenderer.gameObject);
		}
		if(playerNameTextMesh != null){
			playerNameTextMesh.text = playersName;
			playerNameTextMesh.GetComponent<MeshRenderer> ().sortingLayerName = sortingLayer;
			playerNameTextMesh.GetComponent<MeshRenderer> ().sortingOrder = sortingLayerIndex;
			toRender.Add(playerNameTextMesh.gameObject);
		}
		if(characterNameTextMesh != null) {
			characterNameTextMesh.text = characterNameLinks[characterId];
			characterNameTextMesh.GetComponent<MeshRenderer> ().sortingLayerName = sortingLayer;
			characterNameTextMesh.GetComponent<MeshRenderer> ().sortingOrder = sortingLayerIndex;
			toRender.Add(characterNameTextMesh.gameObject);
		} 
		this.targetCharacter = targetCharacter;
		foreach (var item in toRender)
		{
			item.SetActive(false);
		}
		this.delay = delay;
		transform.localScale = Vector3.one;
		targetPosition = new Vector3(targetCharacter.transform.position.x, targetCharacter.transform.position.y, transform.position.z);
		StartCoroutine(StartEffect());
	}

	private IEnumerator StartEffect() {
		yield return new WaitForSeconds(delay);
		foreach (var item in toRender)
		{
			item.SetActive(true);
		}
		float totalTime = 0f;
		initialPosition = transform.position;
		yield return new WaitForSeconds(displayTime / 3f);
		while(totalTime < displayTime / 3f) {
			transform.position = Vector3.Lerp(initialPosition, targetPosition, totalTime / (displayTime / 3f));
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
