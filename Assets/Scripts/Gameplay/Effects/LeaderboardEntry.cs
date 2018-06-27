using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public Image portrait;
    public TMP_Text scoreText;
    public string beforeText = "$";
    public string afterText = "k";

    public List<CharacterType> characterPortraitLinks;
    public List<Sprite> portraitCharacterLinks;

    private ScoreTracker tracker;
    private RectTransform _transform;
    private float targetYPos = 0f;
    private int displayScore = 0;

    void Start()
    {
        _transform = GetComponent<RectTransform>();
    }

    public void SetScoreTracker(ScoreTracker newTracker)
    {
        tracker = newTracker;
        scoreText.text = tracker.Score.ToString();

        portrait.sprite = portraitCharacterLinks[characterPortraitLinks.IndexOf(newTracker.Character)];
    }

    public void SetVerticalPositionTarget(float yPos)
    {
        targetYPos = yPos;
    }

    void Update()
    {
        displayScore = tracker.Score;
        if (Mathf.Abs(_transform.anchoredPosition.y + targetYPos) > .25f)
        {
            _transform.anchoredPosition = Vector2.Lerp(_transform.anchoredPosition, Vector2.down * targetYPos, .3f);
        }

        scoreText.text = beforeText + displayScore.ToString() + afterText;
    }
}