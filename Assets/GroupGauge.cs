using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupGauge : MonoBehaviour {

	public void setColor(Color c) {
		RawImage img;
		img = this.GetComponent<RawImage> ();
		if (img != null) {
			img.color = c;
		}
	}

}
