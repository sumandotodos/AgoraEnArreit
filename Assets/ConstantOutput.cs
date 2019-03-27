using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantOutput : ValueOutputter {

	public float value;
	
	// Update is called once per frame
	void Update () {
		outValue = value;
	}
}
