using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Secret : MonoBehaviour {

	public ControlHub controlHub;

	public int id;
	bool active = true;

	public void touchCallback() {

		if (!active)
			return;
		controlHub.worldMapController.showSecret (id);
		this.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.2f);
		this.GetComponent<RawImage> ().raycastTarget = false;
		active = false;


	}
}
