using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompoundIsland : MonoBehaviour {

	public GameObject enabledIsland;
	public GameObject disabledIsland;

	public Island island1;
	public Island island2;

	public void init() {

		enableIsland ();
		island1.isEnabled = true;
		island2.isEnabled = false;
	}

	public void setLabelColor(Color col) {
		Color colSolid = col;
		colSolid.a = 1.0f;
		Color colFaded = col;
		colFaded.a = 0.35f;
		island1.labelText.color = colSolid;
		island2.labelText.color = colFaded;
	}

	public void enableIsland() {
		enabledIsland.SetActive (true);
		disabledIsland.SetActive (false);
	}

	public void disableIsland() {
		enabledIsland.SetActive (false);
		disabledIsland.SetActive (true);
	}
}
