using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPieceAux : MonoBehaviour {

	public ControlHub controlHub;
	public int landID;


	public void touchCallback() {
		controlHub.worldMapController.touchLand (landID);
	}

	public void enterCallback() {
		controlHub.worldMapController.enterLand (landID);
	}

	public void exitCallback() {
		controlHub.worldMapController.exitLand (landID);
	}

}
