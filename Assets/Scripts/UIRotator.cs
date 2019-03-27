using UnityEngine;
using System.Collections;

public class UIRotator : MonoBehaviour {

	public float speed;
	public float initialPhase;

	// Use this for initialization
	void Start () {
	
		initialPhase = Random.Range (0.0f, 6.28f);
		this.transform.Rotate (new Vector3 (0, 0, initialPhase));

	}
	
	// Update is called once per frame
	void Update () {

		this.transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));
	
	}
}
