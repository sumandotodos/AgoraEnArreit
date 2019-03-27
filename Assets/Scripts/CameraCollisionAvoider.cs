using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionAvoider : ValueOutputter {

	public float avoidanceSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	void OnTriggerStay(Collider other) {
		if (other.tag == "CameraObstacle") {
			Translator t = this.GetComponent<Translator> ();
			//t.addTrans (-avoidanceSpeed * Time.deltaTime);
		}
	}
}
