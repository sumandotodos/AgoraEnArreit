using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFaderScript : Task {

	public Text outlog_N;

	float opacity;
	float targetOpacity;
	public Color fadeColor;
	public float fadeSpeed = 0.9f;
	RawImage imageComponent;
	public bool autoFadeIn;
	public float initialOpacity;
	public float fadeInValue = 0.0f;
	public float fadeOutValue = 1.0f;
	bool started = false;

	// Use this for initialization
	public void Start () {
		if (started == true)
			return;
		imageComponent = this.GetComponent<RawImage> ();
		opacity = initialOpacity;
		targetOpacity = initialOpacity;
		imageComponent.color = new Color (fadeColor.r, fadeColor.g, fadeColor.b, opacity);
		if (initialOpacity > 0.0f) {
			imageComponent.enabled = true;
		} else
			imageComponent.enabled = false;
		if (autoFadeIn) {
			opacity = 1.0f;
			imageComponent.enabled = true;
			imageComponent.color = new Color (fadeColor.r, fadeColor.g, fadeColor.b, opacity);
			fadeIn ();
		}
		started = true;
	}

	public void setFadeValue(float nOp) {
		opacity = targetOpacity = nOp;
		if (imageComponent == null)
			return;
		if (nOp > 0.0f) {
			imageComponent.enabled = true;
			imageComponent.color = new Color (fadeColor.r, fadeColor.g, fadeColor.b, opacity);
		} else {
			imageComponent.enabled = false;
		}
		
	}

	public void fadeInTask(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		fadeIn ();
	}

	public void fadeOutTask(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		fadeOut ();
	}

	public void fadeIn() {
		targetOpacity = fadeInValue;
	}

	public void fadeOut() {
		if (imageComponent == null)
			imageComponent = this.GetComponent<RawImage> ();
		if(imageComponent == null) return;
		imageComponent.enabled = true;
		targetOpacity = fadeOutValue;
	}
	
	// Update is called once per frame
	void Update () {
	

		bool change = Utils.updateSoftVariable (ref opacity, targetOpacity, fadeSpeed);
		if (change) {
			imageComponent.color = new Color (fadeColor.r, fadeColor.g, fadeColor.b, opacity);

		} else {
			if (opacity == fadeInValue) {
				if(opacity == 0.0f) imageComponent.enabled = false;

			} else if (opacity == fadeOutValue) { 
				
			}
			notifyFinishTask ();
		}

		if (outlog_N != null) {
			string s; 
			if (waiter != null) {
				s = "(W!)";
			} else
				s = "()";
			outlog_N.text = "change: " + change + ", waitsatate: " + s;
		}


	}

	public void setFadeInValue(float f) {
		fadeInValue = f;
	}
	public void setFadeOutValue(float f) {
		fadeOutValue = f;
	}
}
