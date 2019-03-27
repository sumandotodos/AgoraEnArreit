using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineText : MonoBehaviour {

	public Text mainText;
	public Text[] shadows;
	//public OutlineText shadows;

	public void setText(string t) {
		mainText.text = t;
		for (int i = 0; i < shadows.Length; ++i) {
			shadows [i].text = t;
		}
	}

	public void setColor(Color col) {
		col.a = 1.0f;
		mainText.color = col;
	}

}
