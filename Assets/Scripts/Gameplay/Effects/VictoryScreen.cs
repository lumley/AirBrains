using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour {

	public List<GameObject> places;
	public List<CharacterType> characterPortraitLinks;
	public List<Sprite> portraitCharacterLinks;

	void OnEnable(){
		List<ScoreTracker> trackers = FindObjectsOfType<ScoreTracker> ().OrderByDescending(score=>score.Score).ToList();;
		for (int placeIndex = 0; placeIndex < places.Count; placeIndex++) {
			if (trackers.Count <= placeIndex) {
				places [placeIndex].SetActive (false);
			} else {
				places [placeIndex].SetActive (true);
				Image winnerImage = places [placeIndex].GetComponentInChildren<Image> ();
				if (winnerImage != null) {
					winnerImage.sprite = portraitCharacterLinks [characterPortraitLinks.IndexOf (trackers[placeIndex].Character)];
				}
			}
		}
	}
}
