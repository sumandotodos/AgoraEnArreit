using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaleFader : Task {

	public float minScale = 0.0f;
	public float maxScale = 1.0f;
	public float speed = 1.0f;

	// non public start
	public float scale;
	public float targetScale;

	public bool finished;
	// non public end

	public bool startMin = true;

	bool initialized = false;

	// Use this for initialization
	public void Start () {
		if (!initialized)
			initialize ();
	}

	public void initialize() {
		initialized = true;
		if (startMin)
			reset ();
		else
			maxreset ();
	}

	// Update is called once per frame
	void Update () {

		if (finished)
			return;

		bool change = Utils.updateSoftVariable (ref scale, targetScale, speed);
		if (change) {
			this.transform.localScale = new Vector3 (scale, scale, scale);
		} else {
			notifyFinishTask ();
			finished = true;
		}

	}

	public void scaleIn() {
		finished = false;
		targetScale = maxScale;
	}

	public void scaleOut() {
		finished = false;
		targetScale = minScale;
	}

	public void scaleOutTask(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		finished = false;
		targetScale = minScale;
	}

	public void reset() {
		finished = true;
		scale = targetScale = minScale;
		this.transform.localScale = new Vector3(scale, scale, scale);
	}

	public void maxreset() {
		finished = true;
		scale = targetScale = maxScale;
		this.transform.localScale = new Vector3(scale, scale, scale);
	}
}
