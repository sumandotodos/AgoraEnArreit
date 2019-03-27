using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchThing : MonoBehaviour {

	public int id;
	public ControlHub controlHub;

	// Use this for initialization
	void Start () {
		
	}
	
	public void onRayHit() {
		controlHub.nexusController.itemTouch (id);
	}
}
