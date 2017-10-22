using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveClock : MonoBehaviour {

	public Image fillImage;
	public Text text;
	public float[] textTriggers;
	public Color textColor;

	private float lastValue;
	private GameRunner runner;
	private Color clearColor;

	void Awake() { 
		runner = GameRunner.FindInScene ();
		clearColor = new Color (textColor.r, textColor.g, textColor.b, 0f);
	}

	void Update () {
		if (runner.TimeLeft > 0f) {
			if (!fillImage.enabled) {
				fillImage.enabled = true;
				text.enabled = true;
			}
			fillImage.fillAmount = runner.TimeLeft / runner.secondsToWaitForInput;
			if (lastValue != -1f) {
				foreach (float threshhold in textTriggers) {
					if (lastValue >= threshhold && runner.TimeLeft <= threshhold) {
						text.text = "" + threshhold;
						text.color = textColor;
					}
				}
			}
			text.color = Color.Lerp (text.color, clearColor, .1f);
			lastValue = runner.TimeLeft;
		} else {
			if (fillImage.enabled) {
				fillImage.enabled = false;
				text.enabled = false;
				lastValue = -1f;
			}
		}
	}
}
