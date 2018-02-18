using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {

	public Camera targetCamera;

	public float timeToConflict = .5f;
	public float timeToArgue = 1f;
	public float timePerStickerAnim = 1f;
	public float highlightZoomWidth = 1f;
	public float cameraSpeed = 30f;
	public float zoomSpeed = 30f;

	public AudioClip stickerNoise;
	public AudioClip rationalDiscussion;

	private Vector3 defaultCameraPos;
	private float defaultCameraWidth;

	private List<GameObject> objectsToAnimate = new List<GameObject>();
	private Dictionary<GameObject, Tile> conflictsToAnimate = new Dictionary<GameObject, Tile>();
	private Dictionary<GameObject, Tile> sourceTiles = new Dictionary<GameObject, Tile>();
	private Dictionary<ScoreTracker, PointsGiver> takeOvers = new Dictionary<ScoreTracker, PointsGiver>();

	void Start() {
		if (targetCamera == null) {
			targetCamera = Camera.main;
		}
		defaultCameraPos = targetCamera.transform.position;
		defaultCameraWidth = targetCamera.orthographicSize;
	}

	public bool HasAnimationFor(GameObject gO){
		return objectsToAnimate.IndexOf (gO) != -1;
	}

	public void SetNewTileLocation(GameObject visitor, Tile oldLocation){
		objectsToAnimate.Add (visitor);
		sourceTiles.Add (visitor, oldLocation);
	}

	public void SetNewCollision(GameObject visitor, Tile conflictTile){
		if (HasAnimationFor (visitor)) {
			sourceTiles.Remove (visitor);
		} else {
			objectsToAnimate.Add (visitor);
		}
		conflictsToAnimate.Add (visitor, conflictTile);
	}

	public void AddNewTakeOver(ScoreTracker tracker, PointsGiver target) {
		takeOvers.Add (tracker, target);
	}

	public IEnumerator PlayAllMovementAnimations() {
		//Normal Walkers
		foreach (KeyValuePair<GameObject, Tile> entry in sourceTiles) {
			GameObject gO = entry.Key;
			Tile oldTile = entry.Value;
			Direction moveToDirection =
				oldTile.GetMoveDirectionForNeighbor(gO.GetComponent<TileVisitor>().CurrentlyVisiting);
			gO.GetComponent<CharacterAnimationController>().ApplyState(StateType.Walk, 
				moveToDirection, 
				gO.GetComponent<TileVisitor>().CurrentlyVisiting.transform.position);
		}
		//Conflict walkers - move halfway!
		foreach (KeyValuePair<GameObject, Tile> entry in conflictsToAnimate) {
			GameObject gO = entry.Key;
			Tile fightTile = entry.Value;
			Direction moveToDirection =
				gO.GetComponent<TileVisitor>().CurrentlyVisiting.GetMoveDirectionForNeighbor(fightTile);
			Vector3 halfPos = Vector3.Lerp (
				gO.GetComponent<TileVisitor> ().CurrentlyVisiting.transform.position,
				fightTile.transform.position,
				                  .5f);
			gO.GetComponent<CharacterAnimationController>().ApplyState(StateType.Walk, 
				moveToDirection, 
				halfPos);
		}
		yield return new WaitForSeconds (timeToConflict);
		bool playFightNoise = false;
		foreach (KeyValuePair<GameObject, Tile> entry in conflictsToAnimate) {
			GameObject gO = entry.Key;
			if(gO.GetComponent<ScoreTracker>() != null) {
				playFightNoise = true;
			}
			gO.GetComponent<CharacterAnimationController> ().ApplyState (StateType.Fight);
			if(entry.Value.GetVisitorsOfTag("player").Count > 0){
				playFightNoise = true;
				entry.Value.GetVisitorsOfTag("player")[0].gameObject.GetComponent<CharacterAnimationController> ().ApplyState (StateType.Fight);
			}
		}
		if(playFightNoise) {
			GetComponent<AudioSource> ().PlayOneShot (rationalDiscussion);
		}
		yield return new WaitForSeconds (timeToArgue);
		//Conflict walkers - move back!
		foreach (KeyValuePair<GameObject, Tile> entry in conflictsToAnimate) {
			GameObject gO = entry.Key;
			Tile fightTile = entry.Value;
			Direction moveToDirection =
				fightTile.GetMoveDirectionForNeighbor(gO.GetComponent<TileVisitor>().CurrentlyVisiting);
			gO.GetComponent<CharacterAnimationController>().ApplyState(StateType.Walk, 
				moveToDirection, 
				gO.GetComponent<TileVisitor> ().CurrentlyVisiting.transform.position);
			if(entry.Value.GetVisitorsOfTag("player").Count > 0){
				entry.Value.GetVisitorsOfTag("player")[0].gameObject.GetComponent<CharacterAnimationController> ().ApplyState (StateType.Idle);
			}
		}
		yield return new WaitForSeconds (timeToConflict);
		foreach (KeyValuePair<GameObject, Tile> entry in conflictsToAnimate) {
			GameObject gO = entry.Key;
			gO.GetComponent<CharacterAnimationController> ().ApplyState (StateType.Idle);
		}

		objectsToAnimate.Clear ();
		conflictsToAnimate.Clear ();
		sourceTiles.Clear ();
	}

	public IEnumerator PlayTakeOverAnimations(){
		foreach (KeyValuePair<ScoreTracker, PointsGiver> entry in takeOvers) {
			ScoreTracker tracker = entry.Key;
			PointsGiver pointsGiver = entry.Value;
			yield return StartCoroutine (MoveCameraToPosition (tracker.GetComponent<TileVisitor> ().CurrentlyVisiting.transform.position));
			yield return StartCoroutine(ZoomCamera(highlightZoomWidth));
			tracker.GetComponent<CharacterAnimationController>().ApplyState(StateType.Sticker,
				pointsGiver.GetComponent<HumanAnimationController>());
			GetComponent<AudioSource> ().PlayOneShot (stickerNoise);
			yield return new WaitForSeconds (timePerStickerAnim);
			yield return StartCoroutine(ZoomCamera(defaultCameraWidth));
		}
		yield return StartCoroutine (MoveCameraToPosition (defaultCameraPos));
		takeOvers.Clear ();
		objectsToAnimate.Clear ();
	}

	private IEnumerator MoveCameraToPosition(Vector3 pos){
		Vector3 targetPos = new Vector3 (pos.x, pos.y, defaultCameraPos.z);
		while (targetCamera.transform.position != targetPos) {
			float distace = Time.deltaTime * cameraSpeed;
			if (Vector3.Distance (targetCamera.transform.position, targetPos) <= distace) {
				targetCamera.transform.position = targetPos;
			} else {
				targetCamera.transform.Translate ((targetPos - targetCamera.transform.position).normalized * distace);
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	private IEnumerator ZoomCamera(float zoomValue){
		while (targetCamera.orthographicSize != zoomValue) {
			float distace = Time.deltaTime * zoomSpeed;
			if (Mathf.Abs (targetCamera.orthographicSize - zoomValue) <= distace) {
				targetCamera.orthographicSize = zoomValue;
			} else {
				float direction = targetCamera.orthographicSize > zoomValue ? -1f : 1f;
				targetCamera.orthographicSize += distace + direction;
			}
			yield return new WaitForEndOfFrame ();
		}
	}

}
