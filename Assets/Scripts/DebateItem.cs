using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebateItem : MonoBehaviour {

	//public Rosetta rosetta;
	public RawImage leftImage;
	public RawImage rightImage;
	public Text text;
	public RawImage frame;
	public RawImage outline;

	bool isEnabled;

	public Color[] difColor;

	public void setDifficulty(int diff) {

		Color newCol = difColor [diff];
		newCol.a = 1.0f;
		frame.color = newCol;

	}
	


	public void setEnabled(bool en) {

		isEnabled = en;
		leftImage.enabled = en;
		rightImage.enabled = en;
		text.enabled = en;
		frame.enabled = en;
		outline.enabled = en;

	}

	public void setAbsent(bool en) {

		frame.enabled = !en;
		leftImage.enabled = !en;
		rightImage.enabled = !en;
		text.enabled = !en;
		outline.enabled = en;

	}
}
