using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valueTarget : ValueOutputter {

	public float[] values;

	public int target;
	float value;
	float targetValue;
	public float duration = 1.0f;
	public int channel = 0;

	// Use this for initialization
	void Start () {
		target = 0;
		value = targetValue = values [channel];
	}

	public void setChannel(int c) {
		channel = c;
		outValue = values [channel];
	}
	

}
