using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueAdder : ValueOutputter {

	public float constant;
	public ValueOutputter val;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		outValue = val.outValue + constant;
	}
}
