using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveClock : MonoBehaviour
{
    public Image fillImage;
    public TMP_Text text;
    public float[] textTriggers;
    public Color textColor;
    public GameObject[] toEnableWhenVisible;

    private float lastValue;
    private GameRunner runner;
    private Color clearColor;

    private void Awake()
    {
        runner = GameRunner.FindInScene();
        clearColor = new Color(textColor.r, textColor.g, textColor.b, 0f);
    }

    private void Update()
    {
        if (runner.TimeLeft > 0f)
        {
            if (!fillImage.enabled)
            {
                fillImage.enabled = true;
                text.enabled = true;
                foreach (GameObject go in toEnableWhenVisible)
                {
                    go.SetActive(true);
                }
            }

            fillImage.fillAmount = runner.TimeLeft / runner.secondsToWaitForInput;
            if (Math.Abs(lastValue - -1f) > Mathf.Epsilon)
            {
                foreach (var threshhold in textTriggers)
                {
                    if (lastValue >= threshhold && runner.TimeLeft <= threshhold)
                    {
                        text.text = Convert.ToString(threshhold, CultureInfo.InvariantCulture);
                        text.color = textColor;
                    }
                }
            }

            text.color = Color.Lerp(text.color, clearColor, .01f);
            lastValue = runner.TimeLeft;
        }
        else if (fillImage.enabled)
        {
            fillImage.enabled = false;
            text.enabled = false;
            lastValue = -1f;
            foreach (GameObject go in toEnableWhenVisible)
            {
                go.SetActive(false);
            }
        }
    }
}