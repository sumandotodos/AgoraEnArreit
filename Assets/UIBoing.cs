using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoing : MonoBehaviour {

	float scale;
	float targetScale;
	public float angleSpeed = 1.0f;
	public float speed = 2.0f;
	float timer;
	public bool autoRun = false;

	// Use this for initialization
	void Start () {
		targetScale = scale = 0;
		timer = 0;
		this.transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		if (autoRun) {
			timer += Time.deltaTime * angleSpeed;
			targetScale = 1.0f - ((1.0f / (timer*2))) * Mathf.Sin (timer);
			Utils.updateSoftVariable (ref scale, targetScale, speed);
			if ((1.0f / (timer*2)) < 0.01f) {
				autoRun = false;
			}
			this.transform.localScale = new Vector3 (scale, scale, scale);
		}


	}

	public void reset() {

		autoRun = false;
		scale = targetScale = 0.0f;
		timer = 0.0f;
		this.transform.localScale = new Vector3 (scale, scale, scale);

	}

	public void boingUp() {

		autoRun = true;

	}
}
