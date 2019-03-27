using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonTemporaryDisable : MonoBehaviour {

	public float delay = 3.0f;
	float remainingTime = -1.0f;

	public Button button;

	public void go() {
		remainingTime = delay;
		button.interactable = false;
	}

	// Update is called once per frame
	void Update () {
		if (remainingTime > 0.0f) {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0.0f) {
				button.interactable = true;
			}
		}
	}
}
