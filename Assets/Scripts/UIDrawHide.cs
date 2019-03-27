using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrawHide : Task {

	public Axis axis = Axis.Y;

	public float hideY = -Screen.height;
	public float showY = 0.0f;
	public float speed = 200.0f;
	public bool startHidden = false;

	float y;
	float targetY;

	bool started = false;

	int state;

	private void updateTransform() {
		if (axis == Axis.Y) {
			this.transform.localPosition = new Vector3 (0, y, 0);
		} else if (axis == Axis.X) {
			this.transform.localPosition = new Vector3 (y, 0, 0);
		}
	}

	// Use this for initialization
	public void Start () {
		
		if (started)
			return;
		started = true;
		if (startHidden) {
			y = hideY;
		} 
		else {
			y = showY;
		}
		updateTransform ();
		state = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0) { // idling

		}

		if (state == 1) { // moving
			bool changed = Utils.updateSoftVariable(ref y, targetY, speed);
			if (changed) {
				updateTransform ();
			} 
			else {
				notifyFinishTask ();
				state = 0;
			}
		}

	}

	public void hideTask(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		hide ();
	}

	public void showTask(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		show ();
	}

	public void hide() {
		targetY = hideY;
		state = 1;
	}

	public void show() {
		targetY = showY;
		state = 1;
	}

	public void hideImmediate() {

		y = targetY = hideY;
		updateTransform ();
		state = 0;

	}
}
