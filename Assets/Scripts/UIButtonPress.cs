using UnityEngine;
using System.Collections;

public class UIButtonPress : MonoBehaviour {

	public ControlHub controllerHub_N;
	public MonoBehaviour buttonPressListener_N;
	//public AudioController audioController;
	public AudioClip sound_N;


	public float maxScale = 1.0f;
	public float minScale = 0.8f;
	float scale;

	public bool enabled = true;

	bool pressed = false;

	public bool executeOnPress = true;

	// Use this for initialization
	void Start () {
		//audioController = GameObject.Find ("AudioController").GetComponent<AudioController> ();
		scale = 1.0f;
		pressed = false;
		this.transform.localScale = new Vector3 (maxScale, maxScale, maxScale);

	}


	public void onPress() {
		if (!enabled)
			return;
		this.transform.localScale = new Vector3 (minScale, minScale, minScale);


		pressed = true;
	}

	public void onRelease() {
		if (!enabled)
			return;
		
		this.transform.localScale = new Vector3 (maxScale, maxScale, maxScale);
		pressed = false;
	}

	public void toggle() {
		if (pressed)
			onRelease();
		else
			onPress ();
	}


}
