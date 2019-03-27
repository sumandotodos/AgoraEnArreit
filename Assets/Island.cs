using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

	public string owner;
	public TextMesh labelText;
	public ObjectBillboard billboard;
	public Camera camera;
	public SeekPlayerController seekPlayerController;

	public bool isEnabled = true;
	
	// Update is called once per frame
	void Update () {

		if (!isEnabled) // you can't click
			return;

		RaycastHit hit;

		if (Input.GetMouseButtonDown (0) && (!seekPlayerController.showingOpponentDebates)) {

			//controlHub.nexusController.touchedItem = -1; // until proven otherwise
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				Transform objectHit = hit.transform;

				if (objectHit == this.transform) {
					seekPlayerController.clickOnIsland (owner);
				}


			}



		}

	}

	public void deactivate() {

	}

	public void setLabel(string lab) {
		labelText.text = lab;
	}


}
