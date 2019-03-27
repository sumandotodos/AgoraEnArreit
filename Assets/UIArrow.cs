using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction { left, right, down };

public class UIArrow : MonoBehaviour {

	public RawImage img;

	public float initialPhase; // phase goes from 0 to 1
	public Direction direction;

	public float phaseSpeed = 0.15f;
	float phase;

	float initialX;
	float initialY;

	float globalOpacity;
	float targetOpacity;
	const float opacitySpeed = 1.0f;

	bool started = false;

	int state;

	public void Start () 
	{
		if (started)
			return;
		started = true;

		globalOpacity = 0.0f;
		img.color = new Color (1, 1, 1, globalOpacity);

		//img = this.GetComponent<RawImage> ();

		if (direction == Direction.left) {
			initialX = Screen.width * 0.15f;
		} 
		else if (direction == Direction.down) 
		{
			initialY = Screen.height * 0.75f;
		}
		else {
			initialX = Screen.width * 0.85f;
		}

		phase = initialPhase;

		state = 0;
	}
	
	void Update () 
	{
		if (phase < 0.33f) {
			float t = phase / 0.33f;
			img.color = new Color (1, 1, 1, t * globalOpacity);
		} else if (phase > 0.66f) {
			float t = (phase - 0.66f) / 0.33f;
			img.color = new Color (1, 1, 1, (1.0f - t) * globalOpacity);
		} else {
			img.color = new Color (1, 1, 1, globalOpacity);
		}

		if (direction == Direction.left)
			this.transform.position = new Vector3(initialX - phase * Screen.width * 0.1f, Screen.height/2, 0);
		else if (direction == Direction.down)
		{
			this.transform.position = new Vector3 (Screen.width / 2, initialY - phase * Screen.height * 0.1f, 0);
		}
		else
			this.transform.position = new Vector3(initialX + phase * Screen.width * 0.1f, Screen.height/2, 0);

		if (phase > 1.0f)
			phase -= 1.0f;

		phase += phaseSpeed * Time.deltaTime;

		Utils.updateSoftVariable (ref globalOpacity, targetOpacity, opacitySpeed);
	}


	public void fadeOut() {
		targetOpacity = 0.0f;
	}

	public void fadeIn() {
		targetOpacity = 1.0f;
	}

	public void setFadeValue(float v) {
		targetOpacity = globalOpacity = v;
	}
}
