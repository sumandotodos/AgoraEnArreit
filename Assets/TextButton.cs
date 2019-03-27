using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour {

	public UIFaderScript buttonImage;
	public UITextFader buttonText;

	public void setText(string t) {
		buttonText.gameObject.GetComponent<Text> ().text = t;
	}

	public void fadeIn() {
		buttonImage.fadeOut ();
		buttonText.fadeIn ();
	}

	public void fadeOut() {
		buttonImage.fadeIn ();
		buttonText.fadeOut ();
	}

	public void setOpacity(float v) {
		buttonImage.setFadeValue (v);
		buttonText.setOpacity (v);
	}

}
