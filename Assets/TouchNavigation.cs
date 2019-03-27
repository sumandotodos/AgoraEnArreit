using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Incluidas unas pequeñas chapuzar que convierten este módulo en
// no-reutilizable fuera de este juego, pero ahorran declarar
// clases adicionales, etc.... Se pueden eliminar para
// convertirlo en reutilizable

public class TouchNavigation : Task {

	public float rotation;
	public float forward;
	public GameObject cockpit;
	public GameObject lever;
	public GameObject camera;

	Vector2 touchPoint;
	bool isTouching;
	bool working = false;

	// we love unstructured data!
	float cockpitX = 0.0f;
	float cockpitY = 0.0f;
	float cTargetX = 0.0f;
	float cTargetY = 0.0f;
	float leverX = -18.0f;
	float lTargetX = -18.0f;

	public float factor = 2.0f;
	public float leverFactor = 1.0f;

	public float cockpitRelaxationSpeed = 1.0f;

	public void startNagivation() {
		working = true;
	}

	public void stopNavigation() {
		working = false;
	}

	// Use this for initialization
	void Start () {
		isTouching = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (!working)
			return;

		if (!isTouching && Input.GetMouseButton (0)) {
			touchPoint = new Vector2 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
			isTouching = true;
		}

		if (isTouching && !Input.GetMouseButton (0)) {
			isTouching = false;
		}

		if (isTouching) {
			Vector2 newTouchPoint = new Vector2 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
			float deltaX = newTouchPoint.x - touchPoint.x; // deltaX is rotation
			float deltaY = newTouchPoint.y - touchPoint.y; // deltaY is movement
			rotation = deltaX;
			forward = deltaY;
		} else {
			rotation = 0.0f;
			forward = 0.0f;
		}
		cTargetX = forward * factor;
		cTargetY = rotation * factor;
		lTargetX = -18.0f + forward * leverFactor;

		cockpit.transform.localRotation = Quaternion.Euler (cockpitX, cockpitY, 0);
		camera.transform.localRotation = Quaternion.Euler (0, 0, cockpitY*3.3f);
		lever.transform.localRotation = Quaternion.Euler (leverX, 0, 0);

		Utils.updateSoftVariable (ref cockpitX, cTargetX, cockpitRelaxationSpeed);
		Utils.updateSoftVariable (ref cockpitY, cTargetY, cockpitRelaxationSpeed);
		Utils.updateSoftVariable (ref leverX, lTargetX, cockpitRelaxationSpeed*10);

	}
}
