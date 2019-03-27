using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILerp : MonoBehaviour {

	public GameObject target;

	public Vector3 pos;
	public Vector3 originalPos;

	public float T = 0.0f;
	const float threshold = 2.0f;
	public float TSpeed = 0.6f;


	public bool moving = false;

	bool started = false;

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		pos = originalPos = this.transform.position;
	}

	public void move() {
		moving = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!moving)
			return;
		T += TSpeed * Time.deltaTime;
		pos = Vector3.Lerp (originalPos, target.transform.position, linToSoft(T));
		this.transform.position = pos;
		if (T >= 1.0f) {
			moving = false;
		}
	}

	public float linToSoft(float x) {
		return 0.5f + ((Mathf.Exp ((-2.5f) + x * 5.0f) - Mathf.Exp (-((-2.5f) + x * 5.0f))) / (Mathf.Exp ((-2.5f) + x * 5.0f) + Mathf.Exp (-((-2.5f) + x * 5.0f)))) / 2.0f;
	}
}
