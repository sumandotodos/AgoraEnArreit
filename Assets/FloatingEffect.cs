using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEffect : MonoBehaviour {

	public float amplitude;
	public float frequency;

	float angle;

	// Use this for initialization
	void Start () {
		angle = Random.Range (0.0f, 360.0f);
	}
	
	// Update is called once per frame
	void Update () {

		angle += frequency * Time.deltaTime;
		if (angle > 360.0f) {
			angle -= 360.0f;
		}
		Vector3 pos = this.transform.position;
		pos.y = amplitude * Mathf.Sin (angle);
		this.transform.position = pos;
		
	}
}
