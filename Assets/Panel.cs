using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour {

	public ControlHub controlHub;
	public UIFaderScript fader;

	public int i;
	public int j;

	public void touchCallback() {

		if (controlHub.gameController.numberOfPanels > 0) {
			--controlHub.gameController.numberOfPanels;
			controlHub.worldMapController.clearPanel (i-1, j-1, true);
		}

	}

}
