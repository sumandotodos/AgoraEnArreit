using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeScaler : MonoBehaviour {

	RawImage img;

	int state = 0;

	bool initialized = false;

	public float initScale = 1.5f, initOpacity = 0.0f;
	public float endScale = 1.0f, endOpacity = 1.0f;
	public float speed = 1.0f;

	float sc, op;

	float t;
	float targetT;

	void updateAttr() {
		sc = Mathf.Lerp (initScale, endScale, t);
		op = Mathf.Lerp (initOpacity, endOpacity, t);
		this.transform.localScale = new Vector3 (sc, sc, sc);
		img.color = new Color (1, 1, 1, op);
	}

	// Use this for initialization
	public void Start () {
		img = this.GetComponent<RawImage> ();
		t = targetT = 0;
		updateAttr ();
		state = 0;
		initialized = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0)
			return;

		if (state == 1) {
			bool change = Utils.updateSoftVariable (ref t, targetT, speed);
			if (!change) {
				state = 0;
			}
			updateAttr ();
		}

	}

	public void go() {
		targetT = 1.0f;
		state = 1;
	}

	public void reset() {
		if (!initialized)
			Start ();
		t = targetT = 0.0f;
		updateAttr ();
	}

}
