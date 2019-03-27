using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlashEffect : MonoBehaviour {

	public Texture flashEffect;
	public UIFadeScaler fl1, fl2, fl3;

	public float delay = 0.25f;

	int state;
	float timer;

	// Use this for initialization
	void Start () {
		fl1.Start ();
		fl2.Start ();
		fl3.Start ();
		state = 0;
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0)
			return;

		if (state == 1) {
			timer += Time.deltaTime;
			if (timer > delay) {
				timer = 0.0f;
				fl1.go ();
				state = 2;
			}
		}
		if (state == 2) {
			timer += Time.deltaTime;
			if (timer > delay) {
				timer = 0.0f;
				fl2.go ();
				state = 3;
			}
		}
		if (state == 3) {
			timer += Time.deltaTime;
			if (timer > delay) {
				timer = 0.0f;
				fl3.go ();
				state = 0;
			}
		}

	}

	public void go() {
		state = 1;
	}

	public void reset() {
		fl1.reset ();
		fl2.reset ();
		fl3.reset ();
		state = 0;
	}
}
