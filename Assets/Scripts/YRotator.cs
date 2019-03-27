using UnityEngine;
using System.Collections;

public class YRotator : MonoBehaviour {

	public float ySpeed;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		this.transform.Rotate (new Vector3 (0, ySpeed * Time.deltaTime, 0));

	}
}
