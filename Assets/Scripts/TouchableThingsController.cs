using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchableThingsController : MonoBehaviour {

	public Camera camera;

	public TouchThing[] thing;
	public ControlHub controlHub;

	public bool cantTouchThis = true;

	void Update () 
	{		
		RaycastHit hit;

		if (cantTouchThis)
			return;

		if (Input.GetMouseButtonDown (0)) {

			controlHub.nexusController.touchedItem = -1; // until proven otherwise
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				Transform objectHit = hit.transform;

				for (int i = 0; i < thing.Length; ++i) {
					if (thing [i].transform == objectHit) {
						thing [i].onRayHit ();
					}
				}
			}

			if (controlHub.nexusController.touchedItem == -1) {
				controlHub.nexusController.itemUntouch ();
			}
		}
	}

	public void CanTouch(bool _can)
	{
		cantTouchThis = _can;
	}
}
