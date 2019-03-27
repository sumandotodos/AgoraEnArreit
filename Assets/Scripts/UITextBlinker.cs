using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum BlinkStyle { Blink, XFade };

public class UITextBlinker : MonoBehaviour {

	public BlinkStyle blinkStyle;
	public float speed;
	Text text;
	float time;
	bool direction;
	public UITextBlinker[] stopOthers_N; // for mutual exclusion
	bool disabled = true;
	int state;

	float r, g, b;

	void Start () 
	{
		time = 0.0f;
		state = 0;
		direction = false;
		text = this.GetComponent<Text> ();
		Color c = text.color;
		r = c.r;
		g = c.g;
		b = c.b;
	}
	
	void Update () 
	{	
		if (state == 0) { // idle

		} 

		if (state == 1) { // going


			time += Time.deltaTime;
			if (time > (1.0f / speed)) {
				time = 0.0f;
				if (text.enabled == true)
					text.enabled = false;
				else
					text.enabled = true;
			}

		}
	}

	public void startBlink() 
	{
		state = 1;
		for (int i = 0; i < stopOthers_N.Length; ++i) 
		{
			stopOthers_N [i].stopBlink ();
		}
	}

	public void stopBlink()
	{
		/*if (disabled) {
			text.enabled = false;
			return;
		}*/
		state = 0;
		direction = false;
		text.enabled = false;
		text.color = new Color (r, g, b, 1);
	}

	public void disable() 
	{
		disabled = true;
	}
}
