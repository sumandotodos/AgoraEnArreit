using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILerpDelay : MonoBehaviour {

	public UILerp lerp;

	public float delay = 1.0f;
	public float elapsedTime;
	public bool started = false;

	public bool going = false;

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		elapsedTime = 0.0f;
		going = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!going)
			return;
		elapsedTime += Time.deltaTime;
		if (elapsedTime > delay) {
			lerp.move ();
			going = false;
		}
	}
}
