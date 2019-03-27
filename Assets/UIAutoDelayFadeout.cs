using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAutoDelayFadeout : MonoBehaviour {

	public UITextFader fader;
	public float delay;
	public float remainingTime = 0.0f;

	bool started = false;

	public void Start() {
		if (started)
			return;
		started = true;
		fader.Start ();
	}

	public void show() {
		remainingTime = delay;
		fader.setOpacity (1.0f);
	}

	public void hide() {
		fader.fadeOut ();
	}
		
	// Update is called once per frame
	void Update () {
		if (remainingTime > 0.0f) {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0.0f) {
				hide ();
			}
		}
	}
}
