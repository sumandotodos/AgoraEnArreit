using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valueBlender : ValueOutputter {

	public ValueOutputter v1;
	public ValueOutputter v2;

	public float invalue1;
	public float invalue2;

	public float v1Adjust = 0.0f, v2Adjust = 0.0f;

	public float blend;
	float sourceBlend;
	float targetBlend;

	public float blendDuration = 1.0f;

	// Use this for initialization
	void Start () {
		blend = targetBlend = 0;
	}

	void updateOutput() {
		outValue = (1.0f - blend) * (v1.outValue+v1Adjust) + (blend) * (v2.outValue+v2Adjust);
		invalue1 = v1.outValue;
		invalue2 = v2.outValue;
	}
	
	// Update is called once per frame
	void Update () {
		bool change = Utils.updateSoftVariable (ref blend, targetBlend, (1.0f / blendDuration));
		if (!change)
			notifyFinishTask ();
		updateOutput ();
	}

	public void setOutput(float v) {
		blend = targetBlend = v;
		updateOutput ();
	}

	public void blendTo1Task(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		blendTo1 ();
	}

	public void blendTo2Task(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		blendTo2 ();
	}

	// use these methods to prevent zero-crossings and choose closes path in circle
	public void blendTo1() {
		if (blend == 0.0f)
			return;
		float directCost = Mathf.Abs(v1.outValue - v2.outValue);
		float zeroCrossCost = 0.0f;
		if (v1.outValue < v2.outValue) {
			zeroCrossCost = Mathf.Abs (360.0f - v2.outValue + v1.outValue);
		} else {
			zeroCrossCost = Mathf.Abs (360.0f - v1.outValue + v2.outValue);
		}
		if (zeroCrossCost < directCost) { // add 360.0 excess to smallest one
			if (v1.outValue < v2.outValue) {
				v1Adjust = 360.0f;
				v2Adjust = 0.0f;
			} else {
				v2Adjust = 360.0f;
				v1Adjust = 0.0f;
			}
		} else {
			v1Adjust = 0.0f;
			v2Adjust = 0.0f;
		}
		targetBlend = 0.0f;
	}

	public void blendTo2() {
		if (blend == 1.0f)
			return;
		float directCost = Mathf.Abs(v2.outValue - v1.outValue);
		float zeroCrossCost = 0.0f;
		if (v1.outValue < v2.outValue) {
			zeroCrossCost = Mathf.Abs (360.0f - v2.outValue + v1.outValue);
		} else {
			zeroCrossCost = Mathf.Abs (360.0f - v1.outValue + v2.outValue);
		}
		if (zeroCrossCost < directCost) {
			if (v1.outValue < v2.outValue) {
				v1Adjust = 360.0f;
				v2Adjust = 0.0f;
			} else {
				v2Adjust = 360.0f;
				v1Adjust = 0.0f;
			}
		} else {
			v1Adjust = 0.0f;
			v2Adjust = 0.0f;
		}
		targetBlend = 1.0f;
	}
}
