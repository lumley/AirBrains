using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour {

	public List<GameObject> places;
	public List<CharacterType> characterPortraitLinks;
	public List<Sprite> portraitCharacterLinks;
	public List<Sprite> winnerPortraitCharacterLinks;

	public AudioClip victorySound;

	void OnEnable(){
		GetComponent<AudioSource> ().PlayOneShot (victorySound);
		List<ScoreTracker> trackers = FindObjectsOfType<ScoreTracker> ().OrderByDescending(score=>score.Score).ToList();
		for (int placeIndex = 0; placeIndex < places.Count; placeIndex++) {
			if (trackers.Count <= placeIndex) {
				places [placeIndex].SetActive (false);
			} else {
				places [placeIndex].SetActive (true);
				Image winnerImage = places [placeIndex].GetComponentInChildren<Image> ();
				if (winnerImage != null) {
					winnerImage.sprite = placeIndex == 0 
						? winnerPortraitCharacterLinks[characterPortraitLinks.IndexOf (trackers[placeIndex].Character)]
						: portraitCharacterLinks [characterPortraitLinks.IndexOf (trackers[placeIndex].Character)];
				}
			}
		}
	}
}
