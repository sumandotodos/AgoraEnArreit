using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggler : MonoBehaviour {


	public float amplitude;
	public float speed;

	float angle;
	float wiggleAngle;

	public Vector3 localPosition;


	// Use this for initialization
	void Start () {

		wiggleAngle = 0.0f;


	}

	// Update is called once per frame
	void Update () {


		wiggleAngle += (speed * Mathf.PI) * Time.deltaTime;

		localPosition = Vector3.zero;
		localPosition.y = amplitude * Mathf.Sin(wiggleAngle/360.0f*2.0f*Mathf.PI);


		this.transform.localPosition = this.transform.localPosition + localPosition * Time.deltaTime;


	}


}
