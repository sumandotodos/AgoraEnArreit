using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlightControl : MonoBehaviour {

	public TouchNavigation nagivation;

	float forward;
	float rotation;

	public Text outOfRangeText;

	public float fMult;
	public float rMult;

	public float angleSpeed;
	public float fSpeed;

	bool manual = true;

	public float angleDissipation;
	public float fSpeedDissipation;

	public Vector3 fuckThisShit;
	public float desiredBearing;
	public float distanceToCenter;

	const float speedThreshold = 0.05f;
	const float angleThreshold = 0.05f;

	public float distFromCenter = 0.0f;

	public Vector3 pos;

	public float AreaSize;

	// Use this for initialization
	void Start () {
		outOfRangeText.enabled = false;
		outOfRangeText.text = "FUERA DEL ÁREA DE DEBATE";
	}

	public float fControl(float desiredForwardSpeed) {
		float res = 0.0f;
		if (fSpeed > desiredForwardSpeed) {
			res = -Mathf.Min (fSpeed - desiredForwardSpeed, fMult);
		} else if (fSpeed < desiredForwardSpeed) {
			res = Mathf.Min (fSpeed - desiredForwardSpeed, fMult);
		}
		return res;

	}

	public float rControl(float desiredAngularSpeed) {
		return 0.0f;
	}

	int autoState = 0;
	
	void Update () 
	{
		distFromCenter = this.transform.position.magnitude;
		if (distFromCenter > AreaSize) {
			while (this.transform.position.magnitude > AreaSize) {
				Vector3 v = this.transform.position;
				v *= 0.95f;
				this.transform.position = v;
				outOfRangeText.GetComponent<UIAutoDelayFadeout> ().Start();
				outOfRangeText.enabled = true;
				outOfRangeText.GetComponent<UIAutoDelayFadeout> ().show ();
			}
		}

		if (manual) {
			forward = nagivation.forward * fMult;
			rotation = nagivation.rotation * rMult;
		} else {
			
			if (Mathf.Abs (this.transform.localRotation.eulerAngles.y - desiredBearing) > 35.0f) {
				rotation = rMult;
				forward = 0.0f;
			} else {
				angleSpeed = 0.0f;
				rotation = 0.0f;
				forward = fMult;
			}
		}

		if (Mathf.Abs (forward) > 0.0f) 
		{
			fSpeed = forward;
		} else {
			fSpeed = fSpeed * (1.0f - (1.0f - fSpeedDissipation) * Time.deltaTime); // speed dissipation
		}
		if (Mathf.Abs (fSpeed) < speedThreshold)
			fSpeed = 0.0f;

		if (Mathf.Abs (rotation) > 0.0f) {
			angleSpeed = rotation;
		} else {
			angleSpeed = angleSpeed * (1.0f - (1.0f - angleDissipation) * Time.deltaTime); // angle dissipation
		}
		if (Mathf.Abs (angleSpeed) < angleThreshold)
			angleSpeed = 0.0f;

		this.transform.Rotate (new Vector3 (0, angleSpeed * Time.deltaTime, 0));
		this.transform.Translate (new Vector3 (0, 0, fSpeed * Time.deltaTime));

		fuckThisShit = this.transform.localRotation.eulerAngles;
		
	}
		

	void OnTriggerEnter(Collider other) {
//		outOfRangeText.enabled = true;
//		autoState = 1;
//		fSpeed = 0.0f;
//		angleSpeed = 0.0f;
//		pos = this.transform.position;
//		distanceToCenter = Mathf.Sqrt (this.transform.position.x * this.transform.position.x + this.transform.position.z * this.transform.position.z);
//		desiredBearing = Mathf.Acos(-this.transform.position.z / distanceToCenter)*Mathf.Rad2Deg;
//		if (this.transform.position.x < 0.0f)
//			desiredBearing = 360.0f - desiredBearing;
//		
//		
//		forward = 0.0f;
//		manual = false;

	}

	void OnTriggerExit(Collider other) {
//		outOfRangeText.enabled = false;
//		angleSpeed = 0.0f;
//		manual = true;
	}
}
