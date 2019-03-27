using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UIScroll : Task {

	public Axis axis;

	public float speed = 5.0f;

	float targetPos;
	float pos;

	Vector3 initialPos;

	// Use this for initialization
	void Start () {
		pos = targetPos = 0.0f;
		initialPos = this.transform.localPosition;
	}

	public void setPos(float p) {
		pos = p;
	}
	
	// Update is called once per frame
	void Update () {

		bool change = Utils.updateSoftVariable(ref pos, targetPos, speed);
		if (change) {
			this.transform.localPosition = new Vector3 (pos, initialPos.y, 0);
		} else
			notifyFinishTask ();

	}

	public void scrollTo(float dest) {
		targetPos = dest;
	}

	public void scrollTo(float dest, Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		targetPos = dest;
	}



}
