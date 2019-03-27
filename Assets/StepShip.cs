using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepShip : MonoBehaviour {



	float linSpeed;
	float rotSpeed;

	float targetLinSpeed;
	float targetRotSpeed;


	public Vector3 initialPosition;

	public float nearLinSpeed;
	public float nearRotSpeed;

	public float awayLinSpeed;
	public float awayRotSpeed;

	const float updateSpeed = 25.0f;

	public void reset() {
		this.transform.position = initialPosition;
		targetLinSpeed = linSpeed = nearLinSpeed;
		targetRotSpeed = rotSpeed = nearRotSpeed;
	}

	// Use this for initialization
	void Start () {
		reset ();
	}
	
	// Update is called once per frame
	void Update () {

		Utils.updateSoftVariable (ref linSpeed, targetLinSpeed, updateSpeed);
		Utils.updateSoftVariable (ref rotSpeed, targetRotSpeed, updateSpeed);

		this.transform.Rotate (new Vector3 (0, rotSpeed * Time.deltaTime, 0));
		this.transform.Translate (new Vector3 (0, 0, linSpeed * Time.deltaTime));
	}

	public void flyAway() {
		targetLinSpeed = awayLinSpeed;
		targetRotSpeed = awayRotSpeed;
	}
}
