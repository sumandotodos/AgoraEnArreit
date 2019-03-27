using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public float minRadius = 5.0f;
	public float minAmplitude = 0.25f;
	public float minSpeed = 90.0f;

	public float awayRadius = 100.0f;
	public float awayAmplitude = 20.0f;
	public float awaySpeed = 200.0f;

	// the following are NOT public.
	public float targetRadius;
	public float targetAmplitude;
	public float targetSpeed;

	public float radius;
	public float amplitude;
	public float speed;

	public float changeSpeed = 30.0f;

	float angle;
	float wiggleAngle;

	Vector3 localPosition;
	Vector3 initialPosition;

	// Use this for initialization
	void Start () {

		angle = 0.0f;
		wiggleAngle = 0.0f;

		targetSpeed = minSpeed;
		targetRadius = minRadius;
		targetAmplitude = minAmplitude;

		radius = minRadius;
		amplitude = minAmplitude;
		speed = minSpeed;

		initialPosition = this.transform.position;

		localPosition = new Vector3();
		localPosition.y = amplitude * Mathf.Sin(wiggleAngle);
		localPosition.x = radius * Mathf.Cos(angle);
		localPosition.z = radius * Mathf.Sin(angle);
		
	}
	
	// Update is called once per frame
	void Update () {



		angle += speed * Time.deltaTime;
		wiggleAngle += (speed * Mathf.PI) * Time.deltaTime;

		localPosition = new Vector3();
		localPosition.y = amplitude * Mathf.Sin(wiggleAngle/360.0f*2.0f*Mathf.PI);
		localPosition.x = radius * Mathf.Cos(angle/360.0f*2.0f*Mathf.PI);
		localPosition.z = radius * Mathf.Sin(angle/360.0f*2.0f*Mathf.PI);

		this.transform.localPosition = initialPosition + localPosition;
		this.transform.localRotation = Quaternion.Euler(0, -angle, 0);

		Utils.updateSoftVariable (ref radius, targetRadius, changeSpeed);
		Utils.updateSoftVariable (ref amplitude, targetAmplitude, changeSpeed);
		Utils.updateSoftVariable (ref speed, targetSpeed, changeSpeed);

	}

	public void flyAway() {

		targetRadius = awayRadius;
		targetSpeed = awaySpeed;
		targetAmplitude = awayAmplitude;

	}

	public void comeBack() {

		targetRadius = minRadius;
		targetSpeed = minSpeed;
		targetAmplitude = minAmplitude;

	}
}
