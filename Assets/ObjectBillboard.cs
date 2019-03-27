using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBillboard : MonoBehaviour {
	
	public string cameraName = "SeekCamera";
	public Camera cam;

	// Use this for initialization
	void Start () {
		cam = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (cam != null) {
			transform.LookAt (transform.position + cam.transform.rotation * Vector3.forward,
				cam.transform.rotation * Vector3.up);
		} else {
			cam = GameObject.Find (cameraName).GetComponent<Camera> ();
		}
	}
}
