using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public GameObject knob;
	public GameObject body;

	public float endKnobTwist =  -20.0f;
	public float endBodyTwist = 20.0f;
	public float speed = 20.0f;

	public float knobTwist;
	public float bodyTwist;

	public int state = 0;
	public int state2 = 0;

	float timer = 0.0f;
	float timer2 = 0.0f;

	// Use this for initialization
	void Start () {
		state = 0;
		state2 = 0;
		timer = 0.0f;
		timer2 = 0.0f;
		knobTwist = 0;
		bodyTwist = 0;
	}
	
	// Update is called once per frame
	void Update () {

		// sm.createState("Open");
		// sm.startWhile(ref angle, '<', 0);
		// sm.execut
		// sm.endWhile();



		// idle
		if (state == 0) {
			
		}




		// open sequence
		if (state == 1) { // turning knob
			bool change = Utils.updateSoftVariable (ref knobTwist, endKnobTwist, speed);
			knob.transform.localRotation = Quaternion.Euler (new Vector3 (knobTwist, 0, 0));
			if (!change) {
				state = 2;
			}
		}

		if (state == 2) {
			timer += Time.deltaTime;
			if (timer > 0.75f) {
				state = 3;
				state2 = 1;
			}
		}

		if (state == 3) {
			bool change = Utils.updateSoftVariable (ref bodyTwist, endBodyTwist, speed);
			body.transform.localRotation = Quaternion.Euler (new Vector3 (0, bodyTwist, 0));
			if (!change) {
				state = 4;
			}
		}

		if (state == 4) {

		}


		// close sequence
		if (state == 10) {
			bool change = Utils.updateSoftVariable (ref bodyTwist, 0, speed * 2);
			body.transform.localRotation = Quaternion.Euler (new Vector3 (0, bodyTwist, 0));
			if (!change) {
				body.transform.localRotation = Quaternion.Euler (new Vector3 (0, bodyTwist, 0));
				state = 0;
			}
		}
			




		// slot 2: knob rotation
		// idle
		if (state2 == 0) {

		}
		if (state2 == 1) {
			timer2 += Time.deltaTime;
			if (timer2 > 0.5f) {
				state2 = 2;
			}
		}
		if (state2 == 2) {
			bool change = Utils.updateSoftVariable (ref knobTwist, 0, speed);
			knob.transform.localRotation = Quaternion.Euler (new Vector3 (knobTwist, 0, 0));
			if (!change) {
				knob.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				state2 = 0;
			}
		}

	}

	public void open() {
		state = 1;
	}

	public void close() {
		state = 10;
		state2 = 2;
	}
}
