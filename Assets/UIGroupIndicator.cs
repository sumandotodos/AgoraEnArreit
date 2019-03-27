using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGroupIndicator : MonoBehaviour {

	public RawImage theImage;
	float scale;
	int number;

	Vector2 initialLocation;

	public void setPosition(Vector3 pos) {
		this.transform.position = pos;
	}

	public void setInitialLocation(Vector2 loc) {

		initialLocation = loc;

	}

	public void setColor(Color c) {
		theImage.color = c;
	}

	public void setIndex(int i) {
		float displacement = ((float)i) - ((float)(number-1.0f)) / 2.0f;
		this.transform.position = initialLocation + new Vector2 (displacement, 0);

	}

	public void setNumber(int n) {
		number = n;
		scale = (1.0f / ((float)n)) * 0.9f;
	}

	// Use this for initialization
	void Start () {
		scale = 1.0f;
		this.transform.localScale = new Vector3(scale, scale, scale);
	}
	

}
