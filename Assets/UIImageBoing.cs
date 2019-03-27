using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIImageBoing : MonoBehaviour {

	public float initialAmplitude;
	float amplitude;
	float phase;
	public float speed;

	bool running = false;

	// Use this for initialization
	void Start () {
		reset ();
	}

	public void reset() {
		phase = 180.0f;
		amplitude = initialAmplitude;
		running = false;
	}

	// Update is called once per frame
	void Update () {
		if (running) {
			float s = 1.0f;
			if (amplitude > 0.01) {
				s = 1.0f + amplitude * Mathf.Cos (phase);
				Utils.updateSoftVariable (ref amplitude, 0.0f, speed);
				phase += 20.0f * speed * Time.deltaTime;
			}
			this.transform.localScale = new Vector3 (s, s, s);
		}
			
	}

	public void boing() {
		running = true;
	}
}
