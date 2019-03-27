using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undercamera : MonoBehaviour {

	public GameObject Overcamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.rotation = Quaternion.Euler (90.0f, 0, -Overcamera.transform.eulerAngles.y);

		this.transform.position = new Vector3 (
			Overcamera.transform.position.x,
			-4600, 
			Overcamera.transform.position.z);
	}
}
